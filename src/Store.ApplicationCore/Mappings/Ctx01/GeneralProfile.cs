using AutoMapper;
using Store.ApplicationCore.DTOs.Ctx01;
using Store.ApplicationCore.Entities.Ctx01;
using System;
using System.Collections.Generic;
using System.Text;

namespace Store.ApplicationCore.Mappings.Ctx01
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            //Product    (Los Ignore para columns que no estan destino lo
            //            necesita los Tests para que no den error)
            //****************************************************
            CreateMap<CreateProductRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<UpdateProductRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore());

            CreateMap<Product, ProductResponse>();




            //Category (Los Ignore para columns que no estan destino lo
            //          necesita los Tests para que no den error)
            //****************************************************
            CreateMap<CreateCategoryRequest, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateCategoryRequest, Category>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Category, CategoryResponse>();

        }
    }
}



/////Ejemplos:
//CreateMap<Usuario, UsuarioDto>()
//    .ForMember(dest => dest.Departamento, origen => origen.MapFrom(origen => origen.Departamento.Nombre))
//    .ForMember(dest => dest.Perfil, origen => origen.MapFrom(origen => origen.Perfil.Nombre))
//    .ReverseMap()
//    .ForMember(origen => origen.Perfil, dest => dest.Ignore())
//    .ForMember(origen => origen.Departamento, dest => dest.Ignore());

//CreateMap<Producto, ProductoDto>()
//    .ForMember(dest => dest.Usuario, origen => origen.MapFrom(origen => origen.Usuario.Nombre))
//    .ReverseMap()
//    .ForMember(origen => origen.Usuario, dest => dest.Ignore());
