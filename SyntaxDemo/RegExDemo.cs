using System.Text.RegularExpressions;

namespace SyntaxDemo
{
    public class RegExDemo
    {
        public static void Menu()
        {
            // Uncomment the test you want to run:
            MatchFunction();
        }
        public static void MatchFunction()
        {
            
            Console.WriteLine(Regex.IsMatch("123", "123"));
        }
    }
}
