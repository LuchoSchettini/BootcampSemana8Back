namespace Store.ApplicationCore.ResponsesApi
{
    public class ApiResponseError : ApiResponseAbstract
    {
        public ApiResponseError(int statusCode, string message = null)
                : base(statusCode, message)
        {
            //Details = details;
        }

    }
}
