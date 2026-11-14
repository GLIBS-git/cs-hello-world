using Microsoft.Extensions.Configuration;

namespace cs_hello_world
{
    internal class Program
    {
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static readonly IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();
        static void Main(string[] args)
        {
            Console.CancelKeyPress += (_sender, _eventArgs) =>
            {
                Console.WriteLine("\n[System] Cancel key pressed. Shutting down softly...");
                _eventArgs.Cancel = true;
                Program.cts.Cancel();
            };
            AppDomain.CurrentDomain.ProcessExit += (_sender, _eventArgs) =>
            {
                if (!Program.cts.IsCancellationRequested)
                {
                    Console.WriteLine("\n[System] Process exit signal received. Shutting down softly...");
                    Program.cts.Cancel();
                }
            };
            // Uncomment the test you want to run:
            SyntaxDemo.Menu();
        }
    }
}
