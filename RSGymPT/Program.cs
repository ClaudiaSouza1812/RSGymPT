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
            // Method to output characters encoded to UTF-8 
            RSGymUtility.SetUnicodeConsole();

            List<User> users = User.MakeUser();

            LoginUtility.ShowLogo("begin");

            Dictionary<string, string> loginMenu = LoginUtility.ShowLoginMenu();
            string action, key;

            do
            {
                key = LoginUtility.GetLoginChoice();
                action = LoginUtility.CheckLoginChoice(loginMenu, key);

                if (action == "Login")
                {
                    Login.LogInUser(users);
                }
                
            } while (action != "Sair");

            LoginUtility.ShowLogo("end");

            




            //Login.LogInUser();

            //User.ListUser(users);

            RSGymUtility.TerminateConsole();
        
        }
    }
}
