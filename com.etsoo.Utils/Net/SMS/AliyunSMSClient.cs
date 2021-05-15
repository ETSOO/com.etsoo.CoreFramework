using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Dysmsapi20170525;
using AlibabaCloud.SDK.Dysmsapi20170525.Models;
using com.etsoo.Utils.Actions;
using com.etsoo.Utils.String;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.etsoo.Utils.Net.SMS
{
    /// <summary>
    /// Aliyun SMS client
    /// 阿里云短信客户端
    /// </summary>
    public sealed class AliyunSMSClient : SMSClient
    {
        private readonly string accessKeyId;
        private readonly string accessKeySecret;

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="accessKeyId">Id</param>
        /// <param name="accessKeySecret">Secret</param>
        public AliyunSMSClient(string accessKeyId, string accessKeySecret)
        {
            this.accessKeyId = accessKeyId;
            this.accessKeySecret = accessKeySecret;
        }

        /// <summary>
        /// Constructor
        /// 构造函数
        /// </summary>
        /// <param name="section">Configuration section</param>
        public AliyunSMSClient(IConfigurationSection section) : this(
            section.GetValue<string>("AccessKeyId"),
            section.GetValue<string>("AccessKeySecret"))
        {
            var resources = section.GetSection("Resources");
            foreach (var r in resources.Get<IEnumerable<SMSResource>>())
            {
                AddResource(r);
            }
        }

        private Client CreateClient(string? endPoint)
        {
            var config = new Config
            {
                AccessKeyId = accessKeyId,
                AccessKeySecret = accessKeySecret,
                Endpoint = endPoint ?? "dysmsapi.aliyuncs.com"
            };

            return new Client(config);
        }

        /// <summary>
        /// Async send code
        /// 异步发送验证码
        /// </summary>
        /// <returns>Result</returns>
        public override async Task<IActionResult> SendCodeAsync(string mobile, string code, SMSResource? resource = null)
        {
            // Resource detect
            if (resource == null)
            {
                resource = this.GetResource();
                if (resource == null)
                {
                    throw new ArgumentNullException(nameof(resource));
                }
            }

            // Client
            var client = CreateClient(resource.EndPoint);

            // Request
            var request = new SendSmsRequest
            {
                PhoneNumbers = mobile,
                SignName = resource.Signature,
                TemplateCode = resource.Template,
                TemplateParam = "{\"code\":\"" + code + "\"}",
            };

            // Response
            var response = await client.SendSmsAsync(request);

            // Response result
            var result = response.Body;

            // To result
            if (result.Code == "OK")
            {
                return new ActionResult { Success = true };
            }
            else
            {
                // Additional data
                var data = new StringKeyDictionaryObject { ["RequestId"] = result.RequestId, ["BizId"] = result.BizId };

                // Result
                return new ActionResult(result.Code, data) { Title = result.Message };
            }
        }
    }
}
