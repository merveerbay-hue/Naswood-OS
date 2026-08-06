using Naswood.BuildingBlocks.Application.Storage;
using Naswood.BuildingBlocks.Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace Naswood.BuildingBlocks.UnitTests;

public class LocalFileStorageProviderTests
{
    [Fact]
    public async Task Upload_download_exists_and_delete_roundtrip()
    {
        var root = Path.Combine(Path.GetTempPath(), "naswood-file-storage-tests", Guid.NewGuid().ToString("N"));
        var options = Options.Create(new FileStorageOptions
        {
            Provider = "Local",
            Local = new LocalFileStorageOptions { RootPath = root }
        });

        IFileStorage storage = new LocalFileStorageProvider(options);

        await using var uploadStream = new MemoryStream("hello-naswood"u8.ToArray());
        var uploaded = await storage.UploadAsync(
            new FileStorageUploadRequest(uploadStream, "note.txt", "text/plain", Folder: "docs"));

        Assert.True(await storage.ExistsAsync(uploaded.StorageKey));

        var download = await storage.DownloadAsync(uploaded.StorageKey);
        await using (download.Content)
        {
            using var reader = new StreamReader(download.Content);
            var text = await reader.ReadToEndAsync();
            Assert.Equal("hello-naswood", text);
        }

        await storage.DeleteAsync(uploaded.StorageKey);
        Assert.False(await storage.ExistsAsync(uploaded.StorageKey));
    }
}
