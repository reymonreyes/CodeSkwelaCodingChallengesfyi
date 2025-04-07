﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompressionTool
{
    public class CCCompressionTool
    {
        public Dictionary<char, int> Compress(string text)
        {
            return new Dictionary<char, int>();
        }

        public Dictionary<char, int> BuildFrequencyTable(string text)
        {
            var result = new Dictionary<char, int>();

            foreach (var character in text)
            {
                if (!result.ContainsKey(character))
                {
                    result.Add(character, 1);
                }
                else
                {
                    result[character]++;
                }
            }

            return result.OrderBy(x => x.Value).ToDictionary();
        }
    }
}
