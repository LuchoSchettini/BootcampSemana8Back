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

public class CategoryRepositoryTests : IClassFixture<SharedDatabaseFixture>
{

    private readonly IMapper _mapper;
    private SharedDatabaseFixture Fixture { get; }

    public CategoryRepositoryTests(SharedDatabaseFixture fixture)
    {
        Fixture = fixture;

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GeneralProfile>();
        });

        _mapper = configuration.CreateMapper();
    }



    [Fact]
    public void GetAll_ReturnsAllCategories()
    {
        using (var context = Fixture.CreateContext())
        {
            ////    Derectamente con el repository de Category:
            ////var repository = new CategoryRepository(context, _mapper);
            ////var categories = repository.GetAll();
            ////Assert.Equal(10, categories.Count);

            ////    Desde el UnitOfWork:
            var repository = new UnitOfWork(context, _mapper);
            var categories = repository.Categories.GetAll();
            Assert.Equal(10, categories.Count);
        }
    }



    //Si no se pone nada estos valores el valor por defecto (asi que lo pongo para la prueba):
    //en el ultimo y demas hago una busqueda.
                                               // const int MaxPageSize = 50;  
    // int _pageSize = 5;      si el valor es > MaxPageSize ? MaxPageSize : value;
    // int _pageIndex = 1;     si el valor es <= 0 ? 1 : value;
    // string _search = "";    si el valor es = !string.IsNullOrEmpty(value) ? value.ToLower() : "";


    [Theory]
    [InlineData(1, 4, 3, 10, 4 , "")]
    [InlineData(2, 4, 3, 10, 4, "")]
    [InlineData(3, 4, 3, 10, 2, "")]
    public async Task GetAllFiltringAsync_CorrectData_ReturnsAllCategoriesInPagers(
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

            PagerResponse<CategoryResponse> pagerResponse = await repository.Categories.GetAllFiltringAsync(pagerRequest);

            Assert.Equal(PageIndex, pagerResponse.PageIndex);
            Assert.Equal(PageSize, pagerResponse.PageSize);
            Assert.Equal(TotalPages, pagerResponse.TotalPages);
            Assert.Equal(TotalReg, pagerResponse.Total);
            Assert.Equal(TotalRegInList, pagerResponse.Registers.Count());
        }
    }

    [Theory]
    [InlineData("Category 1", 2)]   //Category 1    Devuelve 2 reg  ->  Category 1 y Category  10  
    [InlineData("Category 10", 1)]  //Category 10   Category 1 reg  ->  Category 10 
    public async Task GetAllFiltringAsync_CorrectData_ReturnsAllCategoriesInPagersInSerch(
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

            PagerResponse<CategoryResponse> pagerResponse = await repository.Categories.GetAllFiltringAsync(pagerRequest);

            Assert.Equal(TotalRegInList, pagerResponse.Registers.Count());
        }
    }



    [Fact]
    public void GetById_CategoryExist_ReturnsCategories()
    {
        using (var context = Fixture.CreateContext())
        {
            //var repository = new CategoryRepository(context, _mapper);
            var repository = new UnitOfWork(context, _mapper);
            var categoryId = 1;
            var category = repository.Categories.GetById(categoryId);
            Assert.NotNull(category);
        }
    }

    [Fact]
    public void GetById_CategoryDoesntExist_ThrowsNotFoundException()
    {
        using (var context = Fixture.CreateContext())
        {
            //var repository = new CategoryRepository(context, _mapper);
            var repository = new UnitOfWork(context, _mapper);
            var categoryId = 56;

            Assert.Throws<NotFoundException>(() => repository.Categories.GetById(categoryId));
        }
    }

    [Fact]
    public void Create_SavesCorrectData()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {
            var categoryId = 0;

            var request = new CreateCategoryRequest
            {
                Name = "Category 11",
                Description = "Description 11",
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new CategoryRepository(context, _mapper);

                var product = repository.Categories.Create(request);
                categoryId = product.Id;
            }

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new CategoryRepository(context, _mapper);

                var category = repository.Categories.GetById(categoryId);

                Assert.NotNull(category);
                Assert.Equal(request.Name, category.Name);
                Assert.Equal(request.Description, category.Description);
            }
        }
    }


    [Fact]
    public void Create_ExistingCategoryNameUnique_ThrowsDataDbException()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {
            //este Category Name ya existe.
            var request = new CreateCategoryRequest
            {
                Name = "Category 1",
                Description = "Description 1",
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new CategoryRepository(context, _mapper);

                var action = () => repository.Categories.Create(request);

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

            var request = new UpdateCategoryRequest
            {
                Name = "Category 1",
                Description = "Description cambiada de 1",
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new CategoryRepository(context, _mapper);

                repository.Categories.UpdateById(productId, request);
            }

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new CategoryRepository(context, _mapper);

                var product = repository.Categories.GetById(productId);

                Assert.NotNull(product);
                Assert.Equal(request.Name, product.Name);
                Assert.Equal(request.Description, product.Description);
            }
        }
    }

    [Fact]
    public void UpdateById_CategoryDoesntExist_ThrowsNotFoundException()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {

            var categoryId = 15;

            var request = new UpdateCategoryRequest
            {
                Name = "Category 15",
                Description = "Description 15",
            };

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new CategoryRepository(context, _mapper);
                var action = () => repository.Categories.UpdateById(categoryId, request);

                Assert.Throws<NotFoundException>(action);
            }
        }
    }


    [Fact]
    public void DeleteById_EnsuresCategoryIsDeleted()
    {
        using (var transaction = Fixture.Connection.BeginTransaction())
        {
            var categoryId = 2;

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new CategoryRepository(context, _mapper);

                repository.Categories.DeleteById(categoryId);
            }

            using (var context = Fixture.CreateContext(transaction))
            {
                var repository = new UnitOfWork(context, _mapper);
                //var repository = new CategoryRepository(context, _mapper);

                var action = () => repository.Categories.GetById(categoryId);

                Assert.Throws<NotFoundException>(action);
            }
        }
    }

    [Fact]
    public void DeleteById_CategoryDoesntExist_ThrowsNotFoundException()
    {
        var categoryId = 48;

        using (var context = Fixture.CreateContext())
        {
            var repository = new UnitOfWork(context, _mapper);
            //var repository = new CategoryRepository(context, _mapper);
            
            var action = () => repository.Categories.DeleteById(categoryId);

            Assert.Throws<NotFoundException>(action);
        }
    }



}//fin clase
