using IdGen;
using System;

namespace Mook.Util
{
    /// <summary>
    /// ID生成器
    /// </summary>
    public class IdGeneratorHelper
    {
        /// <summary>
        /// 雪花算法ID
        /// https://github.com/RobThree/IdGen
        /// </summary>
        /// <param name="workId">机器编号</param>
        /// <returns></returns>
        public static long SnowflakeId(int workId)
        {
            var options = new IdGeneratorOptions(null, null, SequenceOverflowStrategy.SpinWait);
            var generator = new IdGenerator(workId, options);
            return generator.CreateId();
        }

        /// <summary>
        /// 去掉‘-’的GUID
        /// </summary>
        /// <returns></returns>
        public static string GuidStr()
        {
            return Guid.NewGuid().ToString("N");
        }

        /// <summary>
        /// GUID转换成19位数字
        /// </summary>
        /// <returns></returns>
        public static long GuidNum()
        {
            return BitConverter.ToInt64(Guid.NewGuid().ToByteArray(), 0);
        }
    }
}
