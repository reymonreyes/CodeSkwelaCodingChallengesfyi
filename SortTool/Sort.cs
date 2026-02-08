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
            //now sort using strings?
            var stringsToSort = new string[] { "delta", "echo", "alpha" };
            //get values 



            var buckets = new int[10][];
            var valuesToSort = new int[] { 33, 145, 40, 25, 17, 24 };
            var result = new string[0];
            var largestNumber = valuesToSort.Max(x => x);
            var numberPlace = 1;

            for (; largestNumber / numberPlace > 0; numberPlace = numberPlace * 10)
            {
                for (int j = 0;j < valuesToSort.Length; j++)
                {
                    var value = valuesToSort[j];
                    var digit = (value / numberPlace) % 10;

                    if (buckets[digit] == null)
                        buckets[digit] = new int[] { value };
                    else
                    {
                        var newValues = new int[buckets[digit].Length + 1];
                        for (int a = 0; a < buckets[digit].Length; a++)
                            newValues[a] = buckets[digit][a];

                        newValues[newValues.Length - 1] = value;
                        buckets[digit] = newValues;
                    }
                }

                var sortedIndex = 0;
                valuesToSort = new int[valuesToSort.Length];

                for (int j = 0; j < buckets.Length; j++)
                {
                    if (buckets[j] != null)
                    {
                        for (int a = 0; a < buckets[j].Length; a++)
                        {
                            valuesToSort[sortedIndex] = buckets[j][a];
                            sortedIndex++;
                        }
                    }
                }

                buckets = new int[10][];
            }

            return new string[0];
        }
    }
}
