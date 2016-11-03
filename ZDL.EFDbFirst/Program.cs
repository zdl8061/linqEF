using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDL.EFDbFirst.Dal;
using ZDL.EFDbFirst.Entity;

namespace ZDL.EFDbFirst
{
    class Program
    {
        static void Main(string[] args)
        {
            var _shopDb = new ShopDb();

            //for (int i = 1; i < 100; i++)
            //{
            //    var _new = _shopDb.AddReturnModel<Z_Goods>(new Z_Goods
            //    {
            //        barcode = "aaaa" + i,
            //        goodsType = 2,
            //        isAudit = true,
            //        isEnabled = false,
            //        name = "测试商品" + i
            //    });
            //}

        }

        ////由于每次定义dbcontext对象都很慢，先初始化 如果是asp.net可放在application_start中初始所有dbcontext
        //public static void init()
        //{
        //    using (var dbcontext = new BrandDbContext())
        //    {
        //        var objectContext = ((IObjectContextAdapter)dbcontext).ObjectContext;
        //        var mappingCollection = (StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(DataSpace.CSSpace);
        //        mappingCollection.GenerateViews(new List<EdmSchemaError>());
        //    }
        //}
    }
}
