namespace cutTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || !args[0].StartsWith("-f"))
                return;

            //Step1IntegrationTest(args);
            //Step2IntegrationTest(args);
            //Step31IntegrationTest(args);
            Step32IntegrationTest(args);
        }

        static void Step1IntegrationTest(string[] args)
        {
            //testing or field list option
            //note: set arguments in Debug Properties -> Command line arguments -> -f2 sample.tsv
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
            //note: set arguments in Debug Properties -> Command line arguments -> -f1 -d, fourchords.csv
            var cutTool = new Cut();
            var fileParam = args.LastOrDefault();
            var result = cutTool.Run(args, fileParam);

            foreach (var item in result)
            {
                Console.WriteLine(item);
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
