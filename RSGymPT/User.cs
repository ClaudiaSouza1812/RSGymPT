// Purpose: Class to create and manage users in the system.
// The class contains properties, constructors, and methods to create, validate, and list users.
// The class also contains a method to create and return 2 initial users.
// Restriction: The class is internal
// Version 1.0

// Libraries to be used in the class
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;


namespace RSGymPT
{
    internal class User
    {
        #region Fields (properties, private variables)
        
        #endregion

        #region Properties (public or internal)
        #region Auto-implemented properties 2.0
        
        internal int UserId { get; }
        private static int NextId { get; set; } = 1;
        internal string Name { get; set; }
        internal DateTime Birth { get; set; }
        internal string UserName { get; set; }
        internal string Password { get; set; }

        #endregion

        #region Bodied-expression properties 3.0
        // Property to show the full user
        internal string FullUser => $"(Id): {UserId}\n(Nome): {Name}\n(Data de nascimento): {Birth.ToShortDateString()}\n(Nome de utilizador): {UserName}";

        #endregion

        #endregion

        #region Constructors (public or internal)
        
        // Default constructor substitute
        internal User()
        {
            UserId = NextId++;
            Name = string.Empty;
            Birth = DateTime.MinValue;
            UserName = string.Empty;
            Password = string.Empty;
        }
        
        // Second constructor with mandatory parameters
        internal User(string name, DateTime birth, string userName, string password)
        {
            UserId = NextId++;
            Name = name;
            Birth = birth;
            UserName = userName;
            Password = password;
        }

        #endregion

        #region Methods (public or internal)

        // Function to create and return 2 initial usersList
        internal static List<User> CreateUsersList()
        {
            List<User> usersList = new List<User>()
            {
                new User("Claudia Souza", new DateTime(1992, 12, 18), "claudia", UserUtility.EncryptPassword("12345678")),
                new User("Paula Magalhães", new DateTime(1984, 12, 08), "paula", UserUtility.EncryptPassword("87654321"))
            };
            return usersList;
        }

        // Function to login and return the user
        internal static User LogInUser(List<User> usersList)
        {
            RSGymUtility.WriteTitle("Login", "", "\n\n");

            string userName = UserUtility.AskUserName();

            if (!CheckUserName(usersList, userName))
            {
                RSGymUtility.WriteMessage("Nome de utilizador inválido ou inexistente.", "", "\n");

                RSGymUtility.PauseConsole();
                return null;
            }

            string password = UserUtility.AskUserPassword();

            User user = ValidateUser(usersList, userName, UserUtility.EncryptPassword(password));

            if (user == null)
            {
                RSGymUtility.WriteMessage("Palavra-passe inválida.", "\n", "\n");

                RSGymUtility.PauseConsole();
                return user;
            }

            return user;
        }

        // Function to check if the username is valid, returning true or false
        internal static bool CheckUserName(List<User> usersList, string userName)
        {
            bool isValid = usersList.Any(u => u.UserName == userName);
            return isValid;
        }

        // Function to validate and return the user 
        internal static User ValidateUser(List<User> usersList, string userName, string password)
        {
            User user = usersList.FirstOrDefault(u => u.UserName == userName && u.Password == password);
            return user;
        }

        // Method to list users properties
        internal static void ListUser(List<User> list, string userName) 
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Lista de Utilizadores", "\n", "\n\n");
            RSGymUtility.WriteMessage($"{userName}, Utilizadores cadastrados: ", "", "\n\n");

            foreach (User user in list)
            {
                RSGymUtility.WriteMessage($"{user.FullUser}", "\n", "\n");
            }
            RSGymUtility.PauseConsole();
        }

        #endregion
    }
}
