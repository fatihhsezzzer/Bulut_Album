using Bulut_Album.Data;
using Bulut_Album.Models;
using Bulut_Album.Models.Entities;
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

        private readonly S3Service _s3Service;

        public UploadController(AppDbContext context, IWebHostEnvironment env, S3Service s3Service)
        {
            _context = context;
            _env = env;
            _s3Service = s3Service;
        }


        [HttpPost]
        public async Task<IActionResult> Upload([FromForm] UploadRequest request)
        {
            var customerId = Guid.Parse(User.FindFirst("CustomerId")?.Value);

            if (request.File == null || request.File.Length == 0)
                return BadRequest("Dosya boş.");

            if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                return BadRequest("İsim ve soyisim gerekli.");

            var allowedTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedTypes.Contains(request.File.ContentType))
                return BadRequest("Sadece resim dosyası yükleyebilirsiniz (jpg/png).");

            var newFileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);

            string uploadedUrl;
            using (var stream = request.File.OpenReadStream())
            {
                uploadedUrl = await _s3Service.UploadFileAsync(stream, $"customers/{customerId}/{newFileName}", request.File.ContentType);
            }

            _context.Media.Add(new Media
            {
                FileName = request.File.FileName,
                FilePath = uploadedUrl, // URL artık dosya yolu
                FirstName = request.FirstName,
                LastName = request.LastName,
                CustomerId = customerId,
                UploadDate = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            return Ok(new { message = "Yükleme başarılı.", url = uploadedUrl });
        }

    }
}
