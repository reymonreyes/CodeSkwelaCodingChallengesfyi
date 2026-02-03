using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortTool
{
    public class Sort
    {
        private readonly string _file;
        public Sort(string file)
        {
            _file = file;
        }
        public string[] Run(string[] options)
        {
            var tempLines = System.IO.File.ReadAllLines(_file);
            var lines = new List<string>(tempLines);

            //remove empty strings
            lines.RemoveAll(x => x == string.Empty);
            lines.RemoveAll(x => x == "§" || x == "§§");           
            
            var result = lines.OrderBy(x => x, StringComparer.Ordinal).ToList();

            if (options.Contains("-u"))
                result = result.Distinct().ToList();

            return result.ToArray();
        }
    }
}
