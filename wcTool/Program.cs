using Microsoft.VisualBasic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace wcTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (Console.IsInputRedirected)
            {
                int readChar;
                List<byte> bytesFromConsole = new List<byte>();


                while ((readChar = Console.Read()) >= 0)
                {
                    bytesFromConsole.Add((byte)readChar);
                }

                WcFromConsoleInput(args, bytesFromConsole);
            }
            else if (args.Length == 2)
            {
                switch (args[0])
                {
                    case "-c":
                        Console.WriteLine($"{GetNumberOfBytes(args[1])} {args[1]}");
                        break;
                    case "-l":
                        Console.WriteLine($"{GetNumberOfLines(args[1])} {args[1]}");
                        break;
                    case "-w":
                        Console.WriteLine($"{GetNumberOfWords(args[1])} {args[1]}");
                        break;
                    case "-m":
                        Console.WriteLine($"{GetNumberOfCharacters(args[1])} {args[1]}");
                        break;
                }
            }
            else if (args.Length == 1)
            {
                Console.WriteLine($"{GetNumberOfLines(args[0])}\t{GetNumberOfWords(args[0])}\t{GetNumberOfBytes(args[0])} {args[0]}");
                //GetDefaultValues(args[0]);
            }
            
        }

        private static int GetNumberOfCharacters(string file)
        {
            return File.ReadAllText(file).Length;
        }

        private static int GetNumberOfWords(string file)
        {
            var contents = File.ReadAllText(file);
            var splitContents = contents.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            return splitContents.Length;
        }

        private static int GetNumberOfLines(string file)
        {
            return File.ReadAllLines(file).Length;
        }

        private static int GetNumberOfBytes(string file)
        {
            return File.ReadAllBytes(file).Length;
        }

        private static void GetDefaultValues(string file)
        {
            var fileBytes = File.ReadAllBytes(file);
            var byteString = System.Text.Encoding.UTF8.GetString(fileBytes);
            var numberOfLines = byteString.Split(new char[] { '\n' }).Length;
            var numberOfWords = byteString.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

            Console.WriteLine($"{numberOfLines}\t{numberOfWords}\t{fileBytes.Length} {file}");
        }

        private static void WcFromConsoleInput(string[] args, List<byte> bytesFromConsole)
        {
            if (bytesFromConsole.Count > 0)
            {
                var fullString = System.Text.Encoding.Default.GetString(bytesFromConsole.ToArray());

                if (args.Length == 0)
                {
                    var numberOfLines = fullString.Split('\n').Length;
                    var numberOfWords = fullString.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    Console.WriteLine($"{numberOfLines}\t{numberOfWords}\t{bytesFromConsole.Count}");
                }
                else
                {
                    switch (args[0])
                    {
                        case "-c":
                            Console.WriteLine($"{bytesFromConsole.Count}");
                            break;
                        case "-l":
                            var numberOfLines = fullString.Split('\n').Length;
                            Console.WriteLine($"{numberOfLines}");
                            break;
                        case "-w":
                            var numberOfWords = fullString.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                            Console.WriteLine($"{numberOfWords}");
                            break;
                        case "-m":
                            Console.WriteLine($"{fullString.Length}");
                            break;
                    }
                }
            }
        }
    }
}
