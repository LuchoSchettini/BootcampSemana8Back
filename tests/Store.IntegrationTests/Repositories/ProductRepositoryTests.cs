using AutoMapper;
using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Exceptions;
using Store.ApplicationCore.Helpers.Pager;
using Store.ApplicationCore.Mappings.Ctx01;
using Store.Infrastructure.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.IntegrationTests.Repositories;

public class ProductRepositoryTests : IClassFixture<SharedDatabaseFixture>
{

    private readonly IMapper _mapper;
    private SharedDatabaseFixture Fixture { get; }

    public ProductRepositoryTests(SharedDatabaseFixture fixture)
    {
        Fixture = fixture;

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GeneralProfile>();
        });

        _mapper = configuration.CreateMapper();
    }

    //Usaremos transaction cundo hagamos cabios en los datos asi cunado termine el Test 
    //estos cambios no se permaneceran (asi en los demas testes los datos seran los mimsmos que al principio)
    //ya veras en los metodos Tests como se hace:

    [Fact]
    public void GetAll_ReturnsAllProducts()
    {
        using (var context = Fixture.CreateContext())
        {
            var repository = new UnitOfWork(context, _mapper);
            //var repository = new CategoryRepository(context, _mapper);

            var products = repository.Products.GetAll();

            Assert.Equal(10, products.Count);
        }
    }

    [Fact]
    public void GetById_ProductExist_ReturnsProduct()
    {
        using (var context = Fixture.CreateContext())
        {
            var repository = new UnitOfWork(context, _mapper);
            //var repository = new CategoryRepository(context, _mapper);

            var productId = 1;
            var product = repository.Products.GetById(productId);

            Assert.NotNull(product);
        }
    }

    [Fact]
    public void GetById_ProductDoesntExist_ThrowsNotFoundException()
    {
        using (var context = Fixture.CreateContext())
        {
            var repository = new UnitOfWork(context, _mapper);
            //var repository = new CategoryRepository(context, _mapper);

            var productId = 56;

            Assert.Throws<NotFoundException>(() => repository.Products.GetById(productId));
        }
    }

    [Fact]
    public void Create_SavesCorrectData()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {
            var productId = 0;

            var request = new CreateProductRequest
            {
                Name = "Product 11",
                Description = "Description 11",
                Price = 5,
                Stock = 1,
                CategoryId = 1
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new ProductRepository(context, _mapper);

                var product = repository.Products.Create(request);
                productId = product.Id;
            }

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new ProductRepository(context, _mapper);

                var product = repository.GetById(productId);

                Assert.NotNull(product);
                Assert.Equal(request.Name, product.Name);
                Assert.Equal(request.Description, product.Description);
                Assert.Equal(request.Price, product.Price);
                Assert.Equal(request.Stock, product.Stock);
                Assert.Equal(request.CategoryId, product.CategoryId);
            }
        }
    }


    [Fact]
    public void Create_ExistingProductNameUnique_ThrowsDataDbException()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {
            //este Product Name ya existe.
            var request = new CreateProductRequest
            {
                Name = "Product 1",
                Description = "Description 1",
                Price = 5,
                Stock = 1,
                CategoryId = 1
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new ProductRepository(context, _mapper);

                var action = () => repository.Products.Create(request);

                //Assert.Throws<NotFoundException>(action);
                Assert.Throws<ConflictDbException>(action);
            }
        }
    }


    [Fact]
    public void Create_NotExistingCategoryId_ThrowsDataDbException()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {
            //este CategoryId no existe.
            var request = new CreateProductRequest
            {
                Name = "Product 20",
                Description = "Description 20",
                Price = 5,
                Stock = 1,
                CategoryId = 999
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new ProductRepository(context, _mapper);

                var action = () => repository.Products.Create(request);

                //Assert.Throws<NotFoundException>(action);
                Assert.Throws<ConflictDbException>(action);
            }
        }
    }



    [Fact]
    public void UpdateById_SavesCorrectData()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {
            var productId = 1;

            var request = new UpdateProductRequest
            {
                Name = "Product 1",
                Description = "Description cambiada de 1",
                Price = 5.12,
                Stock = 23,
                CategoryId = 1
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new ProductRepository(context, _mapper);

                repository.Products.UpdateById(productId, request);
            }

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new ProductRepository(context, _mapper);

                var product = repository.GetById(productId);

                Assert.NotNull(product);
                Assert.Equal(request.Name, product.Name);
                Assert.Equal(request.Description, product.Description);
                Assert.Equal(request.Price, product.Price);
                Assert.Equal(request.Stock, product.Stock);
                Assert.Equal(request.CategoryId, product.CategoryId);
            }
        }
    }

    [Fact]
    public void UpdateById_ProductDoesntExist_ThrowsNotFoundException()
    {

        using (var transaction = Fixture.Connection.BeginTransaction())
        {

            var productId = 15;

            var request = new UpdateProductRequest
            {
                Name = "Product 15",
                Description = "Description 15",
                Price = 5.12,
                Stock = 23,
                CategoryId = 1
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new ProductRepository(context, _mapper);

                var action = () => repository.Products.UpdateById(productId, request);

                Assert.Throws<NotFoundException>(action);
            }


        }
    }


    [Fact]
    public void DeleteById_EnsuresProductIsDeleted()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {
            var productId = 2;

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new ProductRepository(context, _mapper);

                repository.Products.DeleteById(productId);
            }

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new ProductRepository(context, _mapper);

                var action = () => repository.Products.GetById(productId);

                Assert.Throws<NotFoundException>(action);
            }
        }
    }

    [Fact]
    public void DeleteById_ProductDoesntExist_ThrowsNotFoundException()
    {
        var productId = 48;

        using (var context = Fixture.CreateContext())
        {
            var repository = new UnitOfWork(context, _mapper);
            //var repository = new ProductRepository(context, _mapper);

            var action = () => repository.Products.DeleteById(productId);

            Assert.Throws<NotFoundException>(action);
        }
    }


    //Si no se pone nada estos valores el valor por defecto (asi que lo pongo para la prueba):
    //en el ultimo y demas hago una busqueda.
    // const int MaxPageSize = 50;  
    // int _pageSize = 5;      si el valor es > MaxPageSize ? MaxPageSize : value;
    // int _pageIndex = 1;     si el valor es <= 0 ? 1 : value;
    // string _search = "";    si el valor es = !string.IsNullOrEmpty(value) ? value.ToLower() : "";


    [Theory]
    [InlineData(1, 4, 3, 10, 4, "")]
    [InlineData(2, 4, 3, 10, 4, "")]
    [InlineData(3, 4, 3, 10, 2, "")]
    public async Task GetAllFiltringAsync_CorrectData_ReturnsAllProductsInPagers(
         int PageIndex, int PageSize, int TotalPages, int TotalReg, int TotalRegInList, string StringDeBusqueda)
    {
        using (var context = Fixture.CreateContext())
        {

            ////    Desde el UnitOfWork:
            var repository = new UnitOfWork(context, _mapper);
            PagerRequest pagerRequest = new PagerRequest();

            pagerRequest.PageIndex = PageIndex;
            pagerRequest.PageSize = PageSize;
            pagerRequest.Search = StringDeBusqueda;

            PagerResponse<ProductResponse> pagerResponse = await repository.Products.GetAllFiltringAsync(pagerRequest);

            Assert.Equal(PageIndex, pagerResponse.PageIndex);
            Assert.Equal(PageSize, pagerResponse.PageSize);
            Assert.Equal(TotalPages, pagerResponse.TotalPages);
            Assert.Equal(TotalReg, pagerResponse.Total);
            Assert.Equal(TotalRegInList, pagerResponse.Registers.Count());
        }
    }

    [Theory]
    [InlineData("Product 1", 2)]   //Product 1    Devuelve 2 reg  ->  Product 1 y Product  10  
    [InlineData("Product 10", 1)]  //Product 10   Product 1 reg  ->  Product 10 
    public async Task GetAllFiltringAsync_CorrectData_ReturnsAllProductsInPagersInSerch(
     string StringDeBusqueda, int TotalRegInList)
    {
        using (var context = Fixture.CreateContext())
        {

            ////    Desde el UnitOfWork:
            var repository = new UnitOfWork(context, _mapper);
            PagerRequest pagerRequest = new PagerRequest();

            //pagerRequest.PageIndex = PageIndex; 
            //pagerRequest.PageSize = PageSize;
            pagerRequest.Search = StringDeBusqueda;

            PagerResponse<ProductResponse> pagerResponse = await repository.Products.GetAllFiltringAsync(pagerRequest);

            Assert.Equal(TotalRegInList, pagerResponse.Registers.Count());
        }
    }



}//fin clase
