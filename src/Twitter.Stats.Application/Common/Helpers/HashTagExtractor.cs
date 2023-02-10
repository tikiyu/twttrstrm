using System.Text;

namespace Twitter.Stats.Application.Common.Helpers
{
    public static class HashtagExtractor
    {
        public static HashSet<string> ExtractHashtags(this string text)
        {
            int startIndex = -1;
            int length = text.Length;

            HashSet<string> hashtags = new();
            StringBuilder sb = new();

            for (int i = 0; i < length; i++)
            {
                char currentChar = text[i];
                if (currentChar == '#')
                {
                    startIndex = i;
                    sb.Clear();
                }
                else if (startIndex != -1)
                {
                    if (!char.IsWhiteSpace(currentChar))
                    {
                        sb.Append(currentChar);
                    }
                    else
                    {
                        if (sb.Length > 0)
                        {
                            hashtags.Add(sb.ToString());
                            sb.Clear();
                        }
                        startIndex = -1;
                    }
                }
            }

            if (startIndex != -1 && sb.Length > 0)
            {
                hashtags.Add(sb.ToString());
            }

            return hashtags;
        }
    }
}
