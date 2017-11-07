using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    /// <summary>
    /// 可监控是否正在执行任务的接口
    /// </summary>
    public interface IDuring
    {
        bool During { get; }

        event EventHandler<DuringArgs> DuringChanged;
    }

    public class DuringArgs : EventArgs
    {
        public DuringArgs(bool during)
        {
            During = during;
        }

        public bool During { get; private set; }
    }
}
