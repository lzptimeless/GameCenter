using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppCore
{
    public class ConfigManager
    {
        private ConfigManager()
        { }

        #region fields
        
        #endregion

        #region properties
        #region Default
        private static ConfigManager _default;
        public static ConfigManager Default
        {
            get
            {
                if (_default == null)
                {
                    Interlocked.CompareExchange(ref _default, new ConfigManager(), null);
                }

                return _default;
            }
        }
        #endregion
        #endregion

        #region public
        
        #endregion
    }
}
