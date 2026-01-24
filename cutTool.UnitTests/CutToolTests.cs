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
            string[] expected = ["f1", "1", "6", "11", "16", "21"];
            var cutTool = new Cut();

            var result = cutTool.Run("f1", "./challenge-cut/sample.tsv");
            Assert.Equal(expected, result);

            string[] expected2 = ["f0", "0", "5", "10", "15", "20"];
            var result2 = cutTool.Run("f0", "./challenge-cut/sample.tsv");
            Assert.Equal(expected2, result2);
        }
    }
}
