namespace SortTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Step1Sort(args);
            Step2Sort(args);
        }

        static void Step1Sort(string[] args)
        {
            //note: set arguments in Debug Properties -> Command line arguments -> words.txt
            if (args.Length == 0) return;

            var sort = new Sort(args[0]);
            var result = sort.Run(args);

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }

        static void Step2Sort(string[] args)
        {
            //note: set arguments in Debug Properties -> Command line arguments -> -u words.txt
            if (args.Length == 0) return;

            var sort = new Sort(args[1]);
            var result = sort.Run(args);

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
    }
}
