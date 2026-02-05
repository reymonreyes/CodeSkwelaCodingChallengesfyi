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
            //var tempLines = System.IO.File.ReadAllLines(_file, Encoding.UTF8);
            //var lines = new List<string>(tempLines);

            //var ligatures = lines.Where(x => x.Contains("outman")).ToList();

            ////clean up
            //lines.RemoveAll(x => x == string.Empty);

            ////convert each string line into a number value
            //var lineNumberValues = new int[lines.Count];

            //for (int i = 0; i < lines.Count; i++)
            //{
            //    lineNumberValues[i] = lines[i].Select(x => x).Sum(x => x);
            //}

            //var largest = lineNumberValues.Max();
            //var indexOfLargest = Array.IndexOf(lineNumberValues, largest);

            var words = new string[] { "delta", "alpha", "golf", "foxtrot", "november" };
            var buckets = new int[10][];
            var wordValues = words.Select(x => x.Select(y => y).Sum(y => y)).ToArray().Select(x => x.ToString()).ToArray();//new string[] { "33", "45", "40", "25", "17", "24" };//
            var longestNumber = wordValues.Max(x => x.Length);
            var sorted = new int[wordValues.Length];

            for (int i = longestNumber - 1; i >= 0; i--)
            {
                for (int j = 0;j < wordValues.Length; j++)
                {
                    var digit = int.Parse(new ReadOnlySpan<char>(wordValues[j][i]));
                    var alpha = buckets[digit];

                    var newValue = new int[] { int.Parse(wordValues[j]) };

                    if (buckets[digit] == null)
                        buckets[digit] = newValue;
                    else
                    {
                        var newValues = new int[buckets[digit].Length + newValue.Length];
                        for (int a = 0; a < buckets[digit].Length; a++)
                            newValues[a] = buckets[digit][a];

                        newValues[newValues.Length - 1] = newValue[0];
                        buckets[digit] = newValues;
                    }
                }

                var sortedIndex = 0;

                for (int j = 0; j < buckets.Length; j++)
                {
                    if (buckets[j] != null)
                    {
                        for (int a = 0; a < buckets[j].Length; a++)
                        {
                            sorted[sortedIndex] = buckets[j][a];
                            sortedIndex++;
                        }
                    }
                }

                wordValues = sorted.Select(x => x.ToString()).ToArray();
                buckets = new int[10][];
            }

            return new string[0];
        }
    }
}
