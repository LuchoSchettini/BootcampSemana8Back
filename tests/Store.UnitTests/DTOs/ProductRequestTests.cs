using Store.ApplicationCore.DTOs.Ctx01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.UnitTests.DTOs
{
    public class ProductRequestTests : BaseTests
    {
        //Tests de todas las clases Request de Products.
        //Que son las clases que recibimos del cliente.
        //En este caso CreateProductRequest y UpdateProductRequest

        [Theory]
        [InlineData("Test", "Description", 0.02, 1, 3, 0)]
        [InlineData("Test", null, 0.02, 1, 3, 1)]
        [InlineData(null, null, 0.02, 1, 3, 2)]
        [InlineData(null, null, -1, 1, 3, 3)]
        [InlineData(null, null, -1, 999, 3, 4)]
        [InlineData(null, null, -1, 999, 0, 5)]
        public void ValidateModel_CreateProductRequest_ReturnsCorrectNumberOfErrors
            (string name, string description, double price,
            int Stock, int CategoryId, int numberExpectedErrors)
        {
            var request = new CreateProductRequest
            {
                Name = name,
                Description = description,
                Price = price,
                Stock = Stock,
                CategoryId = CategoryId
            };

            var errorList = ValidateModel(request);

            Assert.Equal(numberExpectedErrors, errorList.Count);
        }


        [Theory]
        [InlineData("Test", null, 0.02, 1, 3, 0)]
        [InlineData("Te", "", 0.02, 1, 3, 1)]
        [InlineData("Te", null, 0.02, 1, 3, 1)]
        [InlineData("Te", "", -1, 1, 3, 2)]
        [InlineData("Te", "", -1, 999, 3, 3)]
        [InlineData("Te", "", -1, 999, 0, 4)]
        public void ValidateModel_UpdateProductRequest_ReturnsCorrectNumberOfErrors
            (string name, string description, double price, 
            int stock, int CategoryId, int numberExpectedErrors)
        {
            //Descripcion -> es nunable por lo que no da error ""/null.
            //por eso solo puedo tener maximo 4 errores y no 5
            var request = new UpdateProductRequest
            {
                Name = name,
                Description = description,
                Price = price,
                Stock = stock,
                CategoryId = CategoryId
            };

            var errorList = ValidateModel(request);

            Assert.Equal(numberExpectedErrors, errorList.Count);
        }


    }
}
