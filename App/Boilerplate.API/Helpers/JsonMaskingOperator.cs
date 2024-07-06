using Boilerplate.Infrastructure.Utils;
using Newtonsoft.Json;

using Serilog;
using Serilog.Enrichers.Sensitive;

namespace Boilerplate.API.Helpers
{
    internal class JsonMaskingOperator : IMaskingOperator
    {
        private readonly IReadOnlyList<string> _sensitiveProperties;

        public JsonMaskingOperator(string[] sensitiveData)
        {
            _sensitiveProperties = sensitiveData;
        }

        public MaskingResult Mask(string input, string mask)
        {
            bool isMatch = false;
            var dynamicObj = (object)null;
            try
            {
                dynamicObj = JsonConvert.DeserializeObject(input);

                JsonObjectTraverser.Iterate(dynamicObj, prop =>
                {
                    var propName = prop.Path.Split('.').LastOrDefault();

                    if (_sensitiveProperties.Any(sp => sp.ToLowerInvariant() == propName?.ToLowerInvariant()))
                    {
                        isMatch = true;
                        prop.Value = mask;
                    }
                });
            }
            catch (Exception ex) when (ex is not JsonReaderException)
            {
                Log.Logger.Fatal(ex, "Sensitive data masking failed while logging.");
            }

            return new MaskingResult
            {
                Match = isMatch,
                Result = dynamicObj == null ? input : JsonConvert.SerializeObject(dynamicObj)
            };
        }
    }
}
