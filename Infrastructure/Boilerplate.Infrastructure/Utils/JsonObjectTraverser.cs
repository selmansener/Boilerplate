using Newtonsoft.Json.Linq;

namespace Boilerplate.Infrastructure.Utils
{
    public static class JsonObjectTraverser
    {
        public static void Iterate(dynamic variable, Action<JValue> mutator)
        {
            if (variable.GetType() == typeof(JObject))
            {
                foreach (var property in variable)
                {
                    Iterate(property.Value, mutator);
                }
            }
            else if (variable.GetType() == typeof(JArray))
            {
                foreach (var item in variable)
                {
                    Iterate(item, mutator);
                }
            }
            else if (variable.GetType() == typeof(JValue))
            {
                mutator(variable);
            }
        }
    }
}
