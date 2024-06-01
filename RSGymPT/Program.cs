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
            string loginAction, loginKey;

            do
            {
                loginKey = LoginUtility.GetChoice("login");
                loginAction = LoginUtility.CheckLoginChoice(loginMenu, loginKey);

                if (loginAction == "Login")
                {
                    Login.LogInUser(users);
                }
                
            } while (loginAction != "Sair");

            LoginUtility.ShowLogo("end");


            



            //Login.LogInUser();

            //User.ListUser(users);

            RSGymUtility.TerminateConsole();
        
        }
    }
}
