using Bulut_Album.Data;
using Bulut_Album.Helper;
using Bulut_Album.Models;
using Bulut_Album.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Bulut_Album.Models.DTO.Customer;
using Bulut_Album.Services;

namespace Bulut_Album.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly JwtService _jwtService;
        public CustomerController(AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == request.Email))
                return BadRequest("Bu e-posta adresi zaten kayıtlı.");

            request.Password = PasswordHelper.HashPassword(request.Password); // düz şifreyi hashliyoruz
            _context.Customers.Add(new Customer
            {
                CustomerId = new Guid(),
                Email = request.Email,
                FullName = request.FullName,
                PasswordHash = request.Password,
                CreatedAt=DateTime.UtcNow,
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Kayıt başarılı"});
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var user = await _context.Customers.SingleOrDefaultAsync(c => c.Email == model.Email);
            if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
                return Unauthorized("Geçersiz e-posta veya şifre.");

            var token = _jwtService.GenerateToken(user.CustomerId, user.Email);

            return Ok(new
            {
                message = "Giriş başarılı",
                token = token,
            });
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

 
}
