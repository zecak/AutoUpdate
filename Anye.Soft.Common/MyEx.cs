using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anye.Soft.Common
{
    public static class MyEx
    {
        public static string MD5FileForBytes(this byte[] filebytes)
        {
            byte[] file = filebytes;
            byte[] b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(file);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("X").PadLeft(2, '0');
            return ret;
        }

        public static string MD5File(this string fileName)
        {
            if (!File.Exists(fileName)) { throw new Exception("文件不存在"); }
            byte[] file = File.ReadAllBytes(fileName);
            byte[] b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(file);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("X").PadLeft(2, '0');
            return ret;
        }

        public static string MD5(this string content)
        {
            byte[] b = Encoding.Default.GetBytes(content);
            b = new System.Security.Cryptography.MD5CryptoServiceProvider().ComputeHash(b);
            string ret = "";
            for (int i = 0; i < b.Length; i++)
                ret += b[i].ToString("X").PadLeft(2, '0');
            return ret;
        }

        public static string DateTimeFormat { get; set; } = "yyyy/MM/dd HH:mm:ss.fffffff";
        public static string ToJson(this object obj, bool isFormatting = false)
        {
            try
            {
                var datetimeFormat = DateTimeFormat;
                var dtc = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                dtc.DateTimeFormat = datetimeFormat;
                //日期和间都管用  
                JsonSerializerSettings jsSettings = new JsonSerializerSettings();

                jsSettings.TypeNameHandling = TypeNameHandling.Auto;

                jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jsSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                jsSettings.NullValueHandling = NullValueHandling.Ignore;
                if (isFormatting)
                {
                    jsSettings.Formatting = Formatting.Indented;
                }
                jsSettings.Converters.Add(dtc);
                return JsonConvert.SerializeObject(obj, jsSettings);
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        public static T JsonTo<T>(this string json)
        {
            try
            {

                var datetimeFormat = DateTimeFormat;
                var dtc = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                dtc.DateTimeFormat = datetimeFormat;


                //日期和间都管用  
                JsonSerializerSettings jsSettings = new JsonSerializerSettings();

                jsSettings.TypeNameHandling = TypeNameHandling.Auto;

                jsSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                jsSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
                jsSettings.NullValueHandling = NullValueHandling.Ignore;
                //jsSettings.Formatting = Formatting.Indented;
                jsSettings.Converters.Add(dtc);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json, jsSettings);
            }
            catch (Exception ex)
            {
                return default(T);
            }

        }

        public static string ToBase64Encode(this string data)
        {
            try
            {
                if (data == null) { return ""; }
                byte[] b = System.Text.Encoding.UTF8.GetBytes(data);
                return Convert.ToBase64String(b);
            }
            catch (Exception ex)
            {
                return data;
            }

        }

        public static string ToBase64Decode(this string result)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(result);
                return System.Text.Encoding.UTF8.GetString(bytes);

            }
            catch (Exception ex)
            {
                return result;
            }

        }

        /// <summary>本地时间转换成UTC时间</summary>
        /// <param name="vDate">待转换的时间</param>
        /// <param name="Milliseconds">是否精确到毫秒</param>
        /// <returns>UTC时间</returns>
        public static long ToTimestamp(this DateTime vDate, bool Milliseconds = true)
        {
            vDate = vDate.ToUniversalTime();
            var dtZone = new DateTime(1970, 1, 1, 0, 0, 0);
            if (Milliseconds)
            {
                return (long)vDate.Subtract(dtZone).TotalMilliseconds;
            }
            return (long)vDate.Subtract(dtZone).TotalSeconds;
        }

        public static DateTime ToDateTime(this long timestamp, bool milliseconds = true)
        {

            System.DateTime startTime = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1, 0, 0, 0), TimeZoneInfo.Local);
            if (milliseconds)
            {
                return startTime.AddMilliseconds(timestamp);
            }
            else
            {
                return startTime.AddSeconds(timestamp);
            }
        }
    }
}
