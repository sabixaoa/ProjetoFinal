using Microsoft.WindowsAzure.Storage.Blob;

namespace LambdaForums.Data
{
    public interface IUpload
    {
        CloudBlobContainer GetBlobContainer(string connectionString, string containerName);
    }
}
