namespace cutTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || !(args[0].StartsWith("-f") || args[0].StartsWith("-d")))
                return;            

            var fileOption = args.LastOrDefault(x => !(x.StartsWith("-f") || x.StartsWith("-d")) || x == "-");
            if (string.IsNullOrWhiteSpace(fileOption) || fileOption == "-")
            {
                var input = string.Empty;
                var lines = new List<string>();
                do
                {
                    input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        lines.Add(input);
                }
                while (!string.IsNullOrWhiteSpace(input));

                var cutTool = new Cut();
                var result = cutTool.Run(args, lines.ToArray());
                PrintResult(args, result);
            }
            else
            {
                var cutTool = new Cut();
                var fileParam = args.LastOrDefault();
                var result = cutTool.Run(args, fileParam);
                PrintResult(args, result);
            }
            //Step1IntegrationTest(args);
            //Step2IntegrationTest(args);
            //Step31IntegrationTest(args);
            //Step32IntegrationTest(args);
        }

        static void PrintResult(string[] args, string[,] result)
        {
            var delimiterParams = args.FirstOrDefault(x => x.StartsWith("-d"));
            char delimiter = '\t';
            if (!string.IsNullOrWhiteSpace(delimiterParams) && delimiterParams.Length > 2)
                delimiter = delimiterParams[2];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Console.Write($"{result[i, j]}");
                    if (j < result.GetLength(1) - 1)
                        Console.Write($"{delimiter}");
                }
                Console.WriteLine();
            }
        }

        static void Step1IntegrationTest(string[] args)
        {
            //testing or field list option
            //note: set arguments in Debug Properties -> Command line arguments -> -f2 challenge-cut/sample.tsv
            var cutTool = new Cut();           
            var fileParam = args.LastOrDefault();
            var result = cutTool.Run(args, fileParam);
            
            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        static void Step2IntegrationTest(string[] args)
        {
            //testing for delimiter option
            //note: set arguments in Debug Properties -> Command line arguments -> -f1,2 -d, challenge-cut/fourchords.csv
            var cutTool = new Cut();
            var fileParam = args.LastOrDefault();
            var result = cutTool.Run(args, fileParam);
            var delimiterParams = args.FirstOrDefault(x => x.StartsWith("-d"));
            char delimiter = '\t';
            if (!string.IsNullOrWhiteSpace(delimiterParams) && delimiterParams.Length > 2)
                delimiter = delimiterParams[2];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Console.Write($"{result[i, j]}");
                    if (j < result.GetLength(1))
                        Console.Write($"{delimiter}");
                }
                Console.WriteLine();
            }
        }

        static void Step31IntegrationTest(string[] args)        
        {
            //testing for comma separated fields list
            //note: set arguments in Debug Properties -> Command line arguments -> -f1,2 challenge-cut/sample.tsv
            var cutTool = new Cut();
            var fileParam = args.LastOrDefault();
            var result = cutTool.Run(args, fileParam);

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Console.Write($"{result[i,j]}\t");
                }
                Console.WriteLine();
            }
        }

        static void Step32IntegrationTest(string[] args)
        {
            //testing for whitespace separated fields list
            //note: set arguments in Debug Properties -> Command line arguments -> -f "1 2" challenge-cut/sample.tsv
            var cutTool = new Cut();
            var fileParam = args.LastOrDefault();
            var result = cutTool.Run(args, fileParam);

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    Console.Write($"{result[i, j]}\t");
                }
                Console.WriteLine();
            }
        }
    }
}
