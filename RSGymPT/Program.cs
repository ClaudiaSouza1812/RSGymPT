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

            User.MakeUser();
            
            Dictionary<string, string> rsgymMenu = LoginUtility.ShowRsgymMenu();
            string action, key;

            do
            {
                key = LoginUtility.GetLoginChoice();
                action = LoginUtility.CheckLoginChoice(rsgymMenu, key);

                if (action == "Login")
                {
                    Login.LogInUser();
                }
                else
                {
                    RSGymUtility.WriteMessage($"Opção inválida. Por favor escolha das opções: ({rsgymMenu.Keys})");
                }

            } while (action != "Sair");
            


            



            //Login.LogInUser();
            
            //User.ListUser(users);

            RSGymUtility.TerminateConsole();
        
        }
    }
}
