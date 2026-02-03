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
        public string[] Run()
        {
            var tempLines = System.IO.File.ReadAllLines(_file);
            var lines = new List<string>(tempLines);

            //remove empty strings
            lines.RemoveAll(x => x == string.Empty);
            lines.RemoveAll(x => x == "§" || x == "§§");

            var result =  lines.OrderBy(x => x, StringComparer.Ordinal).ToArray();

            return result;
        }
    }
}
