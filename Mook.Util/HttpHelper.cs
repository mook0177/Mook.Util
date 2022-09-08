#if NETSTANDARD2_0
using RestSharp;
#endif
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Mook.Util
{
    /// <summary>
    /// Http请求帮助类 
    /// </summary>
    public class HttpHelper
    {
        static readonly string Msg = "调用接口[{0}]失败，原因：{1}";

        /// <summary>
        /// WebRequest Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">POST数据</param>
        /// <returns></returns>
        public static string WebRequestPost(string url, string data)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.Accept = "text/plain";
                request.ContentType = "application/json,multipart/form-data";
                byte[] buffer = Encoding.UTF8.GetBytes(data);
                request.ContentLength = buffer.Length;
                var stream = request.GetRequestStream();
                stream.Write(buffer, 0, buffer.Length);
                stream.Close();
                var response = (HttpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    response.Close();
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Msg, url, ex.Message));
            }
        }

        /// <summary>
        /// WebRequest Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public static string WebRequestGet(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                var response = (HttpWebResponse)request.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    response.Close();
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Msg, url, ex.Message));
            }
        }

        /// <summary>
        /// HttpClient Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">POST数据</param>
        /// <returns></returns>
        public static string HttpClientPost(string url, string data)
        {
            try
            {
                var httpContent = new StringContent(data);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var client = new HttpClient();
                var response = client.PostAsync(url, httpContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                throw new Exception(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Msg, url, ex.Message));
            }
        }

        /// <summary>
        /// HttpClient Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <returns></returns>
        public static string HttpClientGet(string url)
        {
            try
            {
                var client = new HttpClient();
                var response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                throw new Exception(response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Msg, url, ex.Message));
            }
        }

#if NETSTANDARD2_0
        /// <summary>
        /// RestSharp Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">POST数据</param>
        public static string RestSharpPost(string url, string data)
        {
            var client = new RestClient(url);
            var request = new RestRequest().AddJsonBody(data);
            var response = client.Post(request);
            if (response.IsSuccessful) return response.Content;
            throw new Exception(string.Format(Msg, url, response.ErrorException));
        }

        /// <summary>
        /// RestSharp Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        public static string RestSharpGet(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest();
            var response = client.Get(request);
            if (response.IsSuccessful) return response.Content;
            throw new Exception(string.Format(Msg, url, response.ErrorException));
        }
#endif
    }
}