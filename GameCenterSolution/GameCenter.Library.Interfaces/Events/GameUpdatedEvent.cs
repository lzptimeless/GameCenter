using AppCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GameCenter.Library
{
    public class GameUpdatedEvent : PubSubEvent<GameUpdatedEventData>
    {
    }

    public class GameUpdatedEventData : ModelUpdatedEventData<Game>
    {
        public GameUpdatedEventData(Game model, IEnumerable<PropertyPath> updatedPaths)
            : base(model, updatedPaths)
        {
        }

        public new static GameUpdatedEventData Create<TProperty>(Game model,
            params Expression<Func<TProperty>>[] propertyExpressions)
        {
            if (model == null) throw new ArgumentNullException("model");

            List<PropertyPath> propertyPaths = null;
            if (propertyExpressions != null)
            {
                propertyPaths = new List<PropertyPath>();
                foreach (var propertyExpression in propertyExpressions)
                {
                    PropertyPath propPath = PropertySupport.ExtractPropertyPath(propertyExpression);
                    propertyPaths.Add(propPath);
                }
            }

            GameUpdatedEventData data = new GameUpdatedEventData(model, propertyPaths);
            return data;
        }
    }
}
