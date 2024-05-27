using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace RSGymPT
{
    internal class LoginUtility
    {
        internal static Dictionary<string, string> ShowRsgymMenu()
        {
            Console.Clear();

            PrintLogo();

            RSGymUtility.WriteTitle("RSGymPT Menu", "", "\n\n");

            RSGymUtility.WriteMessage("Bem vindo! Vamos malhar hoje? ", "", "\n\n");

            Dictionary<string, string> loginMenu = new Dictionary<string, string>()
            {
                {"1", "Login" },
                {"2", "Sair" }
            };

            foreach (KeyValuePair<string, string> item in loginMenu)
            {
                RSGymUtility.WriteMessage($"({item.Key}) - {item.Value}", "", "\n");
            }

            return loginMenu;
        }

        internal static void PrintLogo()
        {
            string[] logo =
            {
                " --- ",
                "| _/|",
                " ---      __",
                " | |     |  |",
                " | |´***`|  |",
                " |__________|"
            };

            foreach (string item in logo)
            {
                RSGymUtility.WriteMessage($"{item}\n");
            }
        }

        internal static string GetLoginChoice()
        {
            string loginNumber;
            do
            {
                Console.Clear();

                ShowRsgymMenu();

                RSGymUtility.WriteMessage("Digite o número da opção desejada: ", "\n", "\n");

                loginNumber = Console.ReadLine();

            } while (!CheckInt(loginNumber));

            return loginNumber;
        }

        internal static string CheckLoginChoice(Dictionary<string, string> loginMenu, string key)
        {
            string action;
            bool status;

            status = loginMenu.TryGetValue(key, out action);

            if (status)
            {
                return action;
            }
            else
            {
                RSGymUtility.WriteMessage($"Escolha um número válido: (1) ou (2).", "", "\n");
                return GetLoginChoice();
            }
        }

        internal static bool CheckInt(string loginNumber)
        {
            if (int.TryParse(loginNumber, out int number))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
