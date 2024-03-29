﻿using com.etsoo.Address;
using com.etsoo.Utils.Actions;

namespace com.etsoo.SMS
{
    /// <summary>
    /// SMS client interface
    /// 短信客户端接口
    /// </summary>
    public interface ISMSClient : ITemplateClient
    {
        /// <summary>
        /// Demestic region
        /// </summary>
        AddressRegion Region { get; }

        /// <summary>
        /// Async send SMS with template id
        /// 异步通过模板编号发送短信
        /// </summary>
        /// <param name="kind">Template kind</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="vars">Variables</param>
        /// <param name="templateId">Template id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<ActionResult> SendAsync(TemplateKind kind, IEnumerable<string> mobiles, Dictionary<string, string> vars, string templateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async send SMS with template id
        /// 异步通过模板编号发送短信
        /// </summary>
        /// <param name="kind">Template kind</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="vars">Variables</param>
        /// <param name="templateId">Template id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<ActionResult> SendAsync(TemplateKind kind, IEnumerable<AddressRegion.Phone> mobiles, Dictionary<string, string> vars, string templateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async send SMS
        /// 异步发送短信
        /// </summary>
        /// <param name="kind">Template kind</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="vars">Variables</param>
        /// <param name="template">Template</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<ActionResult> SendAsync(TemplateKind kind, IEnumerable<string> mobiles, Dictionary<string, string> vars, TemplateItem? template = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async send SMS
        /// 异步发送短信
        /// </summary>
        /// <param name="kind">Template kind</param>
        /// <param name="mobiles">Mobiles</param>
        /// <param name="vars">Variables</param>
        /// <param name="template">Template</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<ActionResult> SendAsync(TemplateKind kind, IEnumerable<AddressRegion.Phone> mobiles, Dictionary<string, string> vars, TemplateItem? template = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async send code with template id
        /// 异步通过模板编号发送验证码
        /// </summary>
        /// <param name="mobile">Mobile</param>
        /// <param name="code">Code</param>
        /// <param name="templateId">Template id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<ActionResult> SendCodeAsync(AddressRegion.Phone mobile, string code, string templateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async send code
        /// 异步发送验证码
        /// </summary>
        /// <param name="mobile">Mobile</param>
        /// <param name="code">Code</param>
        /// <param name="template">Template</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result</returns>
        Task<ActionResult> SendCodeAsync(AddressRegion.Phone mobile, string code, TemplateItem? template = null, CancellationToken cancellationToken = default);
    }
}
