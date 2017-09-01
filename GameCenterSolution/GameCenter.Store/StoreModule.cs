using AppCore;
using GameCenter.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Store
{
    public class StoreModule : IStore
    {
        public void Initialize(IModule[] dependencies)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Store initialize.");
        }

        public void Release()
        {
            //throw new NotImplementedException();
            Console.WriteLine("Store release.");
        }
    }
}
