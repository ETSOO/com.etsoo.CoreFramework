using com.etsoo.CoreFramework.User;
using System;

namespace com.etsoo.CoreFramework.Authentication
{
    /// <summary>
    /// Authorization action
    /// 授权操作
    /// </summary>
    public record AuthAction (ICurrentUser User, string Audience, TimeSpan LiveSpan, byte[] SecurityKeyBytes);
}
