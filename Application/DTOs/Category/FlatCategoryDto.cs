using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Category
{
    public class FlatCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentCategoryId { get; set; }
    }
}
