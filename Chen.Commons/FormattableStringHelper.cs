using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chen.Commons
{
    public static class FormattableStringHelper
    {
        // 静态方法，用于构建 URL
        public static string BuildUrl(FormattableString urlFormat)
        {
            // 从格式化字符串中提取参数列表
            var invariantParameters = urlFormat.GetArguments()
                // 将每个参数转换为不受区域设置影响的 不可变的字符串，确保生成的 URL 是不可变的
                .Select(s => FormattableString.Invariant($"{s}"));

            // 使用 URI 编码对参数进行处理，以确保它们在 URL 中是安全的
            object[] escapedParameters = invariantParameters
                .Select(s => (object)Uri.EscapeDataString(s)).ToArray();

            // 使用格式字符串和已编码的参数数组组合成最终的 URL
            return string.Format(urlFormat.Format, escapedParameters);
        }
    }
}
