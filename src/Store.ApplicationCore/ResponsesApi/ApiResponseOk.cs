namespace Store.ApplicationCore.ResponsesApi
{
    public class ApiResponseOk : ApiResponseAbstract
    {
        public object Response { get; set; }
        public ApiResponseOk(int statusCode, object response = null, string message = null)
        : base(statusCode, message)
        {
            Response = response;
        }
    }
}
