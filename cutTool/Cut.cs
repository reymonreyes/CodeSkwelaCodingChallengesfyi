using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cutTool
{
    public class Cut
    {
        public string[] Run(string field, string filePath)
        {
            int fieldIndex;
            
            if(string.IsNullOrWhiteSpace(field) || field.Length > 2 || !field.StartsWith('f') || !int.TryParse(field[1].ToString(), out fieldIndex))
                return new string[0];
            if (fieldIndex <= 0)
                return new string[0];

            var file = File.ReadAllLines(filePath);
            string[] result = new string[file.Length];

            for (int i = 0; i < result.Length; i++)
            {
                var line = file[i];
                var rowFields = line.Split('\t');
                result[i] = rowFields[(fieldIndex == 1 ? 0 : fieldIndex - 1)];//note: field position number is 1 based
            }

            return result;
        }
    }
}
