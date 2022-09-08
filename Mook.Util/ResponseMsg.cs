namespace Mook.Util
{
    /// <summary>
    /// 请求响应消息类 Request Result
    /// </summary>
    public class ResponseMsg
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 返回JSON格式数据
        /// </summary>
        public string JsonData { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 操作成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseMsg Successed(string message = "操作成功")
        {
            return new ResponseMsg
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// 操作成功并返回数据
        /// </summary>
        /// <param name="recordCount"></param>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static ResponseMsg Successed(int recordCount, string jsonData)
        {
            return new ResponseMsg
            {
                Success = true,
                RecordCount = recordCount,
                JsonData = jsonData
            };
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseMsg Failed(string message = "操作失败")
        {
            return new ResponseMsg
            {
                Success = false,
                Message = message
            };
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseMsg Failed(int statusCode, string message = "操作失败")
        {
            return new ResponseMsg
            {
                Success = false,
                StatusCode = statusCode,
                Message = message
            };
        }
    }
}
