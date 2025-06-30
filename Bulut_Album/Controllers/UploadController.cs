using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bulut_Album.Data;
using Bulut_Album.Models;
using Microsoft.Extensions.FileProviders;


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

            var allowedTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedTypes.Contains(request.File.ContentType))
                return BadRequest("Sadece resim dosyası yükleyebilirsiniz (jpg/png).");

            var folder = Path.Combine(_env.ContentRootPath, "Uploads", customerId);
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            _context.UploadLogs.Add(new UploadLog
            {
                FileName = fileName,
                SavedPath = filePath,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CustomerId = customerId,
                UploadDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok("Yükleme başarılı.");
        }

        // ✅ Fotoğrafları listeleme endpoint’i
        [Authorize]
        [HttpGet("list")]
        public IActionResult ListUploads()
        {
            var customerId = User.FindFirst("CustomerId")?.Value;

            var uploads = _context.UploadLogs
                .Where(u => u.CustomerId == customerId)
                .OrderByDescending(u => u.UploadDate)
                .Select(u => new
                {
                    u.FileName,
                    u.FirstName,
                    u.LastName,
                    u.UploadDate,
                    Url = $"https://{Request.Host}/uploads/{customerId}/{u.FileName}"
                })
                .ToList();

            return Ok(uploads);
        }
    }
}
