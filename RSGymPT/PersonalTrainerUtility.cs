using System;
// Purpose: Create a utility class to help the Personal Trainer class
// The class contains a method to ask and return the PT code
// Restriction: The class is internal
// Version 1.0


// Libraries to be used in the class
using Utility;

namespace RSGymPT
{
    internal class PersonalTrainerUtility
    {
        // Personal trainer helper function to ask and return the PT code
        internal static string AskPtCode()
        {
            RSGymUtility.WriteMessage("Digite o código do PT: ", "\n\n", "");
            string ptCode = Console.ReadLine().ToUpper();

            return ptCode;
        }
    }
}
