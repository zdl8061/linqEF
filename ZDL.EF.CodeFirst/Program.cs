using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDL.EF.CodeFirst.Data;
using ZDL.EF.CodeFirst.Model;

namespace ZDL.EF.CodeFirst
{
    class Program
    {

        static void Main(string[] args)
        {
            var _contact = new Contact();
            _contact.CreateDate = DateTime.Parse("2015-01-30");
            _contact.Name = "sdf";
            _contact.Address = new Address() {City = "北京" , Code = "017",Contury = "中国",Street = "总部基地"};   

            using (EFCodeFirstContext db = new EFCodeFirstContext("server=192.168.1.33;uid=sa;pwd=zdl;database=EFCodeFirst"))
            {
                //IQueryable r = from c in db.Contacts select c;

                var list = db.Contacts.ToList<Contact>();
                db.Contacts.Add(_contact);
                //List<Contact> s = r.ToList<Contact>();
                db.SaveChanges();
            }

            using (EFCodeFirstContext db = new EFCodeFirstContext("server=192.168.1.33;uid=sa;pwd=zdl;database=EFCodeFirst"))
            {

                db.Contacts.Add(_contact);
                db.SaveChanges();
            }

            Console.WriteLine("Finish");
            Console.ReadKey();
        }
    }
}
