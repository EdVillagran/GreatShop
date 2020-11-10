using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GreatShop.Models
{
    public class Category
    {
        //snipet: prop <tab> <tab>
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }



        
    }
}
