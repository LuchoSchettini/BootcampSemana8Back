using AutoMapper;
using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Entities.Ctx01;
using Store.ApplicationCore.Mappings.Ctx01;
using System.Runtime.Serialization;

namespace Store.UnitTests.Mappings
{
    public class MappingTests
    {

        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;

        public MappingTests()
        {
            _configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<GeneralProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Fact]
        public void ShouldBeValidConfiguration()
        {
            _configuration.AssertConfigurationIsValid();
        }


        [Theory]
        [InlineData(typeof(CreateProductRequest), typeof(Product))]
        [InlineData(typeof(UpdateProductRequest), typeof(Product))]
        [InlineData(typeof(Product), typeof(ProductResponse))]
        [InlineData(typeof(CreateCategoryRequest), typeof(Category))]
        [InlineData(typeof(UpdateCategoryRequest), typeof(Category))]
        [InlineData(typeof(Category), typeof(CategoryResponse))]
        public void Map_SoureceToDestination_ExistConfiguration(Type origin, Type destination)
        {
            var instance = FormatterServices.GetUninitializedObject(origin);
            _mapper.Map(instance, origin, destination);
        }


    }
}
