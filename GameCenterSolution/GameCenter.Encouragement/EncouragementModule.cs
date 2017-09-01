using AppCore;
using GameCenter.Encouragement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Encouragement
{
    public class EncouragementModule : IEncouragement
    {
        public void Initialize(IModule[] dependencies)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Encouragement initialize.");
        }

        public void Release()
        {
            //throw new NotImplementedException();
            Console.WriteLine("Encouragement release.");
        }
    }
}
