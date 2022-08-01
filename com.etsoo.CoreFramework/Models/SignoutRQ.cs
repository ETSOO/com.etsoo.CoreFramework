namespace com.etsoo.CoreFramework.Models
{
    /// <summary>
    /// Signout request data
    /// 退出系统请求数据
    /// </summary>
    /// <param name="DeviceId">Client device id</param>
    public record SignoutRQ(string DeviceId);
}
