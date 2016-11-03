using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZDL.EF.CodeFirst.Model
{
    public class Contact
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public System.DateTime CreateDate { get; set; }
        public virtual Address Address { get; set; }

    }
}
