using Newtonsoft.Json;
using System.Collections.Generic;

namespace Store.FunctionalTests.Models
{

    public class xxxxBadRequestModel
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("errors")]
        public Dictionary<string, string[]> Errors { get; set; }
    }




    //// ASI QUEDA FORMATEADA LA RESPUESTA:
    //{ 
    //    "errors":["The Name field is required."],
    //    "statusCode":400,
    //    "message":"Has realizado una petición incorrecta."
    //}
    public class xxxxBadRequestFormatedModel
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("errorsValidationEntity")]
        public List<string> ErrorsValidationEntity { get; set; }

        //"isSuccessful":false
        [JsonProperty("isSuccessful")]
        public bool IsSuccessful { get; set; }
    }


}


