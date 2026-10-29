using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEasy.Models
{
    public class wishlist
    {

         
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Cid { get; set; }
        public string username { get; set; }

        [ForeignKey("ProductType")]
        public int Pid { get; set; }
        public ProductType? ProductType { get; set; }


    }
}

