using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Anye.Soft.Common
{
    public static class MyString
    {

        #region 类型转换

        public static int[]ToInts(this string[] strlist)
        {
            int[] data =Array.ConvertAll(strlist,int.Parse);
            return data;
        }

        public static int ToInt(this string s)
        {
            int data;
            int.TryParse(s, out data);
            return data;
        }

        public static int ToInt(this string s, int defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                int data = int.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static long ToLong(this string s)
        {
            long data;
            long.TryParse(s, out data);
            return data;
        }

        public static TimeSpan ToTimeSpan(this string s)
        {
            TimeSpan data;
            TimeSpan.TryParse(s, out data);
            return data;
        }

        public static long ToLong(this string s, long defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                long data = long.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static decimal ToDecimal(this string s)
        {
            decimal data;
            decimal.TryParse(s, out data);
            return data;
        }

        public static decimal ToDecimal(this string s, int dotNum)
        {
            decimal data;
            decimal.TryParse(s, out data);

            decimal sp = Convert.ToDecimal(Math.Pow(10, dotNum));
            data = Math.Truncate(data) + Math.Floor((data - Math.Truncate(data)) * sp) / sp;

            return data;
        }

        public static decimal ToDecimal(this string s, decimal defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                decimal data = decimal.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static decimal ToDecimal(this string s, decimal defval, int dotNum)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                decimal data = decimal.Parse(s);

                decimal sp = Convert.ToDecimal(Math.Pow(10, dotNum));
                data = Math.Truncate(data) + Math.Floor((data - Math.Truncate(data)) * sp) / sp;

                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static DateTime ToDateTime(this string s)
        {
            DateTime data;
            DateTime.TryParse(s, out data);
            return data;

        }

        public static DateTime ToDateTime(this string s, DateTime defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                DateTime data = DateTime.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static bool ToBool(this string s)
        {
            bool data;
            bool.TryParse(s, out data);
            return data;
        }

        public static bool ToBool(this string s, bool defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                bool data = bool.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static uint ToUint(this string s)
        {
            uint data;
            uint.TryParse(s, out data);
            return data;
        }

        public static uint ToUint(this string s, uint defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                uint data = uint.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static ulong ToUlong(this string s)
        {
            ulong data;
            ulong.TryParse(s, out data);
            return data;
        }

        public static ulong ToUlong(this string s, ulong defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                ulong data = ulong.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static short ToShort(this string s)
        {
            short data;
            short.TryParse(s, out data);
            return data;
        }

        public static short ToShort(this string s, short defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                short data = short.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static float ToFloat(this string s)
        {
            float data;
            float.TryParse(s, out data);
            return data;
        }

        public static float ToFloat(this string s, float defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                float data = float.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static double ToDouble(this string s)
        {
            double data;
            double.TryParse(s, out data);
            return data;
        }

        public static double ToDouble(this string s, double defval)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return defval; }
                double data = double.Parse(s);
                return data;
            }
            catch
            {
                return defval;
            }
        }


        public static short ToShort(this int? s)
        {
            if (s == null) { return default(short); }
            short data = (short)s;
            return data;
        }

        public static short ToShort(this int? s, short defval)
        {
            try
            {
                if (s == null) { return defval; }
                short data = (short)s;
                return data;
            }
            catch
            {
                return defval;
            }
        }

        public static double ToDouble(this decimal? s)
        {
            if (s == null) { return default(double); }
            double data = (double)s;
            return data;
        }

        public static double ToDouble(this decimal? s, double defval)
        {
            try
            {
                if (s == null) { return defval; }
                double data = (double)s;
                return data;
            }
            catch
            {
                return defval;
            }
        }

        /// <summary>
        /// 第一个字母小写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToCamel(this string s)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) { return default(string); }
                if (s.Length <= 0) { return default(string); }
                return s[0].ToString().ToLower() + s.Substring(1);
            }
            catch
            {
                return default(string);
            }
        }
        /// <summary>
        /// 第一个字母大写
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToPascal(this string s)
        {
            try
            {
                if (s.IsNullOrEmptyOrSpace()) return default(string);
                if (s.Length <= 0) { return default(string); }
                return s[0].ToString().ToUpper() + s.Substring(1);
            }
            catch
            {
                return default(string);
            }
        }


        /// <summary>
        /// 转全角(SBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        public static string ToSBC(this string input)
        {
            try
            {
                char[] c = input.ToCharArray();
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] == 32)
                    {
                        c[i] = (char)12288;
                        continue;
                    }
                    if (c[i] < 127)
                        c[i] = (char)(c[i] + 65248);
                }
                return new string(c);
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// 转半角(DBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        public static string ToDBC(this string input)
        {
            try
            {
                char[] c = input.ToCharArray();
                for (int i = 0; i < c.Length; i++)
                {
                    if (c[i] == 12288)
                    {
                        c[i] = (char)32;
                        continue;
                    }
                    if (c[i] > 65280 && c[i] < 65375)
                        c[i] = (char)(c[i] - 65248);
                }
                return new string(c);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 如果字符串为null，返回string.Empty, 否则返回字符串本身
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetValue(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            return value;
        }

        /// <summary>
        /// 将字符串用指定的编码方式转换为指定长度的字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string value, Encoding encoding, int length)
        {
            var result = new byte[length];
            var bytes = encoding.GetBytes(value.GetValue());
            var copyLength = bytes.Length > length ? length : bytes.Length;
            Array.ConstrainedCopy(bytes, 0, result, 0, copyLength);
            return result;
        }

        /// <summary>
        /// 将字符串用指定编码方式转换为字节数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="encoding"></param>
        /// <param name="length">返回结果的长度</param>
        /// <returns></returns>
        public static byte[] GetBytes(this string value, Encoding encoding, out int length)
        {
            var result = encoding.GetBytes(value.GetValue());
            length = result.Length;
            return result;
        }

        public static string ToMinDecimal(this decimal input)
        {
            try
            {
                var temps = input.ToString().Split('.');
                if (temps.Length == 2)
                {
                    var t2 = temps[1].TrimEnd('0');
                    if (t2.Length == 0)
                    {
                        return temps[0];
                    }
                    return temps[0] + "." + t2;
                }
                return temps[0];
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region 验证操作

        /// <summary>
        /// 检查文件名(或文件夹)合法性
        /// </summary>
        /// <param name="strln"></param>
        /// <returns></returns>
        public static bool IsFileName(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                if (strln.Length == 0 || strln.Length > 255) { return false; }

                var first_black_list = new char[] { '+', '-', '.' };
                if (first_black_list.Contains(strln[0]))
                {
                    return false;
                }
                var black_list = new char[] {'/', '\t', '\b', '@', '#', '$', '%', '^', '&', '*', '(', ')', '[',

']','\\', ':', ',', '?', '\"', '<', '>', '|' };
                for(int i = 0; i < black_list.Length; i++)
                {
                    if (strln.Contains(black_list[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断字符串是否是字符开头为a-zA-Z_中文,其他字符是否是字母数字下划线中文（1-50位范围内）
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsUserName(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^[a-zA-Z_\u4e00-\u9fa5][a-zA-Z0-9_\u4e00-\u9fa5]{0,49}$");
            }
            catch
            {
                return false;
            }
        }
        //中英文数字
        public static bool IsTypeName(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^[A-Za-z0-9\u4e00-\u9fa5]+$");
            }
            catch
            {
                return false;
            }
        }

        public static bool IsName(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^[A-Za-z0-9]+$");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断字符串是否是a-zA-Z0-9_范围内（1-50位范围内）
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsTableName(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^[a-zA-Z_\u4e00-\u9fa5][a-zA-Z0-9_\u4e00-\u9fa5]{0,49}$");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断字符串是否是a-zA-Z0-9特殊字符
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsPassName(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^[0-9a-zA-Z`!@#\$%\^&\*\(\)~\-\=_\+\[\]\{\};'\\:"" \|<>\?,\./]+$");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证邮箱
        /// </summary>
        /// <param name="strIn"></param>
        /// <returns></returns>
        public static bool IsValidEmail(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            }
            catch
            {
                return false;
            }
        }

        public static bool IsUrl(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                string pattern = @"[a-zA-z]+://[^\s]*";
                Regex re = new Regex(pattern, RegexOptions.IgnoreCase);
                return re.IsMatch(strln);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证输入字符串为数字
        /// </summary>
        /// <param name="strln">输入字符</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsNumber(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^(-?\d+)(\.\d+)?$");
            }
            catch
            {
                return false;
            }
        }

        public static bool IsInteger(this string strln)
        {
            try
            {
                string pattern = @"^[0-9]*[0-9][0-9]*$";
                return Regex.IsMatch(strln, pattern);
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 判断用户输入是否为日期
        /// </summary>
        /// <param name="strln"></param>
        /// <returns></returns>
        /// <remarks>
        /// 可判断格式如下（其中-可替换为/，不影响验证)
        /// YYYY | YYYY-MM | YYYY-MM-DD | YYYY-MM-DD HH:MM:SS | YYYY-MM-DD HH:MM:SS.FFF
        /// </remarks>
        public static bool IsDateTime(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                string regexDate = @"[1-2]{1}[0-9]{3}((-|\/|\.){1}(([0]?[1-9]{1})|(1[0-2]{1}))((-|\/|\.){1}((([0]?[1-9]{1})|([1-2]{1}[0-9]{1})|(3[0-1]{1})))( (([0-1]{1}[0-9]{1})|2[0-3]{1}):([0-5]{1}[0-9]{1}):([0-5]{1}[0-9]{1})(\.[0-9]{3})?)?)?)?$";
                if (Regex.IsMatch(strln, regexDate))
                {
                    //以下各月份日期验证，保证验证的完整性
                    int _IndexY = -1;
                    int _IndexM = -1;
                    int _IndexD = -1;
                    if (-1 != (_IndexY = strln.IndexOf("-")))
                    {
                        _IndexM = strln.IndexOf("-", _IndexY + 1);
                        _IndexD = strln.IndexOf(":");
                    }
                    else
                    {
                        _IndexY = strln.IndexOf("/");
                        _IndexM = strln.IndexOf("/", _IndexY + 1);
                        _IndexD = strln.IndexOf(":");
                    }
                    //不包含日期部分，直接返回true
                    if (-1 == _IndexM)
                        return true;
                    if (-1 == _IndexD)
                    {
                        _IndexD = strln.Length + 3;
                    }
                    int iYear = Convert.ToInt32(strln.Substring(0, _IndexY));
                    int iMonth = Convert.ToInt32(strln.Substring(_IndexY + 1, _IndexM - _IndexY - 1));
                    int iDate = Convert.ToInt32(strln.Substring(_IndexM + 1, _IndexD - _IndexM - 4));
                    //判断月份日期
                    if ((iMonth < 8 && 1 == iMonth % 2) || (iMonth > 8 && 0 == iMonth % 2))
                    {
                        if (iDate < 32)
                            return true;
                    }
                    else
                    {
                        if (iMonth != 2)
                        {
                            if (iDate < 31)
                                return true;
                        }
                        else
                        {
                            //闰年
                            if ((0 == iYear % 400) || (0 == iYear % 4 && 0 < iYear % 100))
                            {
                                if (iDate < 30)
                                    return true;
                            }
                            else
                            {
                                if (iDate < 29)
                                    return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 验证输入字符串为11位的手机号码
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsMobile(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^1[0123456789]\d{9}$", RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }



        /// <summary>
        /// 验证身份证是否有效
        /// </summary>
        /// <param name="strln"></param>
        /// <returns></returns>
        public static bool IsIDCard(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                if (strln.Length == 18)
                {
                    bool check = IsIDCard18(strln);
                    return check;
                }
                else if (strln.Length == 15)
                {
                    bool check = IsIDCard15(strln);
                    return check;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证输入字符串为18位的身份证号码
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        static bool IsIDCard18(string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                long n = 0;
                if (long.TryParse(strln.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(strln.Replace('x', '0').Replace('X', '0'), out n) == false)
                {
                    return false;//数字验证
                }
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(strln.Remove(2)) == -1)
                {
                    return false;//省份验证
                }
                string birth = strln.Substring(6, 8).Insert(6, "-").Insert(4, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;//生日验证
                }
                string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
                string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
                char[] Ai = strln.Remove(17).ToCharArray();
                int sum = 0;
                for (int i = 0; i < 17; i++)
                {
                    sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
                }
                int y = -1;
                Math.DivRem(sum, 11, out y);
                if (arrVarifyCode[y] != strln.Substring(17, 1).ToLower())
                {
                    return false;//校验码验证
                }
                return true;//符合GB11643-1999标准
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 验证输入字符串为15位的身份证号码
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        static bool IsIDCard15(string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                long n = 0;
                if (long.TryParse(strln, out n) == false || n < Math.Pow(10, 14))
                {
                    return false;//数字验证
                }
                string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
                if (address.IndexOf(strln.Remove(2)) == -1)
                {
                    return false;//省份验证
                }
                string birth = strln.Substring(6, 6).Insert(4, "-").Insert(2, "-");
                DateTime time = new DateTime();
                if (DateTime.TryParse(birth, out time) == false)
                {
                    return false;//生日验证
                }
                return true;//符合15位身份证标准
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 验证输入字符串为电话号码
        /// </summary>
        /// <param name="P_str_phone">输入字符串</param>
        /// <returns>返回一个bool类型的值</returns>
        public static bool IsPhone(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"(^(\d{2,4}[-_－—]?)?\d{3,8}([-_－—]?\d{3,8})?([-_－—]?\d{1,7})?$)|(^0?1[35]\d{9}$)");
                //弱一点的验证：  @"\d{3,4}-\d{7,8}"
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 验证是否是有效邮箱地址
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsEmail(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 验证是否是有效传真号码
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsFax(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$");
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 验证是否只含有汉字
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsOnllyChinese(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^[\u4e00-\u9fa5]+$");
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 检查字符串是否为[null]或[空]或[空白]
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrSpace(this string s)
        {
            try
            {
                return string.IsNullOrWhiteSpace(s);
            }
            catch
            {
                return false;
            }
        }

        public static bool IsJobNo(this string strln)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(strln)) { return false; }
                return Regex.IsMatch(strln, @"^[0-9][0-9]{3,49}$");
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 金额验证
        /// </summary>
        /// <param name="moneycheck"></param>
        /// <returns></returns>
        public static bool IsMoney(this string moneycheck)
        {
            string monval = @"^([1-9]\d+|[0-9])(\.\d\d?)*$";
            return Regex.IsMatch(moneycheck, monval);
        }

        #endregion

        #region 其他
        public static bool ExIsMatch(this string s, string pattern)
        {
            try
            {
                if (s == null && pattern != null) return false;
                if (s != null && pattern == null) return false;
                if (s == null && pattern == null) return true;
                return System.Text.RegularExpressions.Regex.IsMatch(s, pattern);
            }
            catch
            {
                return false;
            }
        }

        public static string ExMatch(this string s, string pattern)
        {
            try
            {
                if (s == null) return "";
                if (pattern == null) return "";
                return System.Text.RegularExpressions.Regex.Match(s, pattern).Value;
            }
            catch
            {
                return null;
            }
        }

        public static string ToMd5(this string s)
        {
            Encoding encoding = Encoding.UTF8;
            var bytes = encoding.GetBytes(s);
            var md5Bytes = Encrypt(bytes);
            var md5 = "";
            foreach (var b in md5Bytes)
            {
                md5 += b.ToString("X2");
            }

            return md5;
        }
        static byte[] Encrypt(byte[] Source)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Source);
            return result;
        }
        #endregion

    }
}
