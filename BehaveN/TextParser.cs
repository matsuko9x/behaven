using System.Collections.Generic;
using System.IO;

namespace BehaveN
{
    internal class TextParser
    {
        internal static List<string> GetLines(string text)
        {
            List<string> lines = new List<string>();

            using (StringReader reader = new StringReader(text))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (LineIsNotEmptyAndNotAComment(line))
                    {
                        lines.Add(line);
                    }
                }
            }

            return lines;
        }

        private static bool LineIsNotEmptyAndNotAComment(string line)
        {
            return line != "" && !line.StartsWith("#");
        }
    }
}
