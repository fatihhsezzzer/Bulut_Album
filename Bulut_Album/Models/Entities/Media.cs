namespace Bulut_Album.Models.Entities
{
    public class Media
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime UploadDate { get; set; }
        public string? Note { get; set; }
    }


}
