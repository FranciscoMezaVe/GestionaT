using FluentResults;
using GestionaT.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace GestionaT.Api.Common.Result
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result, HttpStatusCodeResolver statusResolver)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            var errors = result.Errors.Select(e => new ApiError
            {
                Code = e.Metadata.TryGetValue("ErrorCode", out var code) ? code?.ToString() ?? "UNKNOWN_ERROR" : "UNKNOWN_ERROR",
                Message = e.Message,
                Detail = e.Metadata.TryGetValue("Detail", out var detail) ? detail?.ToString() : null,
                Target = e.Metadata.TryGetValue("Field", out var field) ? field?.ToString() : null
            }).ToList();

            var httpStatus = statusResolver.Resolve(errors.First().Code);

            return new ObjectResult(errors) { StatusCode = httpStatus };
        }

        public static IActionResult ToActionResult(this FluentResults.Result result, HttpStatusCodeResolver statusResolver)
        {
            if (result.IsSuccess)
                return new NoContentResult();

            var errors = result.Errors.Select(e => new ApiError
            {
                Code = e.Metadata.TryGetValue("ErrorCode", out var code) ? code?.ToString() ?? "UNKNOWN_ERROR" : "UNKNOWN_ERROR",
                Message = e.Message,
                Detail = e.Metadata.TryGetValue("Detail", out var detail) ? detail?.ToString() : null,
                Target = e.Metadata.TryGetValue("Field", out var field) ? field?.ToString() : null
            }).ToList();

            var httpStatus = statusResolver.Resolve(errors.First().Code);
            return new ObjectResult(errors) { StatusCode = httpStatus };
        }

        public static IActionResult ToFileResult(this Result<byte[]> result, string contentType, string fileName, HttpStatusCodeResolver statusResolver, ILogger logger)
        {
            if (result.IsSuccess)
                return new FileContentResult(result.Value, contentType) { FileDownloadName = fileName };

            var errors = result.Errors.Select(e => new ApiError
            {
                Code = e.Metadata.TryGetValue("ErrorCode", out var code) ? code?.ToString() ?? "UNKNOWN_ERROR" : "UNKNOWN_ERROR",
                Message = e.Message,
                Detail = e.Metadata.TryGetValue("Detail", out var detail) ? detail?.ToString() : null,
                Target = e.Metadata.TryGetValue("Field", out var field) ? field?.ToString() : null
            }).ToList();

            var statusCode = statusResolver.Resolve(errors.First().Code);
            logger.LogWarning("Error al generar archivo: {Mensaje}", errors.First().Message);
            return new ObjectResult(errors) { StatusCode = statusCode };
        }
    }

}
