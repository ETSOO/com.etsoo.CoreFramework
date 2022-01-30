namespace com.etsoo.Utils
{
    /// <summary>
    /// Shared utils
    /// 共享工具类
    /// </summary>
    public static class SharedUtils
    {
        public static bool EnumIsDefined<T>(T item) where T : Enum
        {
            return long.TryParse(item.ToString(), out _) is false;
        }
    }
}
