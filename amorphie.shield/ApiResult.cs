using amorphie.core.Base;

namespace amorphie.shield
{
    public static class ApiResult
    {
        public static IResult CreateResult(Response response)
        {
            if (response.Result.Status == "Success")
            {
                if (response.Result.Message == "Not Modified")
                {
                    return Results.StatusCode(StatusCodes.Status304NotModified);
                }
                return Results.Ok(response.Result.Message);
            }
            else
            {
                return Results.Problem(response.Result.Message);
            }
        }
        public static IResult CreateResult<T>(Response<T> response) where T : class
        {
            if (response.Result.Status == "Success")
            {
                if (response.Result.Message == "Not Modified")
                {
                    return Results.StatusCode(StatusCodes.Status304NotModified);
                }
                return Results.Ok(response.Data);
            }
            else
            {
                return Results.Problem(response.Result.Message);
            }
        }
    }
}
