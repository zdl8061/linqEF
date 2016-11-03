using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDL.EF.CodeFirst.Model
{
    //[Table("FE",Schema)]
    public class Address
    {        
        public int ID { get; set; }
        public string Contury { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Code { get; set; }
    }
}
