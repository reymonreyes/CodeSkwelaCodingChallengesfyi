using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace CompressionTool
{
    public class CCCompressionTool
    {
        public string Compress(string file)
        {
            if (File.Exists(file))
            {
                var fileContents = File.ReadAllText(file);
                var encoded = Encode(fileContents);

                //compress to bytes
                var headerEnd = "#HEADEREND#";
                var headerLastIndex = encoded.IndexOf(headerEnd) + headerEnd.Length;
                var header = encoded.Substring(0, headerLastIndex);
                var bitString = encoded.Substring(header.Length);
                List<byte> prefixByteList = new List<byte>();

                for (int i = 0; i < bitString.Length; i += 8)
                {
                    var byteString = string.Empty;
                    var isLastByteString = false;
                    if (bitString.Length - i < 8)
                        byteString = bitString.Substring(i);
                    else
                        byteString = bitString.Substring(i, 8);

                    var byteLength = i + byteString.Length - 1;
                    if (byteLength == bitString.Length - 1)
                        isLastByteString = true;

                    //last byte sequence could vary in length eg. "0000", "0101", "00000000"
                    //so there are actually 2 bytes added
                    //the first byte is the actual byte value
                    //the second is the padding of zeros in byte value
                    if (isLastByteString)
                    {
                        var padCount = 0;

                        for (int j = 0; j < byteString.Length; j++)
                        {
                            if (byteString[j] == '1')
                                break;

                            if (byteString[j] == '0')
                                padCount++;
                        }

                        padCount = padCount - 1 < 0 ? 0 : padCount - 1;

                        prefixByteList.Add(Convert.ToByte(byteString, 2));//actual byte value
                        prefixByteList.Add(Convert.ToByte(padCount));//padding value to be used for decoding later
                    }
                    else
                    {
                        prefixByteList.Add(Convert.ToByte(byteString, 2));
                    }
                }

                var headerBytes = Encoding.ASCII.GetBytes(header);
                var result = headerBytes.Concat(prefixByteList).ToArray();

                var resultFilename = "compressed-" + file;

                File.WriteAllBytes(resultFilename, result);

                var fileInfo = new FileInfo(resultFilename);

                return fileInfo.FullName;
            }            

            return string.Empty;
        }
        
        public List<Node> BuildFrequencyTable(string text)
        {
            var result = new List<Node>();
            var frequencies = new Dictionary<char, int>();

            foreach (var character in text)
            {
                if (!frequencies.ContainsKey(character))
                    frequencies.Add(character, 1);
                else
                    frequencies[character]++;
            }

            result = frequencies.Select(x => new Node { Character = x.Key, Weight = x.Value }).ToList();
            return result;
        }

        public Node BuildBinaryTree(List<Node> frequencyTable)
        {

            var tableCopy = new List<Node>(frequencyTable.OrderBy(x => x.Weight).ToList());
            var result = new Node();

            /*
            loop until frequency table is only 1
                1 - get(remove) first 2 nodes from the frequency table
                2 - add their frequencies
                3 - make that frequency total a parent node of the first 2 nodes
                4 - insert the new node in it's correct position in the frequency table
            */
            
            while(tableCopy.Count > 1)
            {
                var leftNode = tableCopy[0];
                var rightNode = tableCopy[1];
                tableCopy.RemoveAt(0);
                tableCopy.RemoveAt(0);
                var parentNode = new Node();
                parentNode.Weight = leftNode.Weight + rightNode.Weight;
                parentNode.Left = leftNode;
                parentNode.Right = rightNode;

                tableCopy.Add(parentNode);

                
                tableCopy.Sort((a, b) =>
                {
                    if (a.Weight > b.Weight) return 1;
                    else if (a.Weight < b.Weight) return -1;
                    else return 0;
                });
                
            }

            result = tableCopy.FirstOrDefault();

            return result!;
        }
        public List<CharacterCode> AssignCodes(Node node)
        {            
            var result = new List<CharacterCode>();
            var nodes = new List<Node>();

            if (node != null) nodes.Add(node);

            while (nodes.Count > 0)
            {
                var currentNode = nodes.LastOrDefault();

                if ((currentNode.Left == null && currentNode.Right == null) || (currentNode.Left != null && currentNode.Left.Bit == '0' && currentNode.Right != null && currentNode.Right.Bit == '1'))
                {
                    nodes.RemoveAt(nodes.Count - 1);

                    if (currentNode.Left == null && currentNode.Right == null)
                    {
                        var code = string.Empty;
                        foreach (var item in nodes)
                        {
                            if (item.Bit.HasValue)
                                code += item.Bit;
                        }
                        result.Add(new CharacterCode { Character = currentNode.Character.Value, Code = code += currentNode.Bit, Weight = currentNode.Weight });
                    }

                    continue;
                }

                if (currentNode.Left != null && currentNode.Left.Bit is null)
                {
                    currentNode.Left.Bit = '0';
                    nodes.Add(currentNode.Left);
                }
                else if (currentNode.Right != null && currentNode.Right.Bit is null)
                {
                    currentNode.Right.Bit = '1';
                    nodes.Add(currentNode.Right);
                }
            }
            
            return result.ToList();
        }

        public string GenerateHeader(List<Node> nodes)
        {
            var result = new StringBuilder();

            result.Append("#HEADERSTART#");
            result.Append(string.Join(',', nodes.Select(x => $"{(int)x.Character!}-{x.Weight}").ToArray()));
            result.Append("#HEADEREND#");

            return result.ToString();
        }
        
        public string Encode(string source)//Convert the string into bits
        {
            var bitStringBuilder = new StringBuilder();

            var frequencyTable = BuildFrequencyTable(source);
            var tree = BuildBinaryTree(frequencyTable);
            var codes = AssignCodes(tree);
            var codeMap = codes.Select(x => new KeyValuePair<char, string>(x.Character, x.Code)).ToDictionary();

            var header = GenerateHeader(frequencyTable);
            bitStringBuilder.Append(header);

            foreach (var item in source)
                bitStringBuilder.Append(codeMap[item]);

            return bitStringBuilder.ToString();            
        }

        public string Decode(string encoded)//Convert the bits into string
        {
            //parse the header that contains the frequency table            
            string headerStart = "#HEADERSTART#", headerEnd = "#HEADEREND#";
            var headerLastIndex = encoded.IndexOf(headerEnd) + headerEnd.Length;
            var header = encoded.Substring(0, headerLastIndex);
            var frequencyStr = header.Replace(headerStart, string.Empty).Replace(headerEnd, string.Empty).Trim();
            var temp = frequencyStr.Split(',');
            var frequencyTable = new List<Node>();

            foreach (var item in temp)
            {
                var charFrequency = item.Split('-');
                frequencyTable.Add(new Node { Character = Convert.ToChar(Convert.ToInt32(charFrequency[0])), Weight = int.Parse(charFrequency[1]) });
            }

            //build the tree from the frequency table
            var rootNode = BuildBinaryTree(frequencyTable);

            //assign codes
            var codes = AssignCodes(rootNode);

            //decode the bits by following the path on the binary tree
            var encodedStr = encoded.Substring(headerLastIndex).TrimStart();
            var result = new StringBuilder();
            var nodes = new List<Node>() { rootNode };
            var currentNode = rootNode;

            foreach (var item in encodedStr)
            {
                if (item == '0')
                    currentNode = currentNode.Left;
                else if (item == '1')
                    currentNode = currentNode.Right;

                if (currentNode.Left == null && currentNode.Right == null)
                {
                    result.Append(currentNode.Character);
                    currentNode = rootNode;
                }
            }

            return result.ToString();
        }
    }

    public class Node
    {
        public char? Character { get; set; }
        public int Weight { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }
        public char? Bit { get; set; }
    }

    public class CharacterCode
    {
        public char Character { get; set; }
        public int Weight { get; set; }
        public string Code { get; set; }
    }
}
