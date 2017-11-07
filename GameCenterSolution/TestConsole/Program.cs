using AppCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // 验证PropertySupport
            var propPath = PropertySupport.ExtractPropertyPath((B b) => b.A.Age);
            var propPath1 = PropertySupport.ExtractPropertyPath(() => new B().A.Age);
            var propPath2 = PropertySupport.ExtractPropertyPath(() => new A().Age);
            // 验证PropertyPath.GetHashCode
            PrintPropertyPath(propPath);
            PrintPropertyPath(propPath1);
            PrintPropertyPath(propPath2);

            Console.WriteLine(propPath.GetHashCode());
            Console.WriteLine(propPath1.GetHashCode());
            Console.WriteLine(propPath2.GetHashCode());

            Console.WriteLine($"equals:{propPath==propPath1}");
            Console.WriteLine($"equals:{propPath==propPath2}");

            Console.ReadKey();
            
        }

        static void PrintPropertyPath(PropertyPath propPath)
        {
            foreach (var propertyInfo in propPath.Names)
            {
                Console.WriteLine($"{propertyInfo.Name}:{propertyInfo.MetadataToken}");
            }
        }
    }

    class A
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    class B
    {
        public A A { get; set; }
    }
}
