using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Minio;
using Minio.Exceptions;
using PayOut_Aulac_FPT.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayOut_Aulac_FPT.Infrastructure.Services
{
    public class MinioService : IMinioService
    {
        private readonly IConfiguration configuration;
        private string bucketName;
        public MinioService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.bucketName = configuration["Minio:BucketName"];
        }

        private MinioClient CreateClient()
        {
            string endpoint = $"{configuration["Minio:Domain"]}:{configuration["Minio:Port"]}";
            string accessKey = configuration["Minio:AccessKey"];
            string secretKey = configuration["Minio:SecretKey"];
            string region = configuration["Minio:Region"];
            try
            {
                return new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .WithRegion(region)
                    .WithSSL()
                    .Build();
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Create minio client error: {0}", ex.Message));
            }
        }

        public async Task<string> UploadFile(string path, string contentType, string extension, string name, string? parent)
        {
            try
            {
                var client = CreateClient();
                string fileName = $"{parent}/{name}{extension}";
                var puObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(fileName)
                    .WithFileName(path)
                    .WithContentType(contentType);
                await client.PutObjectAsync(puObjectArgs).ConfigureAwait(false);
                client.Dispose();
                return $"upload/{fileName}";
            }
            catch (MinioException e)
            {
                throw new Exception(String.Format("File Upload Error: {0}", e.Message));
            }
        }

        public string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2; //b tells how many times a new letter has been added. It's 2 right now because the first two letters are already in the name.
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }
    }
}
