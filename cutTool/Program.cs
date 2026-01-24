namespace cutTool
{
    public class Program
    {
        static void Main(string[] args)
        {
            Step1IntegrationTest(args);
        }

        static void Step1IntegrationTest(string[] args)
        {
            //note: set arguments in Debug Properties -> Command line arguments
            var cutTool = new Cut();

            if (args.Length < 2 || !args[0].StartsWith("-f"))
                return;

            var fieldParam = args[0].Substring(1);
            var fileParam = args[1];
            var result = cutTool.Run(fieldParam, fileParam);

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
    }
}
