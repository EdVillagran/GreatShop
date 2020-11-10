using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreatShop.Models.ViewModels
{
    public class CategoryVM
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Category { get; set; }
    }
}
