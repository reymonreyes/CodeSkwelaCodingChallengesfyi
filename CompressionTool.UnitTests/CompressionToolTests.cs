using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTool.UnitTests
{
    public class CompressionToolTests
    {
        [Fact]
        public void Compress_ShouldReturnCorrectCounts()
        {
            var compression = new CCCompressionTool();
            var text = "In this step your goal is to build your tool to accept a filename as input. It should return an error if the file is not valid, otherwise your program should open it, read the text and determine the frequency of each character occurring within the text.";

            var results = compression.Compress(text);

            Assert.Contains(results, x => x.Key == 'x' && x.Value == 2);
            Assert.Contains(results, x => x.Key == 'o' && x.Value == 17);
            Assert.Contains(results, x => x.Key == 'e' && x.Value == 24);
        }
    }
}
