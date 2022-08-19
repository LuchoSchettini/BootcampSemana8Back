//using API.Helpers;
//using API.Services;
using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Core.Entities;
//using Core.Interfaces;
//using Infrastructure.Repositories;
//using Infrastructure.UnitOfWork;
//using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.IdentityModel.Tokens;
using Store.ApplicationCore.Helpers.Auth;
using Store.ApplicationCore.ResponsesApi.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.WebApi;
public static class DependencyInjectionApi
{

    public static IServiceCollection AddInjectionApi(this IServiceCollection services, IConfiguration configuration)
    {

        //**********************ConfigureRateLimitiong************************>>>
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()   //WithOrigins("https://dominio.com")
                .AllowAnyMethod()          //WithMethods("GET","POST")
                .AllowAnyHeader());        //WithHeaders("accept","content-type")
        });
        //**********************ConfigureRateLimitiong************************<<<



        //**********************ConfigureRateLimitiong************************>>>
        services.AddMemoryCache();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddInMemoryRateLimiting();

        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.HttpStatusCode = 429;
            options.RealIpHeader = "X-Real-IP";
            options.GeneralRules = new List<RateLimitRule>
                {
                new RateLimitRule
                {
                    Endpoint ="*",
                    //Period = "1s",
                    //Limit = 2    
                    Period = "10s",
                    Limit =2
                }
                };
            //Tambien añade un encabezados:
            // X-Reate-Limit 10s    
            // X-Reate-Limit-Remmaining 2
            // X-Reate-Limit-Reset (FechaHora maxima del tiempo) 10s    
        });
        //**********************ConfigureRateLimitiong************************<<<



        //**********************ConfigureApiVersioning************************>>>
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);

            //si no escogen una version cogera la de por defecto que es la de la linea de arriba
            options.AssumeDefaultVersionWhenUnspecified = true;

            //options.ApiVersionReader = new QueryStringApiVersionReader("ver");
            //options.ApiVersionReader = new HeaderApiVersionReader("X-Version");

            //vamos a poner 2 formas de poder enviar la version del EndPoint 
            //Ojo Escoje solo usar 1 forma para enviar la version de las 2 que tenemos.
            //(si pones las dos fromas y cada una con un valor distinto dara error)
            options.ApiVersionReader = ApiVersionReader.Combine(
            //Establecemos la opcion de poder enviar la version por parameto en la Url
            //Ejemplo -> ... ?ver=1.1 
            new QueryStringApiVersionReader("ver"),
            //Establecemos la opcion de poder enviar la version en el encabezado (headers)
            //Ejemplo -> Key: X-Version    Value: 1.1 
            new HeaderApiVersionReader("X-Version")
            );

            //Indica que versiones soporta en un encabezado de la URL llamado api-supported-versions
            options.ReportApiVersions = true;
        });
        //**********************ConfigureApiVersioning************************<<<




        //**********************Recuperamos seccion de AppSettings************>>>
        //Configuration from AppSettings
        //Agregamos la seccion JWT de nuestro appsettings.json a nuestra clase JWT.
        services.Configure<JWT>(configuration.GetSection("JWT"));
        //**********************Recuperamos seccion de AppSettings************<<<



        //**********************AddAuthentication******************************>>>
        //Adding Athentication - JWT
        services.AddAuthentication(
                //definimos el tipo de autentificacion (aqui utilizamos JwtBearer:
                options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                //configuramos la informacion del token:
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;  //definimos si necitamos un conexion https (solo en desarrollo la no ponemos a false)
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = configuration["JWT:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
                    };
                });
        //**********************AddAuthentication******************************<<<



        //// la modificacion del servicio ApiBehaviorOptions tenemos que hacer justo despues de xxxxxxx

        //**********************Modificar respuesta InvalidModelStateResponseFactory**>>>  
        //Desde este servicio ApiBehaviorOptions ya registrado en la injeccion de dependencias de la clase program por el framework
        //configuramos la respuesta de las entidades cuando no pasan sus validaciones, la Opcion que se modifica
        //es la InvalidModelStateResponseFactory.
        //Lo que hacemos es formatear la restpuesta para que queede asi:
        ////// AHORA DEVUELVE LO DEVUELVE ASI>>>>>>>>
        //{
        //"ErrorsValidationEntity": [
        //  "The Name field is required.",
        //  "The Description field is required."
        //],
        //"statusCode": 400,
        //"message": "Has realizado una petición incorrecta."
        //}
        ////// AHORA DEVUELVE LO DEVUELVE ASI<<<<<<<<
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = actionContext =>
            {
                //aplanamos la respuesta SelectMay los (keyValuePair.Value.Errors)
                //y devolvemos los ErrorMessage (prop.ErrorMessage, que son los mensajes de error)
                //Ojo ya no se envian los u.Key que contiene los nombre de los campos, asi que lo mensajes tienen que describir bien el campo del error.

                //COMO ESTABA ANTES, PERO SOLO DABA EL VALUE (ASI QUE LO CAMBIE)
                //***************************>>> 
                ////KeyValuePair<string,ModelStateEntity>
                //// key --> tiene el nombre del campo   y   value --> ModelStateEntity (que tiene la lista de errores del campo)

                //var errors = actionContext.ModelState.Where(keyValuePair => keyValuePair.Value.Errors.Count > 0) //los que tengan algun error
                //                                    .SelectMany(keyValuePair => keyValuePair.Value.Errors) //utilizamos la parte value del (KeyValuePair) para aplanar
                //                                    .Select(prop => prop.ErrorMessage).ToArray(); //su propiedad ErrorMesage que es el memaje de error.
                //                                                                                  //con .ToArry() devolvemos un arraray del tipo que devuelve el aplanamiento que hicimos, en este caso un array de string

                //DEVUELVE UN ARRAY DE STRING COMO ESTE (SOLO EL VALUE):
                //The Name field is required.
                //The Description field is required.
                //***************************<<<






                //Esto sería más simple como una comprensión de consulta:
                //***************************>>> 
                IEnumerable<string> errors = from keyValuePair in actionContext.ModelState
                               from e in keyValuePair.Value.Errors
                               select keyValuePair.Key + ": " + e.ErrorMessage;

                ////EJM. DEVUELVE UN IENUMERABLE DE STRING COMO ESTE (KEY: VALUE):
                // Name: The Name field is required.
                // Description: The Description field is required.
                //***************************<<<

                ////Esto sería como lo de arriba (mismo resultado) sin forma de consulta (un poco mas complicado):
                ////***************************>>> 
                // IEnumerable<string> errors = actionContext.ModelState.Keys.SelectMany(
                //    key => actionContext.ModelState[key].Errors.Select(x => key + ": " + x.ErrorMessage));

                ////EJEM. DEVUELVE UN IENUMERABLE DE STRING COMO ESTE (KEY: VALUE):
                // Name: The Name field is required.
                // Description: The Description field is required.
                //***************************<<<



                var errorResponse = new ApiErrorValidationEntity()
                {
                    ErrorsValidationEntity = errors
                };

                return new BadRequestObjectResult(errorResponse);
            };
            //**********************Modificar respuesta InvalidModelStateResponseFactory**<<<
        });






        return services;
    }






    //public static void ConfigureApiVersioning(this IServiceCollection services)
    //{
    //    services.AddApiVersioning(options =>
    //    {
    //        options.DefaultApiVersion = new ApiVersion(1, 0);
    //        options.AssumeDefaultVersionWhenUnspecified = true;
    //        //options.ApiVersionReader = new QueryStringApiVersionReader("ver");
    //        //options.ApiVersionReader = new HeaderApiVersionReader("X-Version");
    //        options.ApiVersionReader = ApiVersionReader.Combine(
    //            new QueryStringApiVersionReader("ver"),
    //            new HeaderApiVersionReader("X-Version"));
    //        options.ReportApiVersions = true;
    //    });
    //}


    //public static void ConfigureCors(this IServiceCollection services) =>
    //            services.AddCors(options =>
    //            {
    //                options.AddPolicy("CorsPolicy", builder =>
    //                    builder.AllowAnyOrigin()   //WithOrigins("https://dominio.com")
    //                    .AllowAnyMethod()          //WithMethods("GET","POST")
    //                    .AllowAnyHeader());        //WithHeaders("accept","content-type")
    //            });




    //public static void AddAplicacionServices(this IServiceCollection services)
    //{
    //    //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    //    //services.AddScoped<IProductoRepository, ProductoRepository>();
    //    //services.AddScoped<IMarcaRepository, MarcaRepository>();
    //    //services.AddScoped<ICategoriaRepository, CategoriaRepository>();
    //    services.AddScoped<IPasswordHasher<Usuario>, PasswordHasher<Usuario>>();
    //    services.AddScoped<IUserService, UserService>();
    //    services.AddScoped<IUnitOfWork, UnitOfWork>();
    //}














    //public static void AddJwt(this IServiceCollection services, IConfiguration configuration)
    //{
    //    //Configuration from AppSettings
    //    services.Configure<JWT>(configuration.GetSection("JWT"));

    //    //Adding Athentication - JWT
    //    services.AddAuthentication(options =>
    //    {
    //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    //    })
    //        .AddJwtBearer(o =>
    //        {
    //            o.RequireHttpsMetadata = false;
    //            o.SaveToken = false;
    //            o.TokenValidationParameters = new TokenValidationParameters
    //            {
    //                ValidateIssuerSigningKey = true,
    //                ValidateIssuer = true,
    //                ValidateAudience = true,
    //                ValidateLifetime = true,
    //                ClockSkew = TimeSpan.Zero,
    //                ValidIssuer = configuration["JWT:Issuer"],
    //                ValidAudience = configuration["JWT:Audience"],
    //                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]))
    //            };
    //        });
    //}



}
