using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace RSGymPT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //
            RSGymUtility.SetUnicodeConsole();

            User.MakeUser();

            Dictionary<string, string> loginMenu = LoginUtility.ShowLoginMenu();

            string key = LoginUtility.GetLoginChoice();

            string action = LoginUtility.CheckLoginChoice(loginMenu, key);


            



            //Login.LogInUser();
            
            //User.ListUser(users);

            RSGymUtility.TerminateConsole();
        
        }
    }
}
