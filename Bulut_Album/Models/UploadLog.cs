namespace Bulut_Album.Models
{
    public class UploadLog
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string SavedPath { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomerId { get; set; }
        public DateTime UploadDate { get; set; }
    }


}
