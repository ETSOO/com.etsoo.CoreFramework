using MimeKit;

namespace com.etsoo.SMTP
{
    /// <summary>
    /// SMTP extensions
    /// SMTP 扩展
    /// </summary>
    public static class SMTPExtensions
    {
        /// <summary>
        /// Add range of addresses
        /// 添加多个地址
        /// </summary>
        /// <param name="list">Address list</param>
        /// <param name="addresses">String email addresses, like 'info@etsoo.com' or 'ETSOO &lt;info@etsoo.com&gt;'</param>
        public static void AddRange(this InternetAddressList list, IEnumerable<string>? addresses)
        {
            if (addresses == null) return;

            foreach (var address in addresses)
            {
                if (MailboxAddress.TryParse(address, out var mailboxAddress) && !list.Contains(mailboxAddress))
                {
                    list.Add(mailboxAddress);
                }
            }
        }
    }
}
