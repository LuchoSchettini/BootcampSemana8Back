namespace Store.ApplicationCore.ResponsesApi.Errors;
public class ApiErrorNoCatched : ApiResponseError
{
    public ApiErrorNoCatched(int statusCode, string message = null, string details = null)
                    : base(statusCode, message)
    {
        Details = details;
    }

    public string Details { get; set; }
}
