using System.Text.RegularExpressions;

namespace Shared.Data
{
    public static class StringEx
    {
        static Regex chapterRegex = new Regex(@"(?:^\s*|^\s*第.*?)(第[^\s,.，。]*?[章篇回讲]\s?.*)");
        public static Match ExtractChapter(this string line)
        {
            return chapterRegex.Match(line);
        }
    }
}
