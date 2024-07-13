using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Chen.Commons
{
    /// <summary>
    /// 正则表达式帮助类
    /// </summary>
    public static class RegexHelper
    {

        // 定义邮箱地址的正则表达式
        public const string MailRegex = @"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
        public const string PhoneNumberRegex = @"^1[3-9]\d{9}$"; // 手机号码的正则表达式模式

        /// <summary>
        /// 判断字符串是否为合法的手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(string input)
        {

            Regex regex = new Regex(PhoneNumberRegex);

            return regex.IsMatch(input);
        }

        /// <summary>
        /// 验证是否为合法的邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {

            // 使用正则表达式检验邮箱格式
            return Regex.IsMatch(email, MailRegex);
        }
    }
}
