// Purpose: Main program file for RSGymPT. This file contains the main method that runs the program.
// The program is a simple console application that allows users to create and manage personal trainers and users in a gym system.
// Restriction: The class is internal
// Version: 1.0

// Libraries to be used in the class
using Utility;

namespace RSGymPT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Method to output characters encoded to UTF-8 
            RSGymUtility.SetUnicodeConsole();

            // Method to run the program
            UserUtility.RunRSGymProgram();

        }
    }
}
