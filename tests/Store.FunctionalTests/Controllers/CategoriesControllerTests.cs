using Newtonsoft.Json;
using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Helpers.Pager;
using Store.ApplicationCore.ResponsesApi.Errors;
using Store.ApplicationCore.Helpers.Utils;
using Store.FunctionalTests.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Store.FunctionalTests.Controllers
{
    public class CategoriesControllerTests : BaseControllerTests
    {
        public CategoriesControllerTests(CustomWebApplicationFactory<Program> factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_ReturnsAllRecords()
        {
            //arange
            var client = this.GetNewClient();
            //act
            var response = await client.GetAsync("/api/Categories");
            //esto no es obligatorio, pero sirve para que lance un excepcion en el caso de que 
            //algo falle, daria error cualquier codigo que no sea un 200, como por ejemplo la validacion
            //de la entidad del dto que es un BadRequest (400) -> (asi que es mejor ponerlo).
            //OSEA si no me tiene que devolver un estado que no sea 200 se debe comentar esta linea.
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<CategoryResponse>>(stringResponse).ToList();
            //me devuelve la palabra del statuscode por ejemplo ->
            //"OK" que es un 200
            //"NotFound"...
            var statusCode = response.StatusCode.ToString();

            //assert
            Assert.Equal("OK", statusCode);
            Assert.True(result.Count == 10);
        }



        //Paginando con los 10 reg de las pruebas :
        //Cuando el Serch esta vacio no lo pone                 -> (no hace filtro de Serch)
        //El Serch lo podemos poner vacio como se ve  (Search=) -> (no hace filtro de Serch)
        //http://localhost:5000/api/Categories/GetAllFilters?Search=&PageSize=4&PageIndex=1   //devuelve 4 reg.  
        //http://localhost:5000/api/Categories/GetAllFilters?Search=&PageSize=4&PageIndex=2   //devuelve 4 reg.
        //http://localhost:5000/api/Categories/GetAllFilters?Search=&PageSize=4&PageIndex=3   //devuelve 2 reg.

        //PageSize=5 , PageIndex=1  y  Search=""  es lo predeternidado si no ponermos nada
        //osea que con que enviamos el parametro de Search es suficiente a menos que quieras algun valor de parametro predeterminado
        //Filtrando
        //http://localhost:5000/api/Categories/GetAllFilters?PageSize=5&PageIndex=1&Search=Category%201    //devuelve 2 reg.      
        //http://localhost:5000/api/Categories/GetAllFilters?PageSize=5&PageIndex=1&Search=Category%2010   //devuelve 1 reg.

        [Theory]
        [InlineData("", 4, 1, 4)]
        [InlineData("", 4, 2, 4)]
        [InlineData("", 4, 3, 2)]
        [InlineData("Category 1", 5, 1, 2)]
        [InlineData("Category 10", 5, 1, 1)]
        public async Task GetAllFilters_CorrectData_ReturnsAllRegInPagers(
             string StrDeBusqueda, int PageSize, int PageIndex, int TotalRegInList)
        {
            var client = this.GetNewClient();

            //Error (la lista llega vacia por lo que es incorrecto, mirarlo mañana)
            var response1 = await client.GetAsync($"/api/Categories/GetAllFilters?Search={StrDeBusqueda}&PageSize={PageSize}&PageIndex={PageIndex}");

            response1.EnsureSuccessStatusCode(); //como esta en la serie de 200 si va.

            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            var result1 = JsonConvert.DeserializeObject<PagerResponse<CategoryResponse>>(stringResponse1);
            var statusCode1 = response1.StatusCode.ToString();

            Assert.Equal("OK", statusCode1);
            Assert.Equal(PageIndex, result1.PageIndex);
            Assert.Equal(PageSize, result1.PageSize);
            Assert.Equal(StrDeBusqueda.ToLower(), result1.Search); //cuando lo recibe del Json esta en minuscula por lo que los comparo en minuscula.


            Assert.Equal(TotalRegInList, result1.Registers.Count());
        }



        [Fact]
        public async Task GetById_CategoryExists_ReturnsCorrectCategory()
        {
            var categoryId = 5;
            var client = this.GetNewClient();
            var response = await client.GetAsync($"/api/Categories/{categoryId}");
            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            ////////var result = JsonConvert.DeserializeObject<SingleCategoryResponse>(stringResponse);
            var result = JsonConvert.DeserializeObject<CategoryResponse>(stringResponse);
            var statusCode = response.StatusCode.ToString();

            Assert.Equal("OK", statusCode);
            Assert.Equal(categoryId, result.Id);
            Assert.NotNull(result.Name);
            Assert.NotNull(result.Description);
        }


        [Theory]
        [InlineData(0)]
        [InlineData(20)]
        public async Task GetById_CategoryDoesntExist_ReturnsNotFound(int categoryId)
        {
            var client = this.GetNewClient();
            var response = await client.GetAsync($"/api/Categories/{categoryId}");

            var statusCode = response.StatusCode.ToString();

            Assert.Equal("NotFound", statusCode);
        }




        [Fact]
        public async Task CreateCategory_ValidData_ReturnsCreatedCategory()
        {
            var client = this.GetNewClient();

            // Create category
            var request = new CreateCategoryRequest
            {
                Description = "Create Description",
                Name = "create category Name"
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response1 = await client.PostAsync("/api/Categories", stringContent);
            response1.EnsureSuccessStatusCode();

            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            ////var createdCategory = JsonConvert.DeserializeObject<SingleCategoryResponse>(stringResponse1);
            var createdCategory = JsonConvert.DeserializeObject<CategoryResponse>(stringResponse1);
            var statusCode1 = response1.StatusCode.ToString();

            //Assert.Equal("Created", statusCode1);
            Assert.Equal("Created", statusCode1);
            Assert.Equal(request.Description, createdCategory.Description);

            Assert.NotNull(createdCategory.Id);
            Assert.True(createdCategory.Id > 0, "El Id tiene ser mayor a 0");
            Assert.Equal(request.Name, createdCategory.Name);


            //// Get created Category
            //var response2 = await client.GetAsync($"/api/Categories/{createdCategory.Id}");
            //response2.EnsureSuccessStatusCode();

            //var stringResponse2 = await response2.Content.ReadAsStringAsync();
            ///////var result2 = JsonConvert.DeserializeObject<SingleCategoryResponse>(stringResponse2);
            ////////var result2 = JsonConvert.DeserializeObject<SingleCategoryResponse>(stringResponse2);
            //var result2 = JsonConvert.DeserializeObject<CategoryResponse>(stringResponse2);
            //var statusCode2 = response2.StatusCode.ToString();

            //Assert.Equal("OK", statusCode2);
            //Assert.Equal(createdCategory.Id, result2.Id);
            //Assert.Equal(createdCategory.Name, result2.Name);
            //Assert.Equal(createdCategory.Description, result2.Description);
        }



        [Fact]
        public async Task CreateCategory_InvalidData_ReturnsErrors()
        {
            var client = this.GetNewClient();

            // Create Category
            var request = new CreateCategoryRequest
            {
                Description = "Create Description",
                Name = null,
            };
            //el valor de Name es obligatorio (por lo que tiene que dar error de validacion)

            //Cuando no valida algun dato del modelo, el error que devuelve la Api es un BadRequest (400)
            //este tipo de error lo da el controlador de la Api automaticamente.
            //Hemos puesto   -> Name = null (esto daria error)

            var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/Categories", stringContent);

            //  En este caso comentamos lo de -> response.EnsureSuccessStatusCode();
            //  ya que sabemos que va dar un error (osea un codigo distinto a 200)
            //  como por ejemplo la validacion de la entidad del dto que es un BadRequest (400)
            //  Asi que en este caso hay que comentarolo.
            //  Para que siga el codigo y poder validar los errores.
            //  OSEA si no me tiene que devolver un estado que no sea 200 se debe comentar esta linea.
            //response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();


            //ASI ES CUANDO NO SE FORMATEA JSON:
            ////El modelo "BadRequestModel" me sirve para las respesta de la Api
            ////(cuando no pasa las validaciones del modelo)
            //var badRequest = JsonConvert.DeserializeObject<BadRequestModel>(stringResponse);
            //var statusCode = response.StatusCode.ToString();

            //Assert.Equal("BadRequest", statusCode);
            //Assert.NotNull(badRequest.Title);
            //Assert.NotNull(badRequest.Errors);
            //Assert.Equal(1, badRequest.Errors.Count());
            //Assert.Contains(badRequest.Errors.Keys, k => k == "Name");


            //ASI ES FORMATEADO EL JSON:
            //el modelo "BadRequestFormatdModel" me sirve para las respesta de la Api
            //(cuando no pasa las validaciones del modelo)

            ////DEVUELVE UN IENUMERABLE DE STRING COMO ESTE (KEY: VALUE):
            ////Name: The Name field is required.



            //LO QUE DEVUELVE
            //{ 
            //    "errorsValidationEntity":["Name: The Name field is required."],
            //    "statusCode":400,
            //    "message":"Has realizado una petición incorrecta.",
            //    "isSuccessful":false
            //}

            ////var badRequestFormated = JsonConvert.DeserializeObject<BadRequestFormatedModel>(stringResponse);
            var badRequestFormated = JsonConvert.DeserializeObject<ApiErrorValidationEntity>(stringResponse);

            var statusCode = response.StatusCode.ToString();

            Assert.Equal("BadRequest", statusCode);
            Assert.NotNull(badRequestFormated.Message);
            Assert.NotNull(badRequestFormated.ErrorsValidationEntity);
            Assert.Equal(1, Utilidades.TotalEncontradosEnListaString(badRequestFormated.ErrorsValidationEntity, "Name:"));
            Assert.Equal(1, badRequestFormated.ErrorsValidationEntity.Count());   
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
        public async Task UpdateById_ValidData_ReturnsUpdatedCategory()
        {
            var client = this.GetNewClient();

            // Update Category
            var categoryId = 6;
            var request = new UpdateCategoryRequest
            {
                Description = "Description Updated",
                Name = "Name category Updated",
            };

            var stringContent = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

            var response1 = await client.PutAsync($"/api/Categories/{categoryId}", stringContent);
            response1.EnsureSuccessStatusCode();

            var stringResponse1 = await response1.Content.ReadAsStringAsync();
            var updatedCategory = JsonConvert.DeserializeObject<CategoryResponse>(stringResponse1);
            var statusCode1 = response1.StatusCode.ToString();

            Assert.Equal("OK", statusCode1);
            Assert.Equal(request.Description, updatedCategory.Description);
            Assert.Equal(request.Name, updatedCategory.Name);

            Assert.NotNull(updatedCategory.Id);
            Assert.Equal(updatedCategory.Id, categoryId);




            //// Get updated Category
            //var response2 = await client.GetAsync($"/api/Categories/{updatedCategory.Id}");
            //response2.EnsureSuccessStatusCode();

            //var stringResponse2 = await response2.Content.ReadAsStringAsync();
            //var result2 = JsonConvert.DeserializeObject<CategoryResponse>(stringResponse2);
            //var statusCode2 = response2.StatusCode.ToString();

            //Assert.Equal("OK", statusCode2);
            //Assert.Equal(updatedCategory.Id, result2.Id);
            //Assert.Equal(updatedCategory.Name, result2.Name);
            //Assert.Equal(updatedCategory.Description, result2.Description);
        }



        [Fact]
        public async Task DeleteCategoryByIdById_ReturnsNoContent204()
        {
            var client = this.GetNewClient();
            var categorytId = 5;

            // Delete Category
            var response1 = await client.DeleteAsync($"/api/Categories/{categorytId}");
            response1.EnsureSuccessStatusCode(); //como esta en la serie de 200 si va.

            var statusCode1 = response1.StatusCode.ToString();

            //Assert.Equal("NoContent", statusCode1);
            Assert.Equal("OK", statusCode1);


            //Get deleted Category
            var response2 = await client.GetAsync($"/api/Categories/{categorytId}");
            //response2.EnsureSuccessStatusCode(); //como NO esta en la serie de 200 esto no va (404).

            //  No tengo que deseralizar nada porque la respuesta es vacia
            //var result2 = JsonConvert.DeserializeObject<CategoryResponse>(stringResponse2);
            var statusCode2 = response2.StatusCode.ToString();

            //regresa un NotFound (ya que el registro se elimino antes)
            Assert.Equal("NotFound", statusCode2);
        }






    }
}