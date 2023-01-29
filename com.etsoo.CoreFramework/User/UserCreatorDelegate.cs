using com.etsoo.Utils.String;
using System.Globalization;
using System.Net;

namespace com.etsoo.CoreFramework.User
{
    /// <summary>
    /// User creator delegate
    /// 用户创建者委托
    /// </summary>
    /// <typeparam name="T">Generic user type</typeparam>
    /// <returns>Result</returns>
    public delegate T? UserCreatorDelegate<T>(StringKeyDictionaryObject data, IPAddress ip, CultureInfo language, string region) where T : IServiceUser;
}