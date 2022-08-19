using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Store.ApplicationCore;
using Store.Infrastructure;
using Store.Infrastructure.Persistence.Seeds;
using System;
using Store.WebApi;
using AspNetCoreRateLimit;
using System.Reflection;
using Store.Infrastructure.Persistence.Contexts.Ctx01;
using Store.Infrastructure.Persistence.Seeds.Ctx01;
using Serilog;
using Store.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);


//    para usar Serilog:
var loggerSerilog = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .CreateLogger();

//builder.Logging.ClearProviders();    //para que no utilice ningun tipo de log (esto seria lo ideal ir viendolo por ahora esta comentado)

//   lo agregamos al sistema de log integrado en Net core, para que utilice Serilog.
builder.Logging.AddSerilog(loggerSerilog);



//////////// Add services to the container.
//////////builder.Services.AddInjectionCore();
//////////await builder.Services.AddInjectionInfrastructure(builder.Configuration);
//////////builder.Services.AddInjectionApi(builder.Configuration);






//SI QUEREMOS TAMBIEN EL FORMATO XML
////   Si quisieramos que nuestra Api Admita tambien el formato Xml
////   Hay que comentar la linea de ABAJO y y descomentar esta seccion de codigo.
////   OJO si usamos estos, si hay algun atributo en los controladores -> [Produces("application/json")] hay que comentarlo.
// builder.Services.AddControllers(options =>
// {
//    options.RespectBrowserAcceptHeader = true;
//    //Y que si el formato no es aceptado retorne un codigo http de error -> 406 Not Acceptable
//    options.ReturnHttpNotAcceptable = true;
// }).AddXmlSerializerFormatters(); //Formato Xml

builder.Services.AddControllers();


//ADD SERVICES TO CONTAINER  (de cada proyecto tengo una uno):
//Como la modificacion del servicio ApiBehaviorOptions tenemos que hacer justo despues de AddControllers
//ya ponemos todos despues de AddControllers
builder.Services.AddInjectionCore();
await builder.Services.AddInjectionInfrastructure(builder.Configuration);
//como en una de las injeciones de la Api modificamos el comportamiento del servicio ApiBehaviorOptions
//y este (tiene que ir justo despues de injectar el servcio AddControllers)
builder.Services.AddInjectionApi(builder.Configuration);







// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();






//Este Middleware -> app.UseMiddleware<ApiErrorNoCatchedMiddleware>()
//Pilla cualquier error que no se halla cogido por un try cach menos los de Error de validacion de entidades.
//Este Middleware le pone a todos los errores que pilla (error 500)

//Este Middleware -> app.UseMiddleware<ApiErrorNoCatchedMiddleware>()
//Cualquier Excepcion que no se halla cogido por un try cach OJO -> (menos los de validacion de entidades, por error en la Url, Por permisos)  
//  En entornos distintos Development -> el error sera con mensaje y detalles,
//  en caso de Development -> el mensaje sera generico y sin detalles.
//  Siempre se escribiera en el log como un LogError (como un error 500 con con mensaje y detalles).
//Solo hay que lanzar mis excepciones personalizadas, las demas no (ya que las pillara el Error no catched)

//Informativo: Los errores de validacion de entidades los pilla un servicio antes de llegar a este Middleware
//    Desde este servicio ApiBehaviorOptions ya registrado en la injeccion de dependencias de la clase program por el framework,
//    Por eso esos errores no llegan hasta aqui.
//    Por cierto, Ya hemos modificado el servicio de ApiBehaviorOptions para dar una respuesta formateada.
//Informativo: los errores por la Url, por permisos de los pilla el siguiente Middleware -> app.UseStatusCodePagesWithReExecute("/errores/{0}");
//   ya aqui no pilla este tipo de error.
app.UseMiddleware<ApiErrorNoCatchedMiddleware>();



//Este Middleware -> app.UseStatusCodePagesWithReExecute("/errores/{0}");
//Es para pillar los errores por rutas http que no son validas, para pillar accesos ilegales por permisos como el 401, 403:
//Formatearemos las respuestas de los errores de rutas http que no son validas, accesos ilegales por permisos.
//  para mostrar mejor los errores cuando no encuetra el EndPoint con Ruta en la url dada
//  o porque se da mal el verbo y no encuentra el EndPoint, o por falta de permisos del recurso.
//Este Middleware UseStatusCodePagesWithReExecute se utiliza para generar paginas de error personalizadas cuando 
//ocurre un error en el servidor.
////Vamos a crear un controlador llamado errores y al cual se le tiene que pasar un codigo http de error (este formateara el resultado del error)
app.UseStatusCodePagesWithReExecute("/errores/{0}");



//para que funcione el  Ip Limiting in time
app.UseIpRateLimiting();



// Configure the HTTP request pipeline.
////Oreden del ASPNETCORE_ENVIRONMENT: 
//  app.Environment.IsDevelopment()
//  app.Environment.IsStaging()
//  app.Environment.IsProduction()
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



////Migraciones Automaticas y Seeds
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
//    try
//    {
//        var context = services.GetRequiredService<StoreContext>();
//        //await context.Database.MigrateAsync();    //-> Ahora mismo lo hago desde Store.Infrastructure DependencyInjection
//        await StoreContextSeed.SeedAsync(context, loggerFactory);

//        //SI HAY MAS CONTEXTOS TAMBIEN PUEDO USARLOS DESDE AQUI:
//        //var context2 = services.GetRequiredService<StoreContext2>();
//        //await context2.Database.MigrateAsync();
//        //await StoreContext2Seed.SeedAsync(context2, loggerFactory);
//    }
//    catch (Exception ex)
//    {
//        var logger = loggerFactory.CreateLogger<Program>();
//        logger.LogError(ex, "Ocurrió un error durante la migración");
//    }
//}


//Migraciones Automaticas y Seeds:
//esto se tiene que hacer aqui ya que esto solo corre cuando se inicia la aplicacion
//y si hay alguna migracion pendiente la corre, igualmente si no tiene datos corro los Seeeds
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    try
    {
        //SI HAY MAS CONTEXTOS TAMBIEN PUEDO USARLOS DESDE AQUI:
        var dbContext = services.GetRequiredService<Ctx01_Store>();
        await dbContext.Database.MigrateAsync();

        //añado datos para para hacer pruebas:
        await DataSeedDesdeJson.DataSeedAsync(dbContext, loggerFactory);


    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "Ocurrió un error durante la migración y Seeds de Datos");
    }
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");




//nuevo
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program{}
