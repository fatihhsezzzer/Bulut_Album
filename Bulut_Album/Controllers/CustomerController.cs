using Bulut_Album.Data;
using Bulut_Album.Helper;
using Bulut_Album.Models;
using Bulut_Album.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bulut_Album.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CustomerController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Customer customer)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == customer.Email))
                return BadRequest("Bu e-posta adresi zaten kayıtlı.");

            customer.PasswordHash = PasswordHelper.HashPassword(customer.PasswordHash); // düz şifreyi hashliyoruz
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kayıt başarılı", customerId = customer.CustomerId });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await _context.Customers.SingleOrDefaultAsync(c => c.Email == model.Email);
            if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
                return Unauthorized("Geçersiz e-posta veya şifre.");

            return Ok(new { message = "Giriş başarılı", customerId = user.CustomerId });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            var user = await _context.Customers.SingleOrDefaultAsync(c => c.Email == model.Email);
            if (user == null)
                return NotFound("Kullanıcı bulunamadı.");

            user.PasswordHash = PasswordHelper.HashPassword(model.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Şifre sıfırlandı." });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
    }
}
