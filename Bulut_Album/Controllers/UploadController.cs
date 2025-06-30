using Bulut_Album.Data;
using Bulut_Album.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MyUploadApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UploadController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadRequest request)
        {
            var customerId = User.FindFirst("CustomerId")?.Value;

            if (request.File == null || request.File.Length == 0)
                return BadRequest("Dosya boş.");

            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                return BadRequest("İsim ve soyisim gerekli.");

            // Sadece resim kontrolü
            var allowedTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedTypes.Contains(request.File.ContentType))
                return BadRequest("Sadece resim dosyası yükleyebilirsiniz (jpg/png).");

            var folder = Path.Combine(_env.ContentRootPath, "Uploads", customerId);
            Directory.CreateDirectory(folder);

            var filePath = Path.Combine(folder, Guid.NewGuid() + Path.GetExtension(request.File.FileName));

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            _context.UploadLogs.Add(new UploadLog
            {
                FileName = request.File.FileName,
                SavedPath = filePath,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CustomerId = customerId,
                UploadDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok("Yükleme başarılı.");
        }


    }
}
