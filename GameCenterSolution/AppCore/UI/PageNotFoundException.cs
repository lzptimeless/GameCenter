using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public class PageNotFoundException : Exception
    {
        public PageNotFoundException(string pageName)
        {
            PageName = pageName;
        }

        protected PageNotFoundException(SerializationInfo info, StreamingContext context)
        {
            PageName = info.GetString("PageName");
        }

        public string PageName { get; set; }

        public override string Message
        {
            get
            {
                return $"Page not found: {PageName}";
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("PageName", PageName);
        }
    }
}
