using com.etsoo.Utils.String;

namespace com.etsoo.Utils.Actions
{
    /// <summary>
    /// Id data modal
    /// </summary>
    /// <typeparam name="T">Generic id type</typeparam>
    public record IdModal<T>(T? Id)
    {
        public static IdModal<T> Create(StringKeyDictionaryObject dic)
        {
            return new IdModal<T>(dic.GetExact<T>("id"));
        }
    }
}
