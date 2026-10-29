using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEasy.Models
{
    public class Stock
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Sid { get; set; }

        [ForeignKey("ProductType")]
        public int Pid { get; set; }

        public int Quantity { get; set; }

        public DateTime Lastupdate { get; set; }

        public ProductType ProductType { get; set; }




    }
}
