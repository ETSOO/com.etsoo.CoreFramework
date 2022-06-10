using com.etsoo.Utils.String;
using System.Text.Json;

namespace com.etsoo.Utils.Serialization
{
    /// <summary>
    /// Json snake naming policy
    /// Json 蛇型命名策略
    /// </summary>
    public class JsonSnakeNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToSnakeCase().ToString();
        }
    }
}
