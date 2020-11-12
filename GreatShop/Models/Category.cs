using System.ComponentModel.DataAnnotations;

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
