using MVC2.Interface;
using MVC2.Models;
using MVC2.ViewModels;

namespace MVC2.Service
{
    public class MockPagination : IPagination
    {

        public int pageSize(string pageSize)
        {

            //get default page size from app.json
            var MyConfig = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            int ps = MyConfig.GetValue<int>("AppSettings:PageSize");

            if (pageSize != null)
            {
                ps = Int32.Parse(pageSize);
            }
            return ps;
        }

    }
}
