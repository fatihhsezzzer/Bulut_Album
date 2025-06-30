namespace Bulut_Album.Models
{
    public class UploadRequest
    {
        public IFormFile File { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

}
