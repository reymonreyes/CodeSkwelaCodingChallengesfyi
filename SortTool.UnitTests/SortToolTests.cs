using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortTool.UnitTests
{
    public class SortToolTests
    {
        [Fact]
        public void Sort_ShouldSortLexicographically()
        {
            var expected = new string[]{ "A", "ACTUAL", "AGREE", "AGREEMENT", "AND" };
            var sort = new Sort("words.txt");
            
            var result = sort.Run(new string[0]);
            result = result.Distinct().Take(5).ToArray();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Sort_UniqueOptionShouldSortLexicographically()
        {
            var expected = new string[] { "A", "ACTUAL", "AGREE", "AGREEMENT", "AND" };
            var sort = new Sort("words.txt");

            var result = sort.Run(new string[] { "-u" });
            result = result.Take(5).ToArray();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void RadixSort_ShouldSort()
        {
            var expected = new string[] { "A", "B", "C", "D", "E" };
            var sort = new Sort("words.txt");

            var result = sort.RunRadixSort();

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Dummy()
        {
            var random = new Random();
            random.Next(0, 1000);
            var numbers = new int[10];
            for (int i = 0; i < 10; i++)
            {
                numbers[i] = random.Next(0, 1000);
            }

            var max = numbers.Max();
            var min = numbers.Min();
            var mid = (max - min) / 2 + min;
            var splitBucket = new int[,] { { min, mid }, { mid + 1, max } };
            var buckets = new int[2][];

            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = 0; j < splitBucket.Length; j++)
                {
                    if(numbers[i] >= splitBucket[j,0] && numbers[i] <= splitBucket[j, 1])
                    {
                        if (buckets[j] == null)
                        {
                            buckets[j] = new int[] { numbers[i] };
                        }
                        else
                        {
                            var newValues = new int[buckets[j].Length + 1];
                            for (int a = 0; a < buckets[j].Length; a++)
                                newValues[a] = buckets[j][a];

                            newValues[newValues.Length - 1] = numbers[i];
                            buckets[j] = newValues;
                        }
                        break;
                    }
                }
            }

            var numberIndex = 0;
            for (int i = 0; i < buckets.Length; i++)
            {
                if (buckets[i] != null)
                {
                    for(int j = 0;j < buckets[i].Length; j++)
                    {
                        numbers[numberIndex] = buckets[i][j];
                        numberIndex++;
                    }
                }
            }
        }
    }
}
