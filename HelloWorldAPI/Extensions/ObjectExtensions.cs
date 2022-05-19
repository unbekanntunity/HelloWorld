using System.Collections;
using System.Text;

namespace HelloWorldAPI.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object obj)
        {
            var properties = obj.GetType().GetProperties().Where(x => x.GetValue(obj) != null && x.CanRead);

            var sb = new StringBuilder("?");

            foreach (var property in properties)
            {
                var type = property.PropertyType;

                if (type.GetInterface(nameof(IEnumerable)) != null && type.Name != nameof(String))
                {
                    var value = (IEnumerable)property.GetValue(obj);
                    foreach (var item in value)
                    {
                        sb.Append($"&{property.Name}={item}");
                    }
                }
                else
                {
                    sb.Append($"&{property.Name}={property.GetValue(obj)}");
                }
            }

            sb.Remove(1, 1);
            return sb.ToString();
        }
    }
}
