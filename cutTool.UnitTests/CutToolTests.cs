using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cutTool.UnitTests
{
    public class CutToolTests
    {
        [Fact]
        public void CutField_ShouldReturnValuesOfSpecifiedField()
        {
            string[,] expected = { { "f1" }, { "1" }, { "6" }, { "11" }, { "16" }, { "21" } };
            var cutTool = new Cut();

            var result = cutTool.Run(["-f2"], "./challenge-cut/sample.tsv");
            Assert.Equal(expected, result);

            string[,] expected2 = { { "f0" }, { "0" }, { "5" }, { "10" }, { "15" }, { "20" } };
            var result2 = cutTool.Run(["-f1"], "./challenge-cut/sample.tsv");
            Assert.Equal(expected2, result2);
        }

        [Fact]
        public void CutWithDelimiter_ShouldReturnValuesFromSpecifiedDelimiter()
        {
            string[,] expected = { { "Song title" }, { "\"10000 Reasons (Bless the Lord)\"" }, { "\"20 Good Reasons\"" }, { "\"Adore You\"" }, {"\"Africa\"" } };
            var cutTool = new Cut();
            var runResult = cutTool.Run(["-f1", "-d,"], "./challenge-cut/fourchords.csv");
            string[,] result = new string[5, 1];

            for (int i = 0; i < 5; i++)
                result[i, 0] = runResult[i, 0];

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CutWithMultipleFields_CommaSpecifiedFieldsShouldReturnDataFromSetFields()
        {
            string[,] expected = {
                {"f0", "f1"},
                {"0", "1"},
                {"5", "6"},
                {"10", "11"},
                {"15", "16"},
                {"20", "21"},
            };

            var cutTool = new Cut();
            var result = cutTool.Run(["-f1,2"], "./challenge-cut/sample.tsv");

            Assert.Equal(expected, result);
        }

        [Fact]
        public void CutWithMultipleFields_WhitespaceSpecifiedFieldsShouldReturnDataFromSetFields()
        {
            string[,] expected = {
                {"f0", "f1"},
                {"0", "1"},
                {"5", "6"},
                {"10", "11"},
                {"15", "16"},
                {"20", "21"},
            };

            var cutTool = new Cut();
            var result = cutTool.Run(["-f", "\"1 2\""], "./challenge-cut/sample.tsv");

            Assert.Equal(expected, result);
        }
    }
}