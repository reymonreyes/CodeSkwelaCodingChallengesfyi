using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser
{
    public class JsonParser
    {
        public int IsValidStringKey(string json)
        {
            if(string.IsNullOrEmpty(json)) return 1;
            //trim start/end of string
            json = json.Trim();
            if (json.Length < 2) return 1;
            //error if start of string is not '{'
            if (json[0] != '{') return 1;
            //error if end of string is not '}'
            if (json[json.Length - 1] != '}') return 1;

            int strStartIndx = -1, strEndIndx = -1;

            for (int i = 0; i < json.Length; i++)
            {
                var charItem = json[i];
                if (!char.IsWhiteSpace(charItem) && charItem == '"' && strStartIndx < 0)
                    strStartIndx = i;

                if(strStartIndx >= 0 && charItem == ':')//find the colon : delimiter
                {
                    int dblQteCounter = i;

                    while (dblQteCounter > strStartIndx)
                    {
                        if (!char.IsWhiteSpace(json[dblQteCounter]) && json[dblQteCounter] == '"')
                        {
                            strEndIndx = dblQteCounter;
                            break;
                        }

                        dblQteCounter--;
                    }

                    if (dblQteCounter == strStartIndx)
                        return 1;                    
                }

                if (strStartIndx >= 0 && strEndIndx >= 0)
                {
                    for (int k = strStartIndx + 1; k < strEndIndx; k++)
                    {
                        char currentItem = json[k];
                        if (k == strStartIndx + 1 && currentItem == '"')
                            return 1;

                        if(currentItem == '"' || currentItem == '\\' || char.IsControl(currentItem) && json[k+1] != '\\')
                            return 1;
                    }
                }
            }

            return 0;
        }        

        public int IsValid(string json)
        {
            if (string.IsNullOrEmpty(json)) return 1;
            json = json.Trim();

            if(json.Length < 2) return 1;

            var firstElement = json[0];
            var lastElement = json[json.Length - 1];
            if (!(firstElement == '{' || firstElement == '[')) return 1;            
            if (firstElement == '{' && lastElement != '}') return 1;
            if (firstElement == '[' && lastElement != ']') return 1;

            var rootTypes = new Stack<int>();

            for (int i = 0; i < json.Length;)
            {
                var element = json[i];

                if(i == json.Length - 1 && element != '}' && element != ']') return 1;                

                if (char.IsWhiteSpace(element))
                {
                    i++;
                    continue;
                }

                if (element == ',')
                {
                    var rootType = json[rootTypes.Peek()];
                    if (rootType == '{')
                    {
                        var hasNextKey = false;
                        for (int x = i + 1; x < json.Length; x++)
                        {
                            if (json[x] == '"')
                            {
                                hasNextKey = true;
                                break;
                            }
                        }
                        if (!hasNextKey) return 1;
                    }

                    //forward scan
                    bool hasValidNextElement = false;
                    for (int x = i + 1;x < json.Length; x++)
                    {
                        var nextElement = json[x];
                        if (nextElement == '"' || char.IsDigit(nextElement) || nextElement == '{' ||
                                nextElement == '[' || nextElement == 't' || nextElement == 'f' || nextElement == 'n')
                        {
                            hasValidNextElement = true;
                            break;
                        }
                    }
                    if (!hasValidNextElement) return 1;

                    i++;
                    continue;
                }

                if (element == '{' || element == '[')
                {
                    if (element == '[')
                    {
                        for (int x = i + 1; x < json.Length - 1; x++)
                        {
                            var nextElement = json[x];
                            if (char.IsWhiteSpace(nextElement)) continue;
                            if (!(nextElement == '"' || char.IsDigit(nextElement) || nextElement == '{' ||
                                nextElement == ']' || nextElement == '}' || nextElement == '[' ||
                                    nextElement == 't' || nextElement == 'f' || nextElement == 'n')) return 1;
                            else
                                break;
                        }
                    }

                    if(element == '{')
                    {
                        for (int x = i + 1; x < json.Length; x++)
                        {
                            if (x < json.Length - 1)
                            {
                                var nextElement = json[x];
                                if (char.IsWhiteSpace(nextElement)) continue;
                                if (nextElement != '"' && nextElement != '}') return 1;
                                else break;
                            }
                        }
                    }

                    rootTypes.Push(i);

                    if (rootTypes.Count >= 20) return 1;

                    i++;
                    continue;
                }

                if(element == '}' || element == ']')
                {
                    if(rootTypes.Count == 0) return 1;
                    rootTypes.Pop();

                    if (i == json.Length - 1 && rootTypes.Count > 0) return 1;

                    i++;
                    continue;
                }

                if(rootTypes.Count == 0) return 1;

                var rootElement = json[rootTypes.Peek()];

                //parse key if object
                if(rootElement == '{')
                {
                    int keyStartIndex = -1;
                    for (int x = i; x < json.Length; x++)
                    {
                        var keyElement = json[x];
                        if(char.IsWhiteSpace(keyElement)) continue;

                        if(keyElement == '"')
                        {
                            keyStartIndex = x;
                            break;
                        }
                        else return 1;
                    }
                    if (keyStartIndex == -1) return 1;

                    //forward scan for unescaped characters
                    int keyDelimiterIndex = -1;
                    for (int x = keyStartIndex + 1; x < json.Length;)
                    {
                        int nextIndex = x + 1;
                        if (nextIndex >= json.Length) return 1;

                        var currentElement = json[x];
                        var nextElement = json[nextIndex];
                        if (currentElement == '\\')
                        {
                            if (!IsEscapeCharacter(nextElement)) return 1;
                            else x = nextIndex + 1;
                        }
                        else if (currentElement == '"')
                        {
                            for (int y = x + 1; y < json.Length; y++)
                            {
                                if (char.IsWhiteSpace(json[y])) continue;

                                if (json[y] == ':')
                                {
                                    keyDelimiterIndex = y;
                                    break;
                                }
                            }

                            if(keyDelimiterIndex == -1) return 1;
                            if (keyDelimiterIndex > 0) break;
                        }
                        else
                            x++;
                    }                    

                    int keyEndIndex = -1;
                    for(int x = keyDelimiterIndex - 1;x > keyStartIndex; x--)
                    {
                        var keyElement = json[x];
                        if (char.IsWhiteSpace(keyElement)) continue;

                        if (keyElement == '"')
                        {
                            keyEndIndex = x;
                            break;
                        }
                    }
                    if (keyEndIndex == -1) return 1;

                    //validate key string
                    if (keyDelimiterIndex >= 0 && keyStartIndex >= 0 && keyEndIndex >= 0)
                    {
                        for (var j = keyStartIndex + 1; j < keyEndIndex; j++)
                        {
                            var currentElement = json[j];
                            if (!char.IsWhiteSpace(currentElement) && currentElement == '"') return 1;

                            if (currentElement == '\\')
                            {
                                var nextElementIndex = j + 1;
                                var nextElement = json[nextElementIndex];//what is a valid next element?
                                if (char.IsWhiteSpace(nextElement) || !IsEscapeCharacter(nextElement))
                                    return 1;
                                else if (nextElement == 'u')
                                {
                                    if (!IsValidHexValue(json, nextElementIndex + 1))
                                        return 1;
                                    else
                                        j = j + 4;
                                }
                                else
                                    j++;
                            }
                        }

                    }
                    
                    i = keyDelimiterIndex + 1;
                }

                //parse value
                int valDelimiterIndex = -1;
                int valStartIndex = -1, valEndIndex = -1;
                //1 find the value delimiter
                for (int j = i; j < json.Length; j++)
                {
                    var valElement = json[j];
                    if (valElement == ',' || valElement == ']' || valElement == '}')
                    {
                        valDelimiterIndex = j;
                        break;
                    }
                    else if(valElement == '"')
                    {
                        if (j == json.Length - 1) return 1;
                        //forward scan for unescaped characters
                        for (int x = j + 1; x < json.Length;)
                        {
                            int nextIndex = x + 1;
                            if (nextIndex >= json.Length) return 1;
                            var currentElement = json[x];
                            var nextElement = json[nextIndex];
                            
                            if(currentElement == '\t') return 1;

                            if (currentElement == '\\')
                            {
                                if (!IsEscapeCharacter(nextElement)) return 1;
                                else x = nextIndex + 1;
                            }
                            else if (currentElement == '"')
                            {
                                var rootType = json[rootTypes.Peek()];
                                for (int y = x + 1; y < json.Length; y++)
                                {
                                    if (char.IsWhiteSpace(json[y])) continue;

                                    if (rootType == '{')
                                    {
                                        if (json[y] == ',' || json[y] == '}' || json[y] == ']')
                                        {
                                            valDelimiterIndex = y;
                                            break;
                                        }
                                    }
                                    else if(rootType == '[')
                                    {
                                        if (json[y] == ',' || json[y] == ']')
                                        {
                                            valDelimiterIndex = y;
                                            break;
                                        }
                                        else
                                            return 1;
                                    }
                                }

                                if (valDelimiterIndex == -1) return 1;
                                if (valDelimiterIndex > 0) break;
                            }
                            else
                                x++;
                        }

                        if (valDelimiterIndex > 0) break;                        
                    }

                }

                if (valDelimiterIndex < 0) return 1;
                
                ValueDataType valueType = ValueDataType.Unknown;
                //2 find the value
                for (var j = i; j < json.Length; j++)
                {
                    var valElement = json[j];

                    if (char.IsWhiteSpace(valElement)) continue;

                    if (valElement == '"')//a string value
                    {
                        valueType = ValueDataType.String;
                        valStartIndex = j;                       

                        for (int m = valDelimiterIndex - 1; m > valStartIndex; m--)
                        {
                            var prevElement = json[m];
                            if (!char.IsWhiteSpace(prevElement) && prevElement == '"' || prevElement == '}')
                            {
                                valEndIndex = m;
                                break;
                            }
                        }

                        if (valEndIndex < 0) return 1;

                        if (valStartIndex >= 0 && valEndIndex >= 0)
                        {
                            var substringLength = valEndIndex - (valStartIndex + 1);
                            if (substringLength >= 0)
                            {
                                var jsonSubstr = json.Substring(valStartIndex + 1, substringLength);
                                if (!IsStringValueValidJson(jsonSubstr))
                                    return 1;
                            }
                        }

                        break;
                    }
                    else if (char.IsDigit(valElement) || valElement == '-')//number value
                    {
                        valueType = ValueDataType.Number;
                        valStartIndex = j;
                        for (int m = valDelimiterIndex - 1; m >= valStartIndex; m--)
                        {
                            var prevElement = json[m];
                            if (!char.IsWhiteSpace(prevElement))
                            {
                                valEndIndex = m;
                                break;
                            }
                        }

                        if(valEndIndex < 0) return 1;

                        //parse 0 to nine
                        //check if there are spaces between digits
                        var valSubstring = json.Substring(valStartIndex, valEndIndex - valStartIndex + 1);
                        var splitWhitespaces = valSubstring.Split(new char[] { ' ', '\r', '\n', '\t' });
                        if (splitWhitespaces.Length >= 2) return 1;
                        if (valSubstring.Count(x => x == '.') > 1) return 1;

                        if (valStartIndex >= 0)
                        {
                            if (json[valStartIndex] == 0 && valEndIndex - valStartIndex > 0) return 1;

                            for (int m = valStartIndex; m <= valEndIndex; m++)
                            {
                                var currentChar = json[m];

                                if (currentChar == '0' && m == valStartIndex && valEndIndex - valStartIndex > 0 && json[m+1] != '.')//must be the only value
                                {
                                    return 1;   
                                }
                                else if (char.IsDigit(currentChar) || currentChar == '+' || currentChar == '-')
                                {
                                    continue;
                                }
                                else if (currentChar == '.')
                                {
                                    var nextChar = json[m + 1];
                                    if (!char.IsDigit(nextChar)) return 1;
                                }
                                else if (currentChar == 'e' || currentChar == 'E')
                                {
                                    var nextChar = json[m + 1];
                                    if (!(char.IsDigit(nextChar) || nextChar != '+' || nextChar != '-') || char.IsWhiteSpace(nextChar)) return 1;
                                }
                                else if (currentChar == '}')
                                    continue;
                                else
                                    return 1;
                            }
                        }

                        break;
                    }
                    else if (valElement == 't' || valElement == 'f')//boolean value
                    {
                        valueType = ValueDataType.Boolean;
                        valStartIndex = j;
                        for (int m = valDelimiterIndex - 1; m >= valStartIndex; m--)
                        {
                            var prevElement = json[m];
                            if (!char.IsWhiteSpace(prevElement))
                            {
                                valEndIndex = m;
                                break;
                            }
                        }

                        if (valEndIndex < 0) return 1;

                        bool tempBool;
                        if (!bool.TryParse(json.Substring(valStartIndex, valEndIndex - valStartIndex + 1), out tempBool)) return 1;

                        break;
                    }
                    else if (valElement == 'n')//null value
                    {
                        valueType = ValueDataType.Null;
                        valStartIndex = j;
                        for (int m = valDelimiterIndex - 1; m >= valStartIndex; m--)
                        {
                            var prevElement = json[m];
                            if (!char.IsWhiteSpace(prevElement))
                            {
                                valEndIndex = m;
                                break;
                            }
                        }

                        if(valEndIndex < 0) return 1;
                        if (valEndIndex - valStartIndex + 1 != 4) return 1;
                        if (!(json[valStartIndex] == 'n' && json[valStartIndex + 1] == 'u' && json[valStartIndex + 2] == 'l' && json[valStartIndex + 3] == 'l')) return 1;

                        break;
                    }
                    else if (valElement == '{' || valElement == '[')//object or array value
                    {
                        valueType = ValueDataType.Object;
                        i = j;
                        break;
                    }
                    else
                        return 1;                    
                }

                if (valueType == ValueDataType.Object) continue;
                
                var valDelimiter = json[valDelimiterIndex];                

                i = valDelimiterIndex;
            }

            return 0;
        }

        public bool IsValidHexValue(string json, int indexStart)
        {
            for (var i = 0; i < 4; i++)
            {
                if (indexStart > json.Length - 1) return false;
                if (!char.IsAsciiHexDigit(json[indexStart])) return false;

                indexStart++;
            }

            return true;
        }

        public bool IsStringValueValidJson(string str)
        {
            if (str == null) return false;

            for (var j = 0; j < str.Length; j++)
            {
                var currentElement = str[j];
                if (!char.IsWhiteSpace(currentElement) && currentElement == '"') return false;

                if(currentElement == '\n') return false;
                if (currentElement == '\\')
                {
                    if (j + 1 >= str.Length) return false;

                    var nextElementIndex = j + 1;
                    var nextElement = str[nextElementIndex];//what is a valid next element?
                    if (char.IsWhiteSpace(nextElement) || !IsEscapeCharacter(nextElement))
                        return false;
                    else if (nextElement == 'u')
                    {
                        if (!IsValidHexValue(str, nextElementIndex + 1))
                            return false;
                        else
                            j = j + 4;
                    }
                    else
                        j++;
                }
            }

            return true;
        }

        private bool IsEscapeCharacter(char charValue)
        {
            return charValue == '\\' || charValue == '\"' || charValue == '/' || charValue == 'b' || charValue == 'f' || charValue == 'n' || charValue == 'r' || charValue == 't' || charValue == 'u';
        }

        public (int endIndex, int delimiterIndex) FindKeyEndIndex(int start, ref string str)
        {
            //int start = 0
            int end = 0, keyEnd = -1;

            while(keyEnd < 0)
            {
                int searchIndex = start;

                while (searchIndex < str.Length && str[searchIndex] != ':')
                {
                    searchIndex++;
                }

                if (searchIndex >= str.Length)
                    break;

                end = searchIndex;

                while(searchIndex < str.Length && searchIndex > start + 1 && str[searchIndex] != '"')                
                    searchIndex--;

                if (searchIndex < str.Length && str[searchIndex] == '"')
                    keyEnd = searchIndex;
                else
                    start = end+1;                
            }
            
            return (keyEnd, end);
        }
    }

    public enum ValueDataType
    {
        String,
        Number,
        Object,
        Array,
        Boolean,
        Null,
        Unknown
    }
}