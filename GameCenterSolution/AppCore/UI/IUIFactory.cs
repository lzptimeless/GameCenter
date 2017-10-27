using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    internal interface IUIFactory
    {
        IBar CreateCaptionBar();
        IBar CreateTopBar();
        IBar CreateBottomBar();
        IBar CreateLeftBar();
        IBar CreateRightBar();
        IPage CreateHomePage();
        IPage CreatePage(string pageTypeName);
    }
}
