using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AppCore
{
    public static class ApplicationExtensions
    {
        public static IApp ToIApp(this Application application)
        {
            if (!(application is IApp)) throw new InvalidCastException("This Application instance not implement IApp");

            return (IApp)application;
        }
    }
}
