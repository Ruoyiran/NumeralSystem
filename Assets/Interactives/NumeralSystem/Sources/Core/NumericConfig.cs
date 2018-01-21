using System.Collections.Generic;
namespace NumeralSystem
{
    public class NumericConfig
    {
        public const int kScreenWith = 1280;
        public const int kScreenHeight = 720;

        private static readonly Dictionary<int, string> ChineseNumbersDict = new Dictionary<int, string>()
        {
            { 1, "一" },
            { 2, "二" },
            { 3, "三" },
            { 4, "四" },
            { 5, "五" },
            { 6, "六" },
            { 7, "七" },
            { 8, "八" },
            { 9, "九" },
            { 10, "十" },
            { 11, "十一" },
            { 12, "十二" },
            { 13, "十三" },
            { 14, "十四" },
            { 15, "十五" },
            { 16, "十六" },
        };

        public static string GetChineseNumber(int number)
        {
            if (ChineseNumbersDict.ContainsKey(number))
                return ChineseNumbersDict[number];
            return "";
        }
    }
}