using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Mook.Util
{
    /// <summary>
    /// 接口请求类
    /// </summary>
    public class WebReq
    {
        public static string baseUrl = ConfigurationManager.AppSettings["BaseUrl"];

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="dicParams">请求条件</param>
        /// <returns></returns>
        public static ResponseMsg Get(string url, Dictionary<string, string> dicParams = null)
        {
            try
            {
                if (dicParams != null)
                {
                    var strParam = string.Join("&", dicParams.Select(o => o.Key + "=" + o.Value));
                    url = string.Concat(url, '?', strParam);
                }
#if NET45
                var jsonData = HttpHelper.HttpClientGet($"{baseUrl}{url}");
#elif NETSTANDARD2_0
                if (string.IsNullOrEmpty(baseUrl))
                    baseUrl = AppSettingsHelper.Configuration["BaseUrl"];
                var jsonData = HttpHelper.RestSharpGet($"{baseUrl}{url}");
#endif
                return JsonHelper.ToObject<ResponseMsg>(jsonData);
            }
            catch (Exception ex) 
            { 
                return ResponseMsg.Failed(ex.Message); 
            }
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">POST数据</param>
        /// <returns></returns>
        public static ResponseMsg Post(string url, string data)
        {
            try
            {
#if NET45
                var jsonData = HttpHelper.HttpClientPost($"{baseUrl}{url}", data);
#elif NETSTANDARD2_0
                if (string.IsNullOrEmpty(baseUrl))
                    baseUrl = AppSettingsHelper.Configuration["BaseUrl"];
                var jsonData = HttpHelper.RestSharpPost($"{baseUrl}{url}", data);
#endif
                return JsonHelper.ToObject<ResponseMsg>(jsonData);
            }
            catch (Exception ex) 
            { 
                return ResponseMsg.Failed(ex.Message); 
            }
        }
    }
}
