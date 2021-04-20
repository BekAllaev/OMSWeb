using OMSWeb.Data.Access.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Common.DataMock
{
    public class FakeDataSeeder
    {
        public static void Initialize(NorthwindContext context, FakeDataGenerator generator)
        {
            using (context)
            {
                context.Categories.AddRange(generator.Categories);
                context.Products.AddRange(generator.Products);

                context.SaveChanges();
            }
        }
    }
}
