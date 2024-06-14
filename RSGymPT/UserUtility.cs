using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utility;
using static System.Collections.Specialized.BitVector32;

namespace RSGymPT
{
    internal class UserUtility
    {
        // Create 2 inicial users
        private static List<User> usersList = User.CreateUsersList();

        // Create 3 initial PTs
        private static List<PersonalTrainer> personalTrainersList = PersonalTrainer.CreatePersonalTrainersList();

        // Create a new user
        private static User user = null;

        // Create a list of orders
        private static List<Order> ordersList = new List<Order>();

        // Run the RSGymPT program
        internal static void RunRSGymProgram()
        {
            RunLoginMenu();
        }

        // Method to run the login menu
        internal static void RunLoginMenu()
        {
            // Show the login menu
            Dictionary<string, string> loginMenu = ShowLoginMenu();

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

            // Show the RSGymPT logo
            ShowLogo("begin", user.Name);

            // Run the main menu
            RunMainMenu();
        }

        // Method to run the main menu
        internal static void RunMainMenu()
        {
            // Show the main menu
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
                        RunOrderSubmenu(menuAction[1]);
                        break;
                    case "Personal Trainer":
                        RunPersonalTrainerSubmenu(menuAction[1]);
                        break;
                    case "Utilizador":
                        if (menuAction[1] == "Listar")
                        {
                            User.ListUser(usersList, user.Name);
                        }
                        break;
                    default:
                        RSGymUtility.WriteMessage("Escolha um número válido.");
                        break;
                }
            } while (menuAction[1] != "Logout");

            ShowLogo("end", user.Name);
        }

        // Method to run the order menu
        internal static void RunOrderSubmenu(string menuAction)
        {
            switch (menuAction)
            {
                case "Registar":
                    ordersList = Order.CreateOrder(personalTrainersList, user);
                    break;
                case "Alterar":
                    if (ValidateOrderList("Alterar"))
                    {
                        ordersList = Order.ChangeOrder(ordersList, personalTrainersList, user);
                    };
                    break;
                case "Eliminar":
                    if (ValidateOrderList("Eliminar"))
                    {
                        ordersList = Order.DeleteOrder(ordersList, personalTrainersList, user);
                    }
                    break;
                case "Listar":
                    Order.ListOrdersByUser(user);
                    KeepGoing();
                    break;

                case "Terminar":
                    if (ValidateOrderList("Terminar"))
                    {
                        ordersList = Order.FinishOrder(user);
                    }
                    break;
            }
        }

        internal static bool ValidateOrderList(string action)
        {
            bool status = false;

            if (ordersList == null || ordersList.Count == 0)
            {
                switch (action)
                {
                    case "Alterar":
                        RSGymUtility.WriteMessage("Não existem pedidos para alterar.", "", "\n");
                        RSGymUtility.PauseConsole();
                        break;
                    case "Eliminar":
                        RSGymUtility.WriteMessage("Não existem pedidos para eliminar.", "", "\n");
                        RSGymUtility.PauseConsole();
                        break;
                    case "Terminar":
                        RSGymUtility.WriteMessage("Não existem pedidos para terminar.", "", "\n");
                        RSGymUtility.PauseConsole();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                status = true;
            }
            return status;
        }

        // Method to run the personal trainer menu
        internal static void RunPersonalTrainerSubmenu(string menuAction)
        {
            switch (menuAction)
            {
                case "Pesquisar":
                    do
                    {
                        PersonalTrainer personalTrainer = PersonalTrainer.FindPersonalTrainerByCode(personalTrainersList, user.Name);
                        PersonalTrainer.ShowFullPersonalTrainer(personalTrainer);

                    } while (KeepGoing());
                    break;
                case "Listar":
                    PersonalTrainer.ListFullPersonalTrainers(personalTrainersList, user);
                    break;
            }
        }

        // Function to show and return the login menu
        internal static Dictionary<string, string> ShowLoginMenu()
        {
            Console.Clear();

            RSGymUtility.WriteTitle("RSGymPT Login Menu", "", "\n\n");
            RSGymUtility.WriteMessage($"Digite o número da opção e aperte 'Enter'", "", "\n\n");

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


        // Get user menu number choice
        internal static int GetUserChoice(string chosenMenu, string userName)
        {
            int menuNumber;
            bool status;
            do
            {
                Console.Clear();

                GetMenu(chosenMenu, userName);

                RSGymUtility.WriteMessage("Número: ", "\n", "");
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

            return string.Empty;
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

        internal static bool KeepGoing()
        {
            RSGymUtility.WriteMessage("Continuar? (s/n): ", "\n");
            string answer = Console.ReadLine().ToLower();
            
            if (answer == "s")
            {
                return true;
            }
            if (answer == "n")
            {
                return false;
            }
            return true;
        }

        internal static bool CheckDelete()
        {
            RSGymUtility.WriteMessage("Tem certeza que deseja eliminar o pedido? (s/n): ", "\n");
            string answer = Console.ReadLine().ToLower();

            if (answer == "s")
            {
                return true;
            }
            if (answer == "n")
            {
                return false;
            }
            return false;
        }

        internal static string EncryptPassword(string password)
        {
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            };

        }

        // Function to ask and return the user username
        internal static string AskUserName()
        {
            RSGymUtility.WriteMessage("Insira seu nome de utilizador: ", "", "\n");

            string userName = Console.ReadLine().ToLower();
            return userName;
        }

        

        // Function to ask and return the user password 
        internal static string AskUserPassword()
        {
            RSGymUtility.WriteMessage("Insira sua palavra-passe: ", "", "\n");

            StringBuilder password = new StringBuilder();
            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Enter)
                {
                    password.Append(key.KeyChar);
                    RSGymUtility.WriteMessage("*");
                }

            } while (key.Key != ConsoleKey.Enter);

            return password.ToString();
        }


        
    }
}



