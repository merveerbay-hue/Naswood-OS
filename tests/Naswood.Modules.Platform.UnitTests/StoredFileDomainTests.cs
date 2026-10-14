using Naswood.Modules.Platform.Application.Files;
using Naswood.Modules.Platform.Domain.Files;

namespace Naswood.Modules.Platform.UnitTests;

public class StoredFileDomainTests
{
    [Fact]
    public void Create_sets_metadata_and_raises_uploaded_event()
    {
        var file = StoredFile.Create(
            "report.pdf",
            "application/pdf",
            1024,
            "abc",
            "COMP-001/Platform/2026/08/id.pdf",
            "General",
            "Platform",
            null,
            null,
            "COMP-001",
            "PLANT-001",
            ["docs"],
            Guid.NewGuid());

        Assert.Equal("report", file.Name);
        Assert.Equal(".pdf", file.Extension);
        Assert.True(file.IsCurrentVersion);
        Assert.Equal(FileStatus.Available, file.Status);
        Assert.Contains(file.DomainEvents, e => e is FileUploaded);
        Assert.True(file.IsPreviewable());
    }

    [Fact]
    public void SoftDelete_marks_deleted()
    {
        var file = StoredFile.Create(
            "note.txt",
            "text/plain",
            10,
            null,
            "key.txt",
            "General",
            "Platform",
            null,
            null,
            "COMP-001",
            null,
            null,
            null);

        file.ClearDomainEvents();
        var result = file.SoftDelete(null, DateTimeOffset.UtcNow);
        Assert.True(result.IsSuccess);
        Assert.Equal(FileStatus.Deleted, file.Status);
        Assert.False(file.IsCurrentVersion);
    }

    [Fact]
    public void FileUploadValidator_rejects_disallowed_extension()
    {
        var options = new FileUploadOptions();
        var result = FileUploadValidator.Validate("malware.exe", "application/octet-stream", 100, options);
        Assert.True(result.IsFailure);
        Assert.Equal("FILE-002", result.Error!.Code);
    }
}
