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
    }
}
