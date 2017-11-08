using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore
{
    public static class ModelExtensions
    {
        public static TModel CloneEx<TModel>(this TModel model) where TModel : ModelBase
        {
            return model.Clone() as TModel;
        }
    }
}
