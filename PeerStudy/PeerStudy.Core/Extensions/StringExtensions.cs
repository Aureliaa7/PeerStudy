using System.Text.RegularExpressions;

namespace PeerStudy.Core.Extensions
{
    public static class StringExtensions
    {
        public static string SplitByUpperLetterAndJoinBySpace(this string words)
        {
            var splitByUpperCase = @"(?<!^)(?=[A-Z])";
            string[] result = Regex.Split(words, splitByUpperCase, RegexOptions.ExplicitCapture);

            return string.Join(" ", result);
        }
    }
}
