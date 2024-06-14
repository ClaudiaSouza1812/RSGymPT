using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace RSGymPT
{
    internal class PersonalTrainerUtility
    {
        // Function to ask and return the PT code
        internal static string AskPtCode()
        {
            RSGymUtility.WriteMessage("Digite o código do PT: ", "\n\n", "");

            string ptCode = Console.ReadLine().ToUpper();
            return ptCode;
        }
    }
}
