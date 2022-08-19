using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Store.ApplicationCore.Entities.Ctx01
{

    //De forma predeterminada, los índices no son únicos
    //tambien se puden hacer indices compuestos -> [Index(nameof(FirstName), nameof(LastName))]
    [Index(nameof(Name), IsUnique = true, Name = "IX_Product_Name_UQ")]
    public class Product : BaseEntity
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        public string Description { get; set; }
        public int Stock { get; set; }
        public double Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //navegacion con Categoria
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}