using System.Collections;
using System.Collections.Generic;

namespace GreatShop.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public ArrayList Quantities;
        public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }



    }
}
