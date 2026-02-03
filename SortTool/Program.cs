namespace SortTool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Step1Sort(args);
        }

        static void Step1Sort(string[] args)
        {
            //note: set arguments in Debug Properties -> Command line arguments -> words.txt
            if (args.Length == 0) return;

            var sort = new Sort(args[0]);
            var result = sort.Run();

            foreach (var item in result)
            {
                Console.WriteLine(item);
            }
        }
    }
}
