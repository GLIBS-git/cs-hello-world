namespace SyntaxDemo
{
    public class Types
    {
        public static void Menu()
        {
            // Uncomment the test you want to run:
            Types.StringLiterals();
        }
        public static void StringLiterals()
        {
            Console.WriteLine("====== String literals ======");
            // string s = "String: C:\python\name.txt"; // This will not compile because of the unrecognized escape sequence \p
            string s = @"Raw string: C:\python\name.txt"; // \ does not work as escape symbol
            Console.WriteLine(s); // Raw string: C:\python\name.txt          
            s = @"Multiline text 1!
Multiline text 2!";
            Console.WriteLine(s); // Multiline text 1!
                                  // Multiline text 2!
        } 
    }
}
