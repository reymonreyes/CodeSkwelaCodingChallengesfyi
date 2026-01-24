namespace cutTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || !args[0].StartsWith("-f"))
                return;

            //Step1IntegrationTest(args);
            Step2IntegrationTest(args);
        }

        static void Step1IntegrationTest(string[] args)
        {
            //note: set arguments in Debug Properties -> Command line arguments -> -f2 sample.tsv
            var cutTool = new Cut();           
            var options = args.Where(x => x.StartsWith("-")).Select(x => x.Substring(1)).ToArray();
            var fileParam = args.FirstOrDefault(x => !x.StartsWith("-"));
            var result = cutTool.Run(options, fileParam);

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        static void Step2IntegrationTest(string[] args)
        {
            //note: set arguments in Debug Properties -> Command line arguments -> -f1 -d, fourchords.csv
            var cutTool = new Cut();
            var options = args.Where(x => x.StartsWith("-")).Select(x => x.Substring(1)).ToArray();
            var fileParam = args.FirstOrDefault(x => !x.StartsWith("-"));
            var result = cutTool.Run(options, fileParam);

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
    }
}
