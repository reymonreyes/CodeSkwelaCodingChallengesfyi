using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CompressionTool
{
    public class CCCompressionTool
    {
        public Dictionary<char, int> Compress(string text)
        {
            return new Dictionary<char, int>();
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

            result = frequencies.Select(x => new Node { Character = x.Key, Weight = x.Value }).OrderBy(x => x.Weight).ToList();

            return result;
        }

        public Node BuildBinaryTree(List<Node> frequencyTable)
        {
            var result = new Node();

            /*
            loop until frequency table is only 1
                1 - get(remove) first 2 nodes from the frequency table
                2 - add their frequencies
                3 - make that frequency total a parent node of the first 2 nodes
                4 - insert the new node in it's correct position in the frequency table
            */
            
            while(frequencyTable.Count > 1)
            {
                var leftNode = frequencyTable[0];
                var rightNode = frequencyTable[1];
                frequencyTable.RemoveAt(0);
                frequencyTable.RemoveAt(0);
                var parentNode = new Node();
                parentNode.Weight = leftNode.Weight + rightNode.Weight;
                parentNode.Left = leftNode;
                parentNode.Right = rightNode;

                frequencyTable.Add(parentNode);

                frequencyTable.Sort((a, b) =>
                {
                    if (a.Weight > b.Weight) return 1;
                    else if (a.Weight < b.Weight) return -1;
                    else return 0;
                });
            }

            result = frequencyTable.FirstOrDefault();

            return result!;
        }
    }

    public class Node
    {
        public char Character { get; set; }
        public int Weight { get; set; }
        public Node? Left { get; set; }
        public Node? Right { get; set; }
    }
}
