﻿using Fur.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fur.MirrorController.Helpers
{
    internal class Helper
    {
        #region 移除字符串前后缀 + internal static string ClearStringAffix(string str, params string[] affixs)

        /// <summary>
        /// 移除字符串前后缀
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="affixs">前后缀</param>
        /// <returns>新的字符串</returns>
        internal static string ClearStringAffix(string str, params string[] affixs)
        {
            if (!str.HasValue()) throw new ArgumentNullException(nameof(str));

            if (affixs == null || affixs.Length == 0) return str;

            var shortAffixString = affixs.OrderBy(u => u.Length).FirstOrDefault();

            bool isClearStart = false;
            bool isClearEnd = false;

            var stringBuilder = new StringBuilder();
            foreach (var affix in affixs)
            {
                if (!isClearStart && str.StartsWith(affix))
                {
                    stringBuilder.Insert(0, str.Substring(affix.Length));
                    isClearStart = true;
                }
                if (!isClearEnd && str.EndsWith(affix))
                {
                    var _tempStr = stringBuilder.Length > 0 ? stringBuilder.ToString() : str;
                    stringBuilder.Append(_tempStr.Substring(0, _tempStr.Length - affix.Length));
                    isClearEnd = true;
                }
                if (isClearStart && isClearEnd) break;
            }
            var newStr = stringBuilder.ToString();
            return newStr.HasValue() ? newStr : str;
        }

        #endregion 移除字符串前后缀 + internal static string ClearStringAffix(string str, params string[] affixs)

        #region 将字符串按照骆驼命名反射切割 + internal string[] CamelCaseSplitString(string str)

        /// <summary>
        /// 将字符串按照骆驼命名反射切割
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>切换后数组</returns>
        internal static string[] CamelCaseSplitString(string str)
        {
            if (!str.HasValue()) throw new ArgumentNullException(nameof(str));
            if (str.Length == 1) return new string[] { str };

            return Regex.Split(str, @"(?=\p{Lu}\p{Ll})|(?<=\p{Ll})(?=\p{Lu})").Where(u => u.HasValue()).ToArray();
        }

        #endregion 将字符串按照骆驼命名反射切割 + internal string[] CamelCaseSplitString(string str)

        #region 获取骆驼命名第一个单词 + internal static string GetCamelCaseFirstWord(string str)

        /// <summary>
        /// 获取骆驼命名第一个单词
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>首个单词</returns>
        internal static string GetCamelCaseFirstWord(string str)
            => CamelCaseSplitString(str).FirstOrDefault();

        #endregion 获取骆驼命名第一个单词 + internal static string GetCamelCaseFirstWord(string str)
    }
}