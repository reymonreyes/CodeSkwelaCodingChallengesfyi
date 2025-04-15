using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTool.UnitTests
{
    public class CompressionToolTests
    {
        private string text = "aaaaaasssssssssdddddddddddffffffffffffffffffggggggggggggggggggggghhh";
        [Fact]
        public void BuildFrequencyTable_ShouldReturnCorrectCounts()
        {
            var compression = new CCCompressionTool();
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
            var frequencyTable = compression.BuildFrequencyTable(text);
            var result = compression.BuildBinaryTree(frequencyTable);

            Assert.Equal(68, result.Weight);
        }

        [Fact]
        public void AssignCodes_ShouldReturnCorrectCodes()
        {
            var compression = new CCCompressionTool();
            var frequencyTable = compression.BuildFrequencyTable(text);
            var tree = compression.BuildBinaryTree(frequencyTable);
            var codes = compression.AssignCodes(tree);

            Assert.Contains(codes, x => x.Character == 'h' && x.Code == "1010");
        }

        [Fact]
        public void WriteFileHeader_ShouldOutputFrequencyTable()
        {
            var compression = new CCCompressionTool();
            var frequencyTable = compression.BuildFrequencyTable(text);
            var tree = compression.BuildBinaryTree(frequencyTable);
            var codes = compression.AssignCodes(tree);
            string header = compression.GenerateHeader(codes);

            Assert.Contains("#HEADERSTART#", header);
            Assert.Contains("h-3,a-6,s-9,d-11,f-18,g-21", header);
            Assert.Contains("#HEADEREND#", header);
        }
    }
}
