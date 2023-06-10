using Azure.Storage.Blobs;
using System.IO;

namespace TastyCook.RecipesAPI.Services;

public class FileService
{
    private readonly string BlobConnectionString = "DefaultEndpointsProtocol=https;AccountName=tastycookfilestorage;AccountKey=OKn6SfOQmsCwdrPTP3vaolcTk2Flq3/HjhQdT7CNG/XNo4FMzMoqUiJg/MVh6gBLq/1k9uKXx1eI+AStGHxcgg==;EndpointSuffix=core.windows.net";
    private readonly string BlobContainerName = "recipeimages";

    public async Task UploadFile(IFormFile data, string fileName)
    {
        var container = new BlobContainerClient(BlobConnectionString, BlobContainerName);
        var blob = container.GetBlobClient(fileName);

        using (var memoryStream = new MemoryStream())
        {
            await data.CopyToAsync(memoryStream);

            memoryStream.Position = 0;
            if (memoryStream.Length < 209715200)
            {
                //var file = memoryStream.ToArray();
                await blob.UploadAsync(memoryStream);
            }
            else
            {
                throw new Exception("File is too big");
            }
        }
    }
}