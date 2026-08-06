using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Naswood.BuildingBlocks.Application.Contracts;
using Naswood.BuildingBlocks.Domain;

namespace Naswood.BuildingBlocks.AspNetCore;

public static class ResultExtensions
{
    public static IActionResult ToActionResult<T>(
        this Result<T> result,
        ControllerBase controller,
        string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(ApiResponse<T>.Ok(result.Value, successMessage));
        }

        return ToErrorResult(result.Error!, controller);
    }

    public static IActionResult ToActionResult(
        this Result result,
        ControllerBase controller,
        string? successMessage = null)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(ApiResponse<object?>.Ok(null, successMessage));
        }

        return ToErrorResult(result.Error!, controller);
    }

    private static IActionResult ToErrorResult(Error error, ControllerBase controller)
    {
        var statusCode = error.Category switch
        {
            "Validation" => StatusCodes.Status400BadRequest,
            "NotFound" => StatusCodes.Status404NotFound,
            "Conflict" => StatusCodes.Status409Conflict,
            "Unauthorized" => StatusCodes.Status401Unauthorized,
            "Forbidden" => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        var response = new ApiErrorResponse
        {
            Message = error.Message,
            Errors =
            [
                new ApiErrorItem
                {
                    Code = error.Code,
                    Category = error.Category,
                    Message = error.Message,
                    Details = new { }
                }
            ],
            Metadata = new ApiErrorMetadata
            {
                CorrelationId = controller.HttpContext.TraceIdentifier,
                Timestamp = DateTimeOffset.UtcNow
            }
        };

        return controller.StatusCode(statusCode, response);
    }
}
