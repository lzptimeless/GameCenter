using AppCore;
using GameCenter.Enhancement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Enhancement
{
    public class EnhancementModule : IEnhancement
    {
        public void Initialize(IModule[] dependencies)
        {
            Console.WriteLine("EnhancementModule initialize.");
            //throw new NotImplementedException();
        }

        public void Release()
        {
            Console.WriteLine("EnhancementModule release.");
            //throw new NotImplementedException();
        }

        public void UnsubscribeEvents(object target)
        {

        }
    }
}
