using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cutTool
{
    public class Cut
    {
        public string[] Run(string[] options, string filePath)
        {
            if (options.Length == 0 || string.IsNullOrWhiteSpace(filePath))
                return new string[0];
            
            //make sure only valid options: f(field),d(delimiter)
            if(options.Where(x => x.StartsWith("f") || x.StartsWith("d")).Count() == 0)
                return new string[0];

            var file = File.ReadAllLines(filePath);
            string[] result = new string[file.Length];

            //get field param
            var fieldParam = options.FirstOrDefault(x => x.StartsWith("f"));
            if(string.IsNullOrEmpty(fieldParam) || fieldParam.Length > 2)
                return new string[0];

            if (!int.TryParse(fieldParam[1].ToString(), out int fieldIndex))
                return new string[0];

            if (fieldIndex <= 0)
                return new string[0];

            //get delimiter param
            var delimiter = '\t';//default
            var delimiterParam = options.FirstOrDefault(x => x.StartsWith("d"));
            if(!string.IsNullOrWhiteSpace(delimiterParam) && delimiterParam.Length == 2)
                delimiter = delimiterParam[1];

            for (int i = 0; i < result.Length; i++)
            {
                var line = file[i];
                var rowFields = line.Split(delimiter);
                result[i] = rowFields[fieldIndex - 1];//note: field position number is 1 based
            }

            return result;
        }
    }
}
