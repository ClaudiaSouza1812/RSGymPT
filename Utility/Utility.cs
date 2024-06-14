// Purpose: Create a class with utility methods to be used in the project.
// The class contains methods to set the console to display characters encoded to UTF-8, to show a stylish title, to show a message with skip lines at the beginning and end of it, and to terminate the console with a stylish message.
// The class also contains a method to pause the console.
// Restriction: The class is public
// Version 1.0

// Libraries to be used in the class
using System;


namespace Utility
{
    public class RSGymUtility
    {
        // Method to ensure that characters from any language, including special characters, can be correctly displayed in the console.
        // This is particularly useful when working with text that contains characters from languages other than English, as UTF-8 supports a wide range of characters and symbols.
        public static void SetUnicodeConsole()
        {
            // Console.WriteLine("á Á à À ã Ã â Â ç Ç º ª");
            Console.OutputEncoding = System.Text.Encoding.UTF8;
        }

        // Method to show a stylish title
        public static void WriteTitle(string title, string beginTitle = "", string endTitle = "")
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"{beginTitle}{new string('-', 44)}\n{title.ToUpper().PadLeft(22 - (title.Length / 2) + title.Length , ' ')}\n{new string('-', 44)}{endTitle}");
            Console.ForegroundColor = ConsoleColor.White;   // Reset original color
        }

        // Method to show a message setted with skip lines at the beginning and end of it
        public static void WriteMessage(string message, string beginMessage = "", string endMessage = "")
        {
            Console.Write($"{beginMessage}{message}{endMessage}");
        }

        // Method to terminate the console with a stylish message
        public static void TerminateConsole()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\n\nPrime qualquer tecla para terminares.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
            Console.Clear();
        }

        // Method to pause the console
        public static void PauseConsole()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nPrime qualquer tecla para continuar.");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();
        }
    }
}
