using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEasy.Models
{
    public class ProductType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int pid { get; set; }
         
        public int CategoryID{get; set;}  
        public string ProductName{get; set; }
        public string Description { get; set;}
        public decimal Price { get; set; }
        [NotMapped]
        public  List<IFormFile>? Image { get; set; }
        public string? ImagePath { get; set; }
        [ForeignKey("CategoryID")]
        public ProductCategory? CategoryModel { get; set; }
        public Stock? Stock { get; set; }

        public string? Owner {  get; set; }

        public ICollection<wishlist>? wishlist { get; set; } = new List<wishlist>();
        public ICollection<Cart>? Cart { get; set; } = new List<Cart>();




    }
}
