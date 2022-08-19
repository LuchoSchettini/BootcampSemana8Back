using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.ApplicationCore.ResponsesApi;
using System.Net.Mime;

namespace Store.WebApi.Controllers
{
    //Este controlador sirve para formatear los siguentes errores: 
    //A este cotrolador lo llama el Middleware  -->   app.UseStatusCodePagesWithReExecute("/errores/{0}");

    //- Errores por rutas http que no son validas,
    //- para pillar accesos ilegales por permisos como el 401, 403:
    //Formatearemos las respuestas de los errores de rutas http que no son validas, accesos ilegales por permisos.
    //Sirve para mostrar mejor los errores cuando no encuetra el EndPoint con Ruta en la url dada
    //  o porque se da mal el verbo y no encuentra el EndPoint, o por falta de permisos del recurso.

    [Route("errores/{code}")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces("application/json")]
    public class ErroresController : ControllerBase
    {

        public IActionResult Error(int code)
        {
            return new ObjectResult(new ApiResponseError(code));
        }

    }
}
