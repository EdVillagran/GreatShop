using System.Collections.Generic;

namespace GreatShop.Models.ViewModels
{
    public class CategoryVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Category { get; set; }
    }
}
