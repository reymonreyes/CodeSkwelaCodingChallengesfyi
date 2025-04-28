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
            Assert.Contains("97-6,115-9,100-11,102-18,103-21,104-3", header);
            Assert.Contains("#HEADEREND#", header);
        }

        [Fact]
        public void Encode_ShouldOutputCorrectEncodedValue()
        {
            var compression = new CCCompressionTool();
            var encoded = compression.Encode("test");
            var headerDecoded = string.Empty;
            var headerDecodedLastIndex = -1;
            for (int i = 0; i < encoded.Length; i++)
            {
                headerDecoded += Convert.ToChar(encoded[i]);

                if (headerDecoded.Contains("#HEADEREND#"))
                {
                    headerDecodedLastIndex = i;
                    break;
                }
            }

            var decodedBitString = encoded.Substring(headerDecodedLastIndex + 1);

            Assert.Contains("116-2,101-1,115-1", headerDecoded);
            Assert.Contains("010110", decodedBitString.ToString());
        }

        [Fact]
        public void Decode_ShouldOutputCorrectDecodedValue()
        {
            var compression = new CCCompressionTool();
            var encoded = compression.Encode(text);
            var decoded = compression.Decode(encoded);

            Assert.Equal(text, decoded);
        }

        [Fact]
        public void Compress_ShouldOutputSmallerFileSize()
        {
            var compression = new CCCompressionTool();
            var fileResult = compression.Compress("test.txt");

            FileInfo source = new FileInfo("test.txt");
            FileInfo compressed = new FileInfo(fileResult);

            Assert.True(compressed.Length < source.Length);
        }

        [Fact]
        public void Decompress_ShouldOutputLargeFileSize()
        {
            var compression = new CCCompressionTool();
            var fileResult = compression.Decompress("compressed-test.txt", "decompressed-test.txt");

            FileInfo sourceFile = new FileInfo("test.txt");
            FileInfo decompressedFile = new FileInfo(fileResult);

            Assert.True(decompressedFile.Length <= sourceFile.Length);
        }
    }
}
