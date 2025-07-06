using Microsoft.AspNetCore.Mvc;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing;
using System.Drawing.Imaging;
using Bulut_Album.Services;

namespace Bulut_Album.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public QrController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet("{customerId}")]
        public IActionResult GenerateQr(Guid customerId)
        {
            // 1. Token oluştur
            var token = _tokenService.GenerateCustomerUploadToken(customerId);

            // 2. QR içeriği
            var uploadUrl = $"https://bulutalbum.com/yukle?token={token}";

            // 3. QR kod oluştur
            var qrWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 300,
                    Width = 300,
                    Margin = 1
                }
            };

            var pixelData = qrWriter.Write(uploadUrl);

            // 4. PNG bitmap oluştur
            using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format32bppRgb
            );
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            // 5. Belleğe PNG olarak yaz
            using var stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Png);
            stream.Position = 0;

            return File(stream.ToArray(), "image/png", $"qr-customer-{customerId}.png");
        }
    }
}
