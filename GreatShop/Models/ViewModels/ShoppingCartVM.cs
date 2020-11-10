using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GreatShop.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public ArrayList Quantities;
        public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }

      

    }
}
