using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        public void RadixSort_ShouldSortLexicographically()
        {
            var expected = new string[] { "A", "ACTUAL", "AGREE", "AGREEMENT", "AND" };
            var sort = new Sort("words.txt");
            var result = sort.RunRadixSort();
            result = result.Distinct().Take(5).ToArray();
            Assert.Equal(expected, result);
        }

        //[Fact]
        public void Dummy3()//this is my final solution for radix sort implementation, refer to the actual test above RadixSort_ShouldSort
        {
            //another attempt, this time I created the buckets to hold each character
            //then go through each letter of the word
            var lines = System.IO.File.ReadAllLines("words.txt", Encoding.UTF8);
            lines = lines.Take(20).ToArray();
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

                    if(word.Length - 1 < characterPosition)
                    {
                        // sample case
                        //  "echo" -     0,1,2,3
                        //"deltas" - 0,1,2,3,4,5
                        var positionDiff = characterPosition - word.Length;//eg. 5 - 4 = 1
                        characterIndex = characterPosition - positionDiff;//5 - 1 = 4, 4 - 1 = 3, 3 - 1 = 2, 2 - 1 = 1, 1 - 1 = 0
                        characterIndex = characterIndex < 0 ? 0 : characterIndex - 1;//4 - 1 = 3
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
                    if(buckets[bucketIndex] != null)
                    {
                        for (int elementIndex = 0;elementIndex < buckets[bucketIndex].Length; elementIndex++)
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
        }

        //[Fact]
        public void Dummy2()//useless radix sort
        {
            //find the longest word
            //determine how many binary splits to do?
            //this binary split value will become the buckets min -> max
            var splitValue = 1000000.0;
            var originalValue = 1000000.0;
            var splitCount = 0;
            Console.WriteLine("unit test debug");
            //while (splitValue > 2)
            //{
            //    splitCount++;
            //    splitValue = Math.Ceiling(splitValue / 2.0);
            //}

            double start = 1, end = start + 15000;

            for (var i = 0; end < originalValue; i++)
            {
                Console.WriteLine($"[{i}] start: {start} end: {end}");
                start = end + 1;
                end = start + 15000;
            }
        }


        //[Fact]
        public void Dummy()//useless as I tried to make creates buckets that are partitioned like half splits
        {
            //var words = new string[] { "delta", "echo", "alpha", "bravo", "foxtrot", "sierra", "kilo" };
            var words = new string[] { "papa", "kilo", "lima", "echo" };
            var longestWord = words.Max(x => x.Length);
            var combinedWords = string.Empty;
            foreach (var word in words)
                combinedWords += word;
            var charsOnly = combinedWords.Select(x => x).Distinct().ToArray();
            var maxCharValue = (int)charsOnly.Max();
            var minCharValue = (int)charsOnly.Min();
            var mid = (maxCharValue - minCharValue) / 2 + minCharValue;
            var bucketRanges = new int[,] { { minCharValue, mid }, { mid + 1, maxCharValue } };
            var buckets = new string[2][];

            for (int charPosition = 0; charPosition < longestWord; charPosition++)
            {
                //buckets = new string[2][];
                for (int wordIndex = 0; wordIndex < words.Length; wordIndex++)
                {
                    var word = words[wordIndex];
                    
                    for (int bucketRangeIndex = 0; bucketRangeIndex < bucketRanges.Length; bucketRangeIndex++)
                    {
                        char character = char.MinValue;

                        if(charPosition > word.Length - 1)
                            character = word[word.Length - 1];
                        else
                            character = word[charPosition];

                        //if (word[charPosition] >= bucketRanges[bucketRangeIndex, 0] && word[charPosition] <= bucketRanges[bucketRangeIndex, 1])
                        if (character >= bucketRanges[bucketRangeIndex, 0] && character <= bucketRanges[bucketRangeIndex, 1])
                        {
                            if (buckets[bucketRangeIndex] == null)
                            {
                                buckets[bucketRangeIndex] = new string[] { word };
                            }
                            else
                            {
                                var newValues = new string[buckets[bucketRangeIndex].Length + 1];
                                for (int a = 0; a < buckets[bucketRangeIndex].Length; a++)
                                    newValues[a] = buckets[bucketRangeIndex][a];

                                newValues[newValues.Length - 1] = word;
                                buckets[bucketRangeIndex] = newValues;
                            }
                            break;
                        }
                    }
                }

                var sortedIndex = 0;
                //words = new string[words.Length];

                for (int j = 0; j < buckets.Length; j++)
                {
                    if (buckets[j] != null)
                    {
                        for (int a = 0; a < buckets[j].Length; a++)
                        {
                            words[sortedIndex] = buckets[j][a];
                            sortedIndex++;
                        }
                    }
                }

                buckets = new string[2][];
            }

            /*
            
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
        
            */
        }
    }
}
