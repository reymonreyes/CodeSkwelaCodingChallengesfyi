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
        public void BuildFrequencyTable_ShouldReturnCorrectCounts()
        {
            var compression = new CCCompressionTool();
            var text = "aaaaaasssssssssdddddddddddffffffffffffffffffggggggggggggggggggggghhh";

            var results = compression.BuildFrequencyTable(text);

            Assert.Contains(results, x => x.Key == 'a' && x.Value == 6);
            Assert.Contains(results, x => x.Key == 's' && x.Value == 9);
            Assert.Contains(results, x => x.Key == 'h' && x.Value == 3);
            Assert.True(results.FirstOrDefault().Key == 'h');
            Assert.True(results.LastOrDefault().Key == 'g');
        }        
    }
}
