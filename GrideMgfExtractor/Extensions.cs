namespace GrideMgfExtractor
{
    internal static class Extensions
    {
        internal static string Between(this string str, string start, string end)
        {
            int startIndex = str.IndexOf(start) + start.Length;
            int endIndex = str.LastIndexOf(end);

            if (startIndex == -1)
                startIndex = 0;
            if (endIndex == -1 || endIndex <= startIndex)
                endIndex = str.Length;

            return str.Substring(startIndex, endIndex - startIndex);
        }

        internal static string Substring(this string str, int startIndex, string end)
        {
            int endIndex = str.LastIndexOf(end);

            if (startIndex < 0)
                startIndex = 0;
            if (endIndex == -1 || endIndex <= startIndex)
                endIndex = str.Length;

            return str.Substring(startIndex, endIndex - startIndex);
        }
    }
}
