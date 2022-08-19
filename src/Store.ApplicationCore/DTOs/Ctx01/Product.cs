using System;
using System.ComponentModel.DataAnnotations;

namespace Store.ApplicationCore.DTOs.Ctx01
{




    public class CreateProductRequest
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 1000)]
        public double Price { get; set; }

        [Required]
        [Range(0, 100)]
        public int Stock { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
    }

    //public class UpdateProductRequest : CreateProductRequest
    public class UpdateProductRequest
    {
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; } = null;


        public string Description { get; set; } = null;


        [Range(0.01, 1000)]
        public double? Price { get; set; } = null;


        [Range(0, 100)]
        public int? Stock { get; set; } = null;


        [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; } = null;
    }

    public class ProductResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
        public int CategoryId { get; set; }

    }


    ////Nuevo -> para el proyect de FunctionaTests:
    //public class SingleProductResponse : ProductResponse
    //{
    //    public string Description { get; set; }
    //}


}