using MVC2.Interface;
using MVC2.Models;
using MVC2.ViewModels;

namespace MVC2.Service
{
    public class MockPagination : IPagination
    {

        private readonly DatabaseContext _context;
        private readonly IStudent _studentinfo;

        public MockPagination(DatabaseContext context, IStudent studentinfo)
        {
            _context = context;
            _studentinfo = studentinfo;
        }
        public int pageSize(string pageSize)
        {
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
