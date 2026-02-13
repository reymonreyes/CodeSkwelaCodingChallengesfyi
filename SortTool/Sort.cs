using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortTool
{
    public class Sort
    {
        private readonly string _file;
        public Sort(string file)
        {
            _file = file;
        }
        public string[] Run(string[] options)
        {
            var tempLines = System.IO.File.ReadAllLines(_file, Encoding.UTF8);
            var lines = new List<string>(tempLines);

            //remove empty strings
            lines.RemoveAll(x => x == string.Empty);
            lines.RemoveAll(x => x == "§" || x == "§§");
            
            var result = lines.OrderBy(x => x, StringComparer.Ordinal).ToList();

            if (options.Contains("-u"))
                result = result.Distinct().ToList();

            return result.ToArray();
        }

        public string[] RunRadixSort()
        {
            
            var lines = System.IO.File.ReadAllLines(_file, Encoding.UTF8);
            var charsOnly = lines.SelectMany(x => x).Select(x => x).Distinct().OrderBy(x => x).ToArray();
            var longestWord = lines.OrderByDescending(x => x.Length).FirstOrDefault();
            var shortestWord = lines.OrderByDescending(x => x.Length).LastOrDefault();
            var buckets = new string[charsOnly.Length][];

            for (int characterPosition = longestWord.Length - 1; characterPosition >= 0; characterPosition--)
            {
                for (int wordIndex = 0; wordIndex < lines.Length; wordIndex++)
                {
                    var word = lines[wordIndex];
                    var characterIndex = characterPosition;

                    if (word.Length - 1 < characterPosition)
                    {                        
                        var positionDiff = characterPosition - word.Length;
                        characterIndex = characterPosition - positionDiff;
                        characterIndex = characterIndex < 0 ? 0 : characterIndex - 1;
                    }

                    var character = word[characterIndex];
                    var characterBucketIndex = Array.IndexOf(charsOnly, character);

                    if (buckets[characterBucketIndex] == null)
                    {
                        buckets[characterBucketIndex] = new string[] { word };
                    }
                    else
                    {
                        var newValues = new string[buckets[characterBucketIndex].Length + 1];
                        for (int a = 0; a < buckets[characterBucketIndex].Length; a++)
                            newValues[a] = buckets[characterBucketIndex][a];

                        newValues[newValues.Length - 1] = word;
                        buckets[characterBucketIndex] = newValues;
                    }
                }

                int lineIndex = 0;
                for (int bucketIndex = 0; bucketIndex < buckets.Length; bucketIndex++)
                {
                    if (buckets[bucketIndex] != null)
                    {
                        for (int elementIndex = 0; elementIndex < buckets[bucketIndex].Length; elementIndex++)
                        {
                            lines[lineIndex] = buckets[bucketIndex][elementIndex];
                            lineIndex++;
                        }
                    }
                }

                //reset the buckets
                buckets = new string[charsOnly.Length][];
            }

            //lines = lines.Distinct().ToArray();

            return lines;
        }
    }
}
