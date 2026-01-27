using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cutTool
{
    public class Cut
    {
        public string[,] Run(string[] options, string filePath)
        {
            if (options.Length == 0 || string.IsNullOrWhiteSpace(filePath))
                return new string[0, 0];

            //make sure only valid options: f(field),d(delimiter)
            if (options.Where(x => x.StartsWith("-f") || x.StartsWith("-d")).Count() == 0)
                return new string[0, 0];

            //get field param
            var fieldParam = options.FirstOrDefault(x => x.StartsWith("-f"));

            if (string.IsNullOrEmpty(fieldParam) || fieldParam.Length < 2)
                return new string[0, 0];

            if (!File.Exists(filePath))
                return new string[0, 0]
;
            var file = File.ReadAllLines(filePath);
            
            var fields = new string[0];
            //check for comma separated field list
            if(fieldParam.Length > 2 && int.TryParse(fieldParam.Substring(2,1), out int tempField))
            {
                fields = fieldParam.Substring(2).Split(",");
            }
            else //check for whitespace separated field list
            {
                //make sure -f is followed by " on the options list
                var fieldListOptionIndex = Array.IndexOf(options, "-f");
                if (!options[fieldListOptionIndex + 1].StartsWith("\""))
                    return new string[0, 0];

                fields = options[fieldListOptionIndex + 1].Split(" ");
                for (int i = 0; i < fields.Length; i++)
                    fields[i] = fields[i].Trim('"');

                fields = fields.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            }

            if (fields.Length == 0)
                return new string[0, 0];

            if (fields.Any(x => !int.TryParse(x, out int fieldIndex)))
                return new string[0, 0];            

            //only valid fields
            var fieldIndexes = new List<int>();
            var hasInvalidField = false;
            foreach ( var field in fields)
            {
                if (int.TryParse(field, out var fieldIndex))
                {
                    if(fieldIndex <= 0)
                    {
                        hasInvalidField = true;
                        break;
                    }

                    if (fieldIndex > 0)
                        fieldIndexes.Add(fieldIndex-1);//note: field position number is 1 based
                }
                else
                {
                    hasInvalidField = true;
                    break;
                }
            }

            if(hasInvalidField) return new string[0, 0];

            var result = new string[file.Length, fieldIndexes.Count];
            //get delimiter param
            var delimiter = '\t';//default
            var delimiterParam = options.FirstOrDefault(x => x.StartsWith("-d"));
            if(!string.IsNullOrWhiteSpace(delimiterParam) && delimiterParam.Length == 3)
                delimiter = delimiterParam[2];

            for (int i = 0; i < file.Length; i++)
            {
                var line = file[i];
                var rowFields = line.Split(delimiter);

                for (int x = 0; x < fieldIndexes.Count; x++)
                {
                    result[i,x] = rowFields[fieldIndexes[x]];
                }
            }

            return result;
        }
    }
}
