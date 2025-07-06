using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;

public class S3Service
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public S3Service(IConfiguration config)
    {
        var accessKey = config["AWS:AccessKey"];
        var secretKey = config["AWS:SecretKey"];
        var region = Amazon.RegionEndpoint.GetBySystemName(config["AWS:Region"]);
        _bucketName = config["AWS:BucketName"];

        _s3Client = new AmazonS3Client(accessKey, secretKey, region);
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
    {
        var uploadRequest = new TransferUtilityUploadRequest
        {
            InputStream = fileStream,
            Key = fileName,
            BucketName = _bucketName,
            ContentType = contentType,
            //CannedACL = S3CannedACL.PublicRead // opsiyonel: herkes erişebilsin mi
        };

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(uploadRequest);

        return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
    }
}
