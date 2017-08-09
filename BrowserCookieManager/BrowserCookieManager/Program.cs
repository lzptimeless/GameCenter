using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowserCookieManager
{
    class Program
    {
        static void Main(string[] args)
        {
            ICookiesStorage cs = new FireFoxCookiesStorage();
            var cookies = cs.GetByDomain("baidu.com");

            foreach (var item in cookies)
            {
                Console.WriteLine(item);
            }
            //Console.ReadKey();

            //cs.DeleteByDomain("jd.com");
            //Console.WriteLine("Deleted jd.com");

            //cookies = cs.GetByDomain("baidu.com");
            //foreach (var item in cookies)
            //{
            //    Console.WriteLine(item);
            //}

            Console.ReadKey();
        }
    }
}
