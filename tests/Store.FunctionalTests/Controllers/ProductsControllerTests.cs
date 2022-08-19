using Newtonsoft.Json;
using Store.ApplicationCore.DTOs.Ctx01;
//using Store.ApplicationCore.Helpers.Utils;
using Store.ApplicationCore.Helpers.Pager;
using Store.ApplicationCore.Helpers.Utils;
using Store.ApplicationCore.ResponsesApi.Errors;
using Store.FunctionalTests.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Store.FunctionalTests.Controllers
{
    public class ProductsControllerTests : BaseControllerTests
    {
        public ProductsControllerTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        //El nombre de la clase de pruebas:
        //**********************************************
        //Primera parte del nombre ->el nombre de la clase a probar 
        //Segunda parte del nombre ->la palabra "Tests" 

        //Por ejemplo un nombre de clase de pruebas:
        //ProductsControllerTests
        //**********************************************


        //El nombre del metodo de prueba:
        //**********************************************
        //Primera parte del nombre ->el nombre del metodo a probar 
        //Segunda parte del nombre ->el escenario en el que se va a probar (esta parte se puede obviar cuando el escenario es el mismo para todos los casos)
        //Tercera parte del nombre ->es el resultado que yo espero.

        //Por ejemplo nombres de metodos de Tests (segun lo explicado antes)
        //**********************************************
        //GetById_ProductExists_ReturnsCorrectProduct()
        //CreateProduct_InvalidData_ReturnsErrors()
        //**********************************************


        // Las 3 partes del codigo del metodo de prueba:
        //**********************************************
        //arange       (lo que se va a probar)
        //act          (el escenario en el que se va a probar)
        //assert       (el resultado que yo espero)
        //**********************************************






        [Fact]
        public async Task GetAll_ReturnsAllRecords()
        {
            //arange       (lo que se va a probar)
            var client = this.GetNewClient();

            //act          (el escenario en el que se va a probar)
            var response = await client.GetAsync("/api/Products");
            //esto no es obligatorio, pero sirve para que lance un excepcion en el caso de que 
            //algo falle, daria error cualquier codigo que no sea un 200, como por ejemplo la validacion
            //de la entidad del dto que es un BadRequest (400) -> (asi que es mejor ponerlo).
            //OSEA si no me tiene que devolver un estado que no sea 200 se debe comentar esta linea.
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ProductResponse>>(stringResponse).ToList();
            //me devuelve la palabra del statuscode por ejemplo ->
            //"OK" que es un 200
            //"NotFound"...
            var statusCode = response.StatusCode.ToString();

            //assert        (el resultado que yo espero)
            Assert.Equal("OK", statusCode);
            Assert.True(result.Count == 10, "Debe haber 10 registros");
        }



        //Paginando con los 10 reg de las pruebas :
        //Cuando el Serch esta vacio no lo pone                 -> (no hace filtro de Serch)
        //El Serch lo podemos poner vacio como se ve  (Search=) -> (no hace filtro de Serch)
        //http://localhost:5000/api/Products/GetAllFilters?Search=&PageSize=4&PageIndex=1   //devuelve 4 reg.  
        //http://localhost:5000/api/Products/GetAllFilters?Search=&PageSize=4&PageIndex=2   //devuelve 4 reg.
        //http://localhost:5000/api/Products/GetAllFilters?Search=&PageSize=4&PageIndex=3   //devuelve 2 reg.

        //PageSize=5 , PageIndex=1  y  Search=""  es lo predeternidado si no ponermos nada
        //osea que con que enviamos el parametro de Search es suficiente a menos que quieras algun valor de parametro predeterminado
        //Filtrando
        //http://localhost:5000/api/Products/GetAllFilters?PageSize=5&PageIndex=1&Search=Product%201    //devuelve 2 reg.      
        //http://localhost:5000/api/Products/GetAllFilters?PageSize=5&PageIndex=1&Search=Product%2010   //devuelve 1 reg.

        [Theory]
        [InlineData("", 4, 1, 4)]
        [InlineData("", 4, 2, 4)]
        [InlineData("", 4, 3, 2)]
        [InlineData("Product 1", 5, 1, 2)]
        [InlineData("Product 10", 5, 1, 1)]
        public async Task GetAllFilters_CorrectData_ReturnsAllRegInPagers(
             string StrDeBusqueda, int PageSize, int PageIndex, int TotalRegInList)
        {
            var client = this.GetNewClient();

            //Error (la lista llega vacia por lo que es incorrecto, mirarlo mañana)
            var response1 = await client.GetAsync($"/api/Products/GetAllFilters?Search={StrDeBusqueda}&PageSize={PageSize}&PageIndex={PageIndex}");

            response1.EnsureSuccessStatusCode(); //como esta en la serie de 200 si va.

            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            var result1 = JsonConvert.DeserializeObject<PagerResponse<ProductResponse>>(stringResponse1);
            var statusCode1 = response1.StatusCode.ToString();

            Assert.Equal("OK", statusCode1);
            Assert.Equal(PageIndex, result1.PageIndex);
            Assert.Equal(PageSize, result1.PageSize);
            Assert.Equal(StrDeBusqueda.ToLower(), result1.Search); //cuando lo recibe del Json esta en minuscula por lo que los comparo en minuscula.


            Assert.Equal(TotalRegInList, result1.Registers.Count());
        }



        [Fact]
        public async Task GetById_ProductExists_ReturnsCorrectProduct()
        {
            var productId = 5;
            var client = this.GetNewClient();

            var response = await client.GetAsync($"/api/Products/{productId}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            ////////var result = JsonConvert.DeserializeObject<SingleProductResponse>(stringResponse);
            var result = JsonConvert.DeserializeObject<ProductResponse>(stringResponse);
            var statusCode = response.StatusCode.ToString();

            Assert.Equal("OK", statusCode);
            Assert.Equal(productId, result.Id);
            Assert.NotNull(result.Name);
            Assert.True(result.Price >= 0.01);
            Assert.True(result.Stock >= 0);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(20)]
        public async Task GetById_ProductDoesntExist_ReturnsNotFound(int productId)
        {
            var client = this.GetNewClient();
            var response = await client.GetAsync($"/api/Products/{productId}");
            //response.EnsureSuccessStatusCode();  -> no se utiliza porque espero un codigo distinto de los 200

            var statusCode = response.StatusCode.ToString();

            Assert.Equal("NotFound", statusCode);
        }



        [Fact]
        public async Task CreateProduct_ValidData_ReturnsCreatedProduct()
        {
            var client = this.GetNewClient();

            // Create product

            var request = new CreateProductRequest
            {
                Description = "created Description product",
                Name = "created Name product",
                Price = 25.3,
                Stock = 1,
                CategoryId = 1
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response1 = await client.PostAsync("/api/Products", stringContent);
            response1.EnsureSuccessStatusCode();

            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            ////var createdProduct = JsonConvert.DeserializeObject<SingleProductResponse>(stringResponse1);
            var createdProduct = JsonConvert.DeserializeObject<ProductResponse>(stringResponse1);
            var statusCode1 = response1.StatusCode.ToString();

            //Assert.Equal("Created", statusCode1);
            Assert.Equal("Created", statusCode1);
            Assert.Equal(request.Description, createdProduct.Description);

            Assert.NotNull(createdProduct.Id);
            Assert.NotEqual(createdProduct.Id,0);
            Assert.True(createdProduct.Id > 0, "El Id tiene ser mayor a 0");
            Assert.Equal(request.Name, createdProduct.Name);
            Assert.Equal(request.Stock, createdProduct.Stock);
            


            //// Get created product
            //var response2 = await client.GetAsync($"/api/Products/{createdProduct.Id}");
            //response2.EnsureSuccessStatusCode();

            //var stringResponse2 = await response2.Content.ReadAsStringAsync();
            ///////var result2 = JsonConvert.DeserializeObject<SingleProductResponse>(stringResponse2);
            ////////var result2 = JsonConvert.DeserializeObject<SingleProductResponse>(stringResponse2);
            //var result2 = JsonConvert.DeserializeObject<ProductResponse>(stringResponse2);
            //var statusCode2 = response2.StatusCode.ToString();

            //Assert.Equal("OK", statusCode2);
            //Assert.Equal(createdProduct.Id, result2.Id);
            //Assert.Equal(createdProduct.Name, result2.Name);
            //Assert.Equal(createdProduct.Description, result2.Description);
            //Assert.Equal(createdProduct.Stock, result2.Stock);
        }



        [Fact]
        public async Task CreateProduct_InvalidData_ReturnsErrors()
        {
            var client = this.GetNewClient();

            // Create product
            var request = new CreateProductRequest
            {
                Description = "created Description",
                Name = null,
                Price = 0,
                Stock = 1,
                CategoryId = 1
            };
            //El Name no puede ser null y Price tiene que ser mayor a 0 (por lo que tiene que tener 2 errores de validacion)

            //Cuando no valida algun dato del modelo, el error que devuelve la Api es un BadRequest (400)
            //este tipo de error lo da el controlador de la Api automaticamente.
            //Hemos puesto   -> Name = null (esto daria error) y Price = 0 (esto tambien daria error)

            var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Products", stringContent);

            //  En este caso comentamos lo de -> response.EnsureSuccessStatusCode();
            //  ya que sabemos que va dar un error (osea un codigo distinto a 200)
            //  como por ejemplo la validacion de la entidad del dto que es un BadRequest (400)
            //  Asi que en este caso hay que comentarolo.
            //  Para que siga el codigo y poder validar los errores.
            //  OSEA si no me tiene que devolver un estado que no sea 200 se debe comentar esta linea.
            //response.EnsureSuccessStatusCode();




            var stringResponse = await response.Content.ReadAsStringAsync();


            //ASI ES CUANDO NO SE FORMATEA JSON:
            ////el modelo "BadRequestModel" me sirve para las respesta de la Api
            ////(cuando no pasa las validaciones del modelo)
            //var badRequest = JsonConvert.DeserializeObject<BadRequestModel>(stringResponse);
            //var statusCode = response.StatusCode.ToString();

            //Assert.Equal("BadRequest", statusCode);
            //Assert.NotNull(badRequest.Title);
            //Assert.NotNull(badRequest.Errors);
            //Assert.Equal(2, badRequest.Errors.Count);
            //Assert.Contains(badRequest.Errors.Keys, k => k == "Name");
            //Assert.Contains(badRequest.Errors.Keys, k => k == "Price");




            //ASI ES FORMATEADO EL JSON:
            //el modelo "BadRequestFormatdModel" me sirve para las respesta de la Api
            //(cuando no pasa las validaciones del modelo)

            ////DEVUELVE UN IENUMERABLE DE STRING COMO ESTE (KEY: VALUE):
            ////Name: The Name field is required.
            ////Price: The field Price must be between 0,01 and 1000.
            //var badRequestFormated = JsonConvert.DeserializeObject<BadRequestFormatedModel>(stringResponse);
            var badRequestFormated = JsonConvert.DeserializeObject<ApiErrorValidationEntity>(stringResponse);
            var statusCode = response.StatusCode.ToString();

            Assert.Equal("BadRequest", statusCode);
            Assert.NotNull(badRequestFormated.Message);
            Assert.NotNull(badRequestFormated.ErrorsValidationEntity);
            Assert.Equal(2, badRequestFormated.ErrorsValidationEntity.Count());
            Assert.Equal(1, Utilidades.TotalEncontradosEnListaString(badRequestFormated.ErrorsValidationEntity, "Name:"));
            Assert.Equal(1, Utilidades.TotalEncontradosEnListaString(badRequestFormated.ErrorsValidationEntity, "Price:"));

        }

        //private int TotalEncontradosEnListaString(List<string> Lista, string CadenaBuscar)
        //{
        //    int Encontrados = 0;
        //    foreach (var cadena in Lista)
        //    {
        //        if (cadena.Contains(CadenaBuscar))
        //        {
        //            Encontrados += 1;
        //        }
        //    }
        //    return Encontrados;
        //}



        [Fact]
        public async Task UpdateById_ValidData_ReturnsUpdatedProduct()
        {
            var client = this.GetNewClient();

            // Update product

            var productId = 6;
            var request = new UpdateProductRequest
            {
                Description = "Updated Description",
                Name = "Updated Name product",
                Price = 17.67,
                Stock = 94,
                CategoryId = 1
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response1 = await client.PutAsync($"/api/Products/{productId}", stringContent);
            response1.EnsureSuccessStatusCode();

            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            var updatedProduct = JsonConvert.DeserializeObject<ProductResponse>(stringResponse1);
            var statusCode1 = response1.StatusCode.ToString();

            Assert.Equal("OK", statusCode1);
            Assert.Equal(request.Description, updatedProduct.Description);

            Assert.NotNull(updatedProduct.Id);
            Assert.Equal(updatedProduct.Id, productId);
            Assert.Equal(request.Name, updatedProduct.Name);
            Assert.Equal(request.Stock, updatedProduct.Stock);




            //// Get updated product
            //var response2 = await client.GetAsync($"/api/Products/{updatedProduct.Id}");
            //response2.EnsureSuccessStatusCode();

            //var stringResponse2 = await response2.Content.ReadAsStringAsync();
            //var result2 = JsonConvert.DeserializeObject<ProductResponse>(stringResponse2);
            //var statusCode2 = response2.StatusCode.ToString();

            //Assert.Equal("OK", statusCode2);
            //Assert.Equal(updatedProduct.Id, result2.Id);
            //Assert.Equal(updatedProduct.Name, result2.Name);
            //Assert.Equal(updatedProduct.Description, result2.Description);
            //Assert.Equal(updatedProduct.Stock, result2.Stock);
        }



        [Fact]
        public async Task DeleteProductByIdById_ValidData_ReturnsNoContent204Code()
        {
            var client = this.GetNewClient();
            var productId = 5;

            // Delete product
            var response1 = await client.DeleteAsync($"/api/Products/{productId}");
            response1.EnsureSuccessStatusCode(); //como esta en la serie de 200 si va.

            var statusCode1 = response1.StatusCode.ToString();

            //Assert.Equal("NoContent", statusCode1);
            Assert.Equal("OK", statusCode1);


            //Get deleted product
            var response2 = await client.GetAsync($"/api/Products/{productId}");
            //response2.EnsureSuccessStatusCode(); //como NO esta en la serie de 200 esto no va (404).

            //  No tengo que deseralizar nada porque la respuesta es vacia
            //var result2 = JsonConvert.DeserializeObject<ProductResponse>(stringResponse2);
            var statusCode2 = response2.StatusCode.ToString();

            //regresa un NotFound (ya que el registro se elimino antes)
            Assert.Equal("NotFound", statusCode2);
        }










    }
}