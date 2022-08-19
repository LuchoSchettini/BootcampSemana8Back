using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace Store.FunctionalTests.Controllers
{
    public class BaseControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public BaseControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        ////public HttpClient GetNewClient()
        ////{
        ////    var newClient = _factory.WithWebHostBuilder(builder =>
        ////    {
        ////        _factory.CustomConfigureServices(builder);
        ////    }).CreateClient();

        ////    return newClient;
        ////}



        //Hola Luis.Sólo tienes que añadir el token en el header. Así:
        public HttpClient GetNewClient()
        {
            var newClient = _factory.WithWebHostBuilder(builder =>
            {
                _factory.CustomConfigureServices(builder);
            }).CreateClient();

            //si queremos añadir un token a al header para los tests funcionales
            newClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Constants.ADMIN_TOKEN);

            return newClient;
        }





    }
}