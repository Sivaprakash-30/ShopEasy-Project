using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopEasy.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Fid { get; set; }
        public string username { get; set; }

        public int Pid { get; set; }

        public int Oid { get; set; }
        public string feedback { get; set; }
        public int Rating { get; set; }
        [NotMapped]
        public CustomerReg? Customer { get; set; }



    }
}
