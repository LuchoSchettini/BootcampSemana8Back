using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.ApplicationCore.DTOs.Ctx01
{
    public class CreateCategoryRequest
    {
        [Required]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }

    }

    public class UpdateCategoryRequest
    {

        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; } = null;

        public string Description { get; set; } = null;
    }

    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }

}

