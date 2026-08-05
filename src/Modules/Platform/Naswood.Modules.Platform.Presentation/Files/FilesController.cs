using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Abstractions;
using Naswood.BuildingBlocks.AspNetCore;
using Naswood.Modules.Platform.Application.Files;
using Naswood.Modules.Platform.Contracts.Files;
using Naswood.Modules.Platform.Presentation.Authorization;

namespace Naswood.Modules.Platform.Presentation.Files;

[ApiController]
[Authorize]
public sealed class FilesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public FilesController(IDispatcher dispatcher) => _dispatcher = dispatcher;

    [HttpGet("api/v1/files/search")]
    [RequirePermission("File.View")]
    public async Task<IActionResult> Search(
        [FromQuery] string? name,
        [FromQuery] string? category,
        [FromQuery] string? module,
        [FromQuery] string? extension,
        [FromQuery] string? relatedEntityType,
        [FromQuery] string? relatedEntityId,
        [FromQuery] string? companyId,
        [FromQuery] string? plantId,
        [FromQuery] string? status,
        [FromQuery] bool currentOnly = true,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _dispatcher.QueryAsync(
                new SearchFilesQuery(
                    name,
                    category,
                    module,
                    extension,
                    relatedEntityType,
                    relatedEntityId,
                    companyId,
                    plantId,
                    status,
                    currentOnly,
                    page,
                    pageSize),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/files/{id:guid}")]
    [RequirePermission("File.View")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new GetFileByIdQuery(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this);
    }

    [HttpGet("api/v1/files/{id:guid}/download")]
    [RequirePermission("File.Download")]
    public async Task<IActionResult> Download(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new DownloadFileQuery(id), cancellationToken)
            .ConfigureAwait(false);
        if (result.IsFailure)
        {
            return result.ToActionResult(this);
        }

        return File(result.Value.Content, result.Value.ContentType, result.Value.FileName);
    }

    [HttpGet("api/v1/files/{id:guid}/preview")]
    [RequirePermission("File.View")]
    public async Task<IActionResult> Preview(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.QueryAsync(new PreviewFileQuery(id), cancellationToken)
            .ConfigureAwait(false);
        if (result.IsFailure)
        {
            return result.ToActionResult(this);
        }

        return File(result.Value.Content, result.Value.ContentType);
    }

    [HttpPost("api/v1/files")]
    [RequirePermission("File.Upload")]
    [RequestSizeLimit(30 * 1024 * 1024)]
    public async Task<IActionResult> Upload(
        IFormFile file,
        [FromForm] string? module,
        [FromForm] string? category,
        [FromForm] string? relatedEntityType,
        [FromForm] string? relatedEntityId,
        [FromForm] string? companyId,
        [FromForm] string? plantId,
        [FromForm] string? tags,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "File is required." });
        }

        await using var stream = file.OpenReadStream();
        var result = await _dispatcher.SendAsync(
                new UploadFileCommand(
                    stream,
                    file.FileName,
                    string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                    module,
                    category,
                    relatedEntityType,
                    relatedEntityId,
                    companyId,
                    plantId,
                    ParseTags(tags)),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "File uploaded successfully.");
    }

    [HttpPost("api/v1/files/bulk-upload")]
    [RequirePermission("File.Upload")]
    [RequestSizeLimit(100 * 1024 * 1024)]
    public async Task<IActionResult> BulkUpload(
        [FromForm] List<IFormFile> files,
        [FromForm] string? module,
        [FromForm] string? category,
        [FromForm] string? relatedEntityType,
        [FromForm] string? relatedEntityId,
        [FromForm] string? companyId,
        [FromForm] string? plantId,
        [FromForm] string? tags,
        CancellationToken cancellationToken)
    {
        if (files is null || files.Count == 0)
        {
            return BadRequest(new { success = false, message = "At least one file is required." });
        }

        var commands = new List<UploadFileCommand>();
        var streams = new List<Stream>();
        try
        {
            foreach (var file in files)
            {
                var stream = file.OpenReadStream();
                streams.Add(stream);
                commands.Add(new UploadFileCommand(
                    stream,
                    file.FileName,
                    string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
                    module,
                    category,
                    relatedEntityType,
                    relatedEntityId,
                    companyId,
                    plantId,
                    ParseTags(tags)));
            }

            var result = await _dispatcher.SendAsync(new BulkUploadFilesCommand(commands), cancellationToken)
                .ConfigureAwait(false);
            return result.ToActionResult(this, successMessage: "Bulk upload completed.");
        }
        finally
        {
            foreach (var stream in streams)
            {
                await stream.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    [HttpPost("api/v1/files/{id:guid}/version")]
    [RequirePermission("File.Version")]
    [RequestSizeLimit(30 * 1024 * 1024)]
    public async Task<IActionResult> CreateVersion(
        Guid id,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { success = false, message = "File is required." });
        }

        await using var stream = file.OpenReadStream();
        var result = await _dispatcher.SendAsync(
                new CreateFileVersionCommand(
                    id,
                    stream,
                    file.FileName,
                    string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "File version created.");
    }

    [HttpPut("api/v1/files/{id:guid}")]
    [RequirePermission("File.Upload")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateFileRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(
                new UpdateFileCommand(
                    id,
                    request.Name,
                    request.Category,
                    request.RelatedEntityType,
                    request.RelatedEntityId,
                    request.Tags),
                cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "File updated successfully.");
    }

    [HttpDelete("api/v1/files/{id:guid}")]
    [RequirePermission("File.Delete")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.SendAsync(new DeleteFileCommand(id), cancellationToken)
            .ConfigureAwait(false);
        return result.ToActionResult(this, successMessage: "File deleted successfully.");
    }

    private static IReadOnlyList<string>? ParseTags(string? tags)
    {
        if (string.IsNullOrWhiteSpace(tags))
        {
            return null;
        }

        return tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }
}
