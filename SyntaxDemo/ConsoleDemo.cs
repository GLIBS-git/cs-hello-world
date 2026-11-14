namespace SyntaxDemo
{
    public class ConsoleDemo
    {
        public static void Menu()
        {
            // Uncomment the test you want to run:
            ConsoleDemo.Streams();
        }
        public static void Streams()
        {
            Console.WriteLine("Hello world!"); // stdout (default)
            Console.Out.WriteLine("Out: Hello world!"); // stdout (explicit)
            Console.Error.WriteLine("Error: Hello world!"); // stderr
        }
    }
}
