using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ApplicationCore.Entities.Ctx01
{
    //De forma predeterminada, los índices no son únicos
    //tambien se puden hacer indices compuestos -> [Index(nameof(FirstName), nameof(LastName))]
    [Index(nameof(Name), IsUnique = true, Name = "IX_Category_Name_UQ")]
    public class Category : BaseEntity
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int Id { get; set; }

        [MaxLength(30)]
        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
