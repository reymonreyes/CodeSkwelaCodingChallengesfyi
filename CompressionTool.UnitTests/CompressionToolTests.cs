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
            Assert.True(results.FirstOrDefault()?.Character == 'a');
            Assert.True(results.LastOrDefault()?.Character == 'h');
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
            string header = compression.GenerateHeader(frequencyTable);

            Assert.Contains("#HEADERSTART#", header);
            //Assert.Contains("h-3,a-6,s-9,d-11,f-18,g-21", header);
            Assert.Contains("a-6,s-9,d-11,f-18,g-21,h-3", header);
            Assert.Contains("#HEADEREND#", header);
        }

        [Fact]
        public void Encode_ShouldOutputCorrectEncodedValue()
        {
            var compression = new CCCompressionTool();
            var encoded = compression.Encode("test");

            Assert.Contains("t-2,e-1,s-1", encoded);
            Assert.Contains("010110", encoded);
        }

        [Fact]
        public void Decode_ShouldOutputCorrectDecodedValue()
        {
            var compression = new CCCompressionTool();
            var encoded = compression.Encode(text);
            var decoded = compression.Decode(encoded);

            Assert.Equal(text, decoded);
        }
    }
}
