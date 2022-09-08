using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mook.Util
{
    public class Tools
    {
        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            var ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            for (int i = 0; i < ipEntry.AddressList.Length; i++)
            {
                if (ipEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    return ipEntry.AddressList[i].ToString();
                }
            }
            return "";
        }

        private static DateTime _dt = DateTime.Now;
        /// <summary>
        /// 禁止键盘输入
        /// </summary>
        public static bool DisableKeyboardInput(DateTime time)
        {
            var ts = time.Subtract(_dt);
            if (ts.TotalMilliseconds > 200)
            {
                _dt = time; 
                return true;
            }
            return false;
        }
    }
}
