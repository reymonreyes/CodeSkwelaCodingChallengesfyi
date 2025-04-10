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

            Assert.Contains(results, x => x.Character == 'a' && x.Weight == 6);
            Assert.Contains(results, x => x.Character == 's' && x.Weight == 9);
            Assert.Contains(results, x => x.Character == 'h' && x.Weight == 3);
            Assert.True(results.FirstOrDefault()?.Character == 'h');
            Assert.True(results.LastOrDefault()?.Character == 'g');
        }
        
        [Fact]
        public void BuildBinaryTree_ShouldReturnCorrectRoot()
        {
            var compression = new CCCompressionTool();
            var text = "aaaaaasssssssssdddddddddddffffffffffffffffffggggggggggggggggggggghhh";
            var frequencyTable = compression.BuildFrequencyTable(text);
            var result = compression.BuildBinaryTree(frequencyTable);

            Assert.Equal(68, result.Weight);
        }    
    }
}
