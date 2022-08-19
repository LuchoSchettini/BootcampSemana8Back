namespace Store.ApplicationCore.ResponsesApi;
public abstract class ApiResponseAbstract
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public bool IsSuccessful { get; set; }


    public ApiResponseAbstract(int statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessage(statusCode);
        IsSuccessful = statusCode < 300;
    }

    private string GetDefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            200 => "Respuesta OK para peticiones correctas.",
            201 => "La petición ha sido completada y ha resultado en la creación de un nuevo recurso",
            204 => "La petición se ha completado con éxito pero su respuesta no tiene ningún contenido.",

            400 => "Has realizado una petición incorrecta.",
            401 => "Usuario no autorizado.",
            403 => "La solicitud fue legal, pero no tiene los privilegios para realizarla.",
            404 => "El recurso que has intentado solicitar no existe.",
            405 => "Este método HTTP no está permitido en el servidor.",
            409 => "Conflict response status.",
            415 => "Unsupported Media Type.",
            500 => "Error en el servidor. Comunícate con el administrador.",
            _ => $"Sin Información del Codigo Http {statusCode}."
        };
    }
}
