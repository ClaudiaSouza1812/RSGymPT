﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace RSGymPT
{
    internal class UserUtility
    {
        internal static void RunRSGymProgram()
        {
            // Show the login menu
            Dictionary<string, string> loginMenu = ShowLoginMenu();

            // Create inicial 2 usersList
            List<User> usersList = User.CreateusersList();

            // Create initial 3 PTs
            List<PersonalTrainer> personalTrainersList = PersonalTrainer.CreatepersonalTrainersList();

            // Create a new user
            User user = null;

            // Create a list of ordersList
            List<Order> ordersList = new List<Order>();

            
            string loginAction;
            int loginKey;

            do
            {
                loginKey = GetUserChoice("login", string.Empty);
                loginAction = ValidateLoginMenu(loginMenu, loginKey);

                if (loginAction == "Sair")
                {
                    ShowLogo("end", "Obrigada");
                    return;
                }

                if (loginAction == "Login")
                {
                    user = User.LogInUser(usersList);
                }

            } while (loginAction != "Sair" && user == null);
            
            // Show RSGymPT logo
            ShowLogo("begin", user.Name);
            

            Dictionary<string, Dictionary<string, string>> mainMenu = ShowMainMenu(user.Name);
            
            int menuKey;
            string[] menuAction;

            do
            {
                menuKey = GetUserChoice("main", user.Name);
                menuAction = ValidateMainMenu(mainMenu, menuKey);

                switch (menuAction[0])
                {
                    case "Pedido":
                        switch (menuAction[1]) 
                        {
                            case "Registar":
                                ordersList = Order.CreateOrder(personalTrainersList, user);
                                break;

                            case "Alterar":
                                if (ordersList.Count == 0)
                                {
                                    RSGymUtility.WriteMessage("Não existem pedidos para alterar.", "", "\n");
                                    RSGymUtility.PauseConsole();
                                    break;
                                }
                                ordersList = Order.ChangeOrder(ordersList, personalTrainersList, user);
                                break;

                            case "Cancelar":
                                if (ordersList.Count == 0)
                                {
                                    RSGymUtility.WriteMessage("Não existem pedidos para cancelar.", "", "\n");
                                    RSGymUtility.PauseConsole();
                                    break;
                                }
                                ordersList = Order.CancelOrder(ordersList, personalTrainersList, user);
                                break;

                            case "Listar":
                                Order.ListOrdersByUser(user);
                                KeepGoing();
                                break;

                            case "Terminar":
                                if (ordersList.Count == 0)
                                {
                                    RSGymUtility.WriteMessage("Não existem pedidos para terminar.", "", "\n");
                                    RSGymUtility.PauseConsole();
                                    break;
                                }
                                ordersList = Order.FinishOrder(ordersList, personalTrainersList, user);
                                break;
                        }
                        break;
                    case "Personal Trainer":
                        switch (menuAction[1])
                        {
                            case "Pesquisar":
                                do
                                {
                                    PersonalTrainer personalTrainer = PersonalTrainer.FindPersonalTrainerByCode(personalTrainersList, user.Name);
                                    PersonalTrainer.ShowPersonalTrainer(personalTrainer);

                                } while (KeepGoing() == "s");
                                break;
                            case "Listar":
                                PersonalTrainer.ListpersonalTrainersList(personalTrainersList);
                                break;
                        }
                        break;
                    case "Utilizador":
                        if (menuAction[1] == "Listar")
                        {
                            User.ListUser(usersList);
                        }
                        break;
                default:
                    RSGymUtility.WriteMessage("Escolha um número válido.");
                    break;
                }
            } while (menuAction[1] != "Logout");

            ShowLogo("end", user.Name);
            
        }


        internal static string KeepGoing()
        {
            RSGymUtility.WriteMessage("Continuar? (s/n): ", "\n");
            string answer = Console.ReadLine().ToLower();

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


        internal static Dictionary<string, Dictionary<string, string>> ShowMainMenu(string userName)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("RSGymPT Menu de navegação", "", "\n\n");
            RSGymUtility.WriteMessage($"{userName}, Digite o número da \nopção desejada e aperte 'Enter'", "", "\n\n");

            Dictionary<string, Dictionary<string, string>> mainMenu = new Dictionary<string, Dictionary<string, string>>()
            {
                { "Pedido", new Dictionary<string, string>()
                    {
                        {"1", "Registar" },
                        {"2", "Alterar" },
                        {"3", "Cancelar" },
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


        // Get user menu number choice
        internal static int GetUserChoice(string chosenMenu, string userName)
        {
            int menuNumber;
            bool status;
            do
            {
                Console.Clear();

                GetMenu(chosenMenu, userName);

                RSGymUtility.WriteMessage("Digite o número da opção desejada: ", "\n");
                string answer = Console.ReadLine();

                status = int.TryParse(answer, out menuNumber);

                if (!status)
                {
                    RSGymUtility.WriteMessage("Digite um número válido.", "\n");
                }

            } while (!status);

            return menuNumber;
        }

        internal static void GetMenu(string menu, string userName)
        {
            if (menu == "login")
            {
                ShowLoginMenu();
            }
            else
            {
                ShowMainMenu(userName);
            }
        }


        // Check if the input is a valid choice
        internal static string ValidateLoginMenu(Dictionary<string, string> loginMenu, int key)
        {
            string action;
            bool status;

            status = loginMenu.TryGetValue(key.ToString(), out action);

            if (status)
            {
                return action;
            }

            RSGymUtility.WriteMessage($"Escolha um número válido.", "\n", "\n");

            RSGymUtility.PauseConsole();

            return "Error";
        }

        internal static string[] ValidateMainMenu(Dictionary<string, Dictionary<string, string>> mainMenu, int menuKey)
        {
            string[] menuSubmenu = { null, null};

            foreach (KeyValuePair<string, Dictionary<string, string>> menu in mainMenu)
            {
                bool status = menu.Value.TryGetValue(menuKey.ToString(), out string action);

                if (status)
                {
                    menuSubmenu[0] = menu.Key;
                    menuSubmenu[1] = action;

                    return menuSubmenu;
                }
            }

            return menuSubmenu;
        }


        // Show RSGymPT logo
        internal static void ShowLogo(string status, string userName)
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

            ShowLogoMessage(status, userName);

            Console.ForegroundColor = ConsoleColor.White;
        }

        // Show RSGymPT logo message
        internal static void ShowLogoMessage(string status, string userName)
        {
            string message01 = $"Bem vindo(a)! {userName}!\n\tVamos treinar?";
            string message02 = $"{userName}, até a próxima!";

            if (status == "begin")
            {
                RSGymUtility.WriteMessage($"{message01.PadLeft(15 - (message01.Length / 2) + message01.Length, ' ')}", "", "\n\n");

                RSGymUtility.PauseConsole();
            }
            else
            {
                RSGymUtility.WriteMessage($"{message02.PadLeft(15 - (message02.Length / 2) + message02.Length, ' ')}", "", "\n");

                RSGymUtility.TerminateConsole();
            }
        }

    }
}



