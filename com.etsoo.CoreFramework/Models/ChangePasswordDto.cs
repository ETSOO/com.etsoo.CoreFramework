namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Change password data
    /// 修改密码数据
    /// </summary>
    /// <param name="OldPassword">Current password</param>
    /// <param name="Password">New password</param>
    public record ChangePasswordDto(string OldPassword, string Password);
}
