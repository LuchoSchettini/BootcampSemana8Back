using Store.ApplicationCore.ResponsesApi;
using System.Collections.Generic;

namespace Store.ApplicationCore.ResponsesApi.Errors;
public class ApiErrorValidationEntity : ApiResponseError
{
    //Como es un codigo de EntityDataValidation yo se que es un codigo 400
    //EntityDataValidation
    public ApiErrorValidationEntity() : base(400)
    {

    }

    public IEnumerable<string> ErrorsValidationEntity { get; set; }

}
