using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEasy.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Oid { get; set; }

        [ForeignKey("CutomerReg")]
        public string username { get; set; }
        public int quantity { get; set; }
        [ForeignKey("ProductType")]
        public int pid { get; set; }
        public string orderdate { get; set; }
        public int totalprice { get; set; }
        public int addressId {  get; set; }


        public int  status { get; set; }
        [NotMapped]
        public CustomerReg? CustomerReg { get; set; }
        [NotMapped]
        public ProductType? ProductType { get; set; }


    }
}
