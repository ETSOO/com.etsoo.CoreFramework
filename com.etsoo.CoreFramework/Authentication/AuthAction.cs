using System.Security.Claims;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Authorization action
    /// 授权操作
    /// </summary>
    public record AuthAction(ClaimsIdentity Claims, string Audience, TimeSpan LiveSpan, string KeyId = "SmartERP");
}
