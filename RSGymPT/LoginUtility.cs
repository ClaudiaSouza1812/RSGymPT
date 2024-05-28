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

            RSGymUtility.WriteTitle("RSGymPT Menu", "", "\n\n");
            RSGymUtility.WriteMessage("Escolha o número de uma das seguintes opções: ", endMessage: "\n\n");

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

        // Show RSGymPT logo
        internal static void ShowLogo(string status)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("RSGymPT APP", "", "\n\n");

            Console.ForegroundColor = ConsoleColor.Yellow;

            string[] logo =
            {
                "-------------------------------",
                "|  _____    _____   ___       |",
                "| | |   |  |  ___| | _/|      |",
                "| | |___|  | |___  |___|      |",
                "| | |\\\\    |___  |  | |       |",
                "| | | \\\\    ___| |  | |´ ' `\\ |",
                "| |_|  \\\\  |_____|  |_______/ |",
                "|                             |",
                "-------------------------------"
            };

            foreach (string item in logo)
            {
                RSGymUtility.WriteMessage($"{item}\n");
            }

            ShowLogoMessage(status);

            Console.ForegroundColor = ConsoleColor.White;
        }

        // Show RSGymPT logo message
        internal static void ShowLogoMessage(string status)
        {
            string message01 = "Bem vindo! Vamos treinar?";
            string message02 = "Até a próxima!";

            if (status == "begin")
            {
                RSGymUtility.WriteMessage($"{message01.PadLeft(15 - (message01.Length / 2) + message01.Length, ' ')}", "", "\n\n");

                RSGymUtility.PauseConsole();
            }
            else
            {
                RSGymUtility.WriteMessage($"{message02.PadLeft(15 - (message02.Length / 2) + message02.Length, ' ')}", "", "\n\n");
            }
        }

        // Get user login choice
        internal static string GetLoginChoice()
        {
            string loginNumber;
            bool status;
            do
            {
                ShowRsgymMenu();

                RSGymUtility.WriteMessage("Digite o número da opção desejada: ", "\n");

                loginNumber = Console.ReadLine();

                status = CheckInt(loginNumber);

            } while (!status);

            return loginNumber;
        }

        // Check if the input is a valid choice
        internal static string CheckLoginChoice(Dictionary<string, string> loginMenu, string key)
        {
            string action;
            bool status;

            status = loginMenu.TryGetValue(key, out action);

            if (status)
            {
                return action;
            }

            RSGymUtility.WriteMessage($"Escolha um número válido: (1) ou (2).", "\n", "\n");

            RSGymUtility.PauseConsole();

            return null;
        }

        internal static bool CheckInt(string loginNumber)
        {
            bool status = int.TryParse(loginNumber, out int number);
            return status;
        }


    }
}
