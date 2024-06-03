using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace RSGymPT
{
    internal class UserUtility
    {
        internal static void StartRSGymProgram(List<User> users, List<PersonalTrainer> personalTrainers)
        {
            ShowLogo("begin");

            Dictionary<string, string> loginMenu = ShowLoginMenu();
            string loginAction, loginKey;
            User user = new User();

            do
            {
                loginKey = GetChoice("login");
                loginAction = CheckLoginChoice(loginMenu, loginKey);

                if (loginAction == "Login")
                {
                    user = User.LogInUser(users);
                }

            } while (loginAction != "Sair" && user == null);

            if (loginAction == "Sair")
            {
                ShowLogo("end");
                RSGymUtility.TerminateConsole();
                return;
            }
            

            
            Dictionary<string, Dictionary<string, string>> mainMenu = ShowMainMenu();
            string menuKey;
            string[] menuAction;

            do
            {
                menuKey = GetChoice("main");
                menuAction = CheckMainMenuChoice(mainMenu, menuKey);

                switch (menuAction[0])
                {
                    case "Pedido":
                        switch (menuAction[1]) 
                        {
                            case "Registar":
                                do
                                {
                                    Order.AddOrder(personalTrainers, user);

                                } while (KeepGoing() == "s");
                                break;

                            case "Alterar":
                                break;

                            case "Eliminar":
                                break;

                            case "Listar":
                                Order.ListOrdersByUser(user);
                                break;

                            case "Terminar":
                                break;
                            default:
                                RSGymUtility.WriteMessage("Escolha um número válido.");
                                break;
                        }
                        break;
                    case "Personal Trainer":
                        switch (menuAction[1])
                        {
                            case "Pesquisar":
                                do
                                {
                                    PersonalTrainer.FindPersonalTrainer(personalTrainers);

                                } while (KeepGoing() == "s");
                                break;
                            case "Listar":
                                break;
                            default:
                                RSGymUtility.WriteMessage("Escolha um número válido.");
                                break;
                        }
                        break;
                    case "Utilizador":
                        if (menuAction[1] == "Listar")
                        {
                                
                        }
                        else
                        {
                            RSGymUtility.WriteMessage("Escolha um número válido.");
                        }
                        break;
                }
            } while (menuAction[1] != "Logout");

            ShowLogo("end");
            
        }


        internal static string KeepGoing()
        {
            string answer;

            RSGymUtility.WriteMessage("Continuar? (s/n): ", "\n");
            answer = Console.ReadLine().ToLower();

            return answer;
        }



        internal static Dictionary<string, string> ShowLoginMenu()
        {
            Console.Clear();

            RSGymUtility.WriteTitle("RSGymPT Login Menu", "", "\n\n");

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


        internal static Dictionary<string, Dictionary<string, string>> ShowMainMenu()
        {
            Console.Clear();

            RSGymUtility.WriteTitle("RSGymPT Main Menu", "", "\n\n");

            Dictionary<string, Dictionary<string, string>> mainMenu = new Dictionary<string, Dictionary<string, string>>()
            {
                { "Pedido", new Dictionary<string, string>()
                    {
                        {"1", "Registar" },
                        {"2", "Alterar" },
                        {"3", "Eliminar" },
                        {"4", "Listar" },
                        {"5", "Terminar" }
                    }
                },
                { "Personal Trainer", new Dictionary<string, string>()
                    {
                        {"6", "Pesquisar" },
                        {"7", "Listar" }
                    }
                },
                { "Utilizador", new Dictionary<string, string>()
                    {
                        {"8", "Listar" },
                        {"9", "Logout" }
                    }
                }
            };

            foreach (KeyValuePair<string, Dictionary<string, string>> menu in mainMenu)
            {
                RSGymUtility.WriteTitle($"{menu.Key}", "", "\n");

                foreach (KeyValuePair<string, string> subMenu in menu.Value)
                {
                    RSGymUtility.WriteMessage($"({subMenu.Key}) - {subMenu.Value}", "", "\n");
                }
            }

            return mainMenu;
        }


        // Get user choice
        internal static string GetChoice(string menu)
        {
            string loginNumber;
            bool status;
            do
            {
                Console.Clear();

                GetMenu(menu);

                RSGymUtility.WriteMessage("Digite o número da opção desejada: ", "\n");
                loginNumber = Console.ReadLine();

                status = CheckInt(loginNumber);

            } while (!status);

            return loginNumber;
        }


        internal static void GetMenu(string menu)
        {
            if (menu == "login")
            {
                ShowLoginMenu();
            }
            else
            {
                ShowMainMenu();
            }
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

            RSGymUtility.WriteMessage($"Escolha um número válido.", "\n", "\n");

            RSGymUtility.PauseConsole();

            return null;
        }

        internal static string[] CheckMainMenuChoice(Dictionary<string, Dictionary<string, string>> mainMenu, string menuKey)
        {
            string[] menuSubmenu = new string[2];
            string action = null;
            bool status = false;

            foreach (KeyValuePair<string, Dictionary<string, string>> menu in mainMenu)
            {
                status = menu.Value.TryGetValue(menuKey, out action);

                if (status)
                {
                    menuSubmenu[0] = menu.Key;
                    menuSubmenu[1] = action;

                    return menuSubmenu;
                }
            }

            
            RSGymUtility.WriteMessage($"Escolha um número válido.", "\n", "\n");

            RSGymUtility.PauseConsole();

            return null;
        }

        internal static bool CheckInt(string loginNumber)
        {
            bool status = int.TryParse(loginNumber, out int number);
            return status;
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
                RSGymUtility.WriteMessage($"{message02.PadLeft(15 - (message02.Length / 2) + message02.Length, ' ')}", "", "\n");
            }
        }

    }
}



