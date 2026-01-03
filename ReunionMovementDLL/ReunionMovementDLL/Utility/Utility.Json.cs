using System;

namespace ReunionMovementDLL
{
    public static partial class Utility
    {
        /// <summary>
        /// JSON 相关的实用函数。
        /// </summary>
        public static partial class Json
        {
            private static IJsonHelper jsonHelper = null;

            /// <summary>
            /// 设置 JSON 辅助器。
            /// </summary>
            /// <param name="jsonHelper">要设置的 JSON 辅助器。</param>
            public static void SetJsonHelper(IJsonHelper jsonHelper)
            {
                Json.jsonHelper = jsonHelper;
            }

            /// <summary>
            /// 将对象序列化为 JSON 字符串。
            /// </summary>
            /// <param name="obj">要序列化的对象。</param>
            /// <returns>序列化后的 JSON 字符串。</returns>
            public static string ToJson(object obj)
            {
                if (jsonHelper == null)
                {
                    throw new ReunionMovementException("JSON 辅助器无效。");
                }

                try
                {
                    return jsonHelper.ToJson(obj);
                }
                catch (Exception exception)
                {
                    if (exception is ReunionMovementException)
                    {
                        throw;
                    }

                    throw new ReunionMovementException(Text.Format("转换为 JSON 时发生异常：'{0}'。", exception), exception);
                }
            }

            /// <summary>
            /// 将 JSON 字符串反序列化为对象。
            /// </summary>
            /// <typeparam name="T">对象类型。</typeparam>
            /// <param name="json">要反序列化的 JSON 字符串。</param>
            /// <returns>反序列化后的对象。</returns>
            public static T ToObject<T>(string json)
            {
                if (jsonHelper == null)
                {
                    throw new ReunionMovementException("JSON 辅助器无效。");
                }

                try
                {
                    return jsonHelper.ToObject<T>(json);
                }
                catch (Exception exception)
                {
                    if (exception is ReunionMovementException)
                    {
                        throw;
                    }

                    throw new ReunionMovementException(Text.Format("转换为对象时发生异常：'{0}'。", exception), exception);
                }
            }

            /// <summary>
            /// 将 JSON 字符串反序列化为对象。
            /// </summary>
            /// <param name="objectType">对象类型。</param>
            /// <param name="json">要反序列化的 JSON 字符串。</param>
            /// <returns>反序列化后的对象。</returns>
            public static object ToObject(Type objectType, string json)
            {
                if (jsonHelper == null)
                {
                    throw new ReunionMovementException("JSON 辅助器无效。");
                }

                if (objectType == null)
                {
                    throw new ReunionMovementException("对象类型无效。");
                }

                try
                {
                    return jsonHelper.ToObject(objectType, json);
                }
                catch (Exception exception)
                {
                    if (exception is ReunionMovementException)
                    {
                        throw;
                    }

                    throw new ReunionMovementException(Text.Format("转换为对象时发生异常：'{0}'。", exception), exception);
                }
            }
        }
    }
}
