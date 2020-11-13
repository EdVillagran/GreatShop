using System.Collections;
using System.Collections.Generic;

namespace GreatShop.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public ArrayList Quantities { get; set; }
        public Product Product { get; set; }
        public IEnumerable<Product> Products { get; set; }



    }
}
