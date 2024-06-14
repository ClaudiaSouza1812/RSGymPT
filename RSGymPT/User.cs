using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Utility;


namespace RSGymPT
{
    internal class User
    {
        #region Fields (properties, private variables)
        /*
        variáveis internas da classe para serem usadas dentro das propriedades (Classic properties / Bodied-expression properties)
        */

        #endregion

        #region Properties (public or internal)
        #region Auto-implemented properties 2.0
        /* 
        Exemplo de uma propriedade usando Auto-implemented properties
        internal string Operator { get; set; } // propriedade no singular
        */

        internal int UserId { get; }
        private static int NextId { get; set; } = 1;
        internal string Name { get; set; }
        internal DateTime Birth { get; set; }
        internal string UserName { get; set; }
        internal string Password { get; set; }

        #endregion

       

        #region Bodied-expression properties 3.0
        internal string FullUser => $"(Id): {UserId}\n(Nome): {Name}\n(Data de nascimento): {Birth.ToShortDateString()}";


        #endregion
        #endregion

        #region Constructors (public or internal)
        // Fazer substituto do default constructor

        internal User()
        {
            UserId = NextId++;
            Name = string.Empty;
            Birth = DateTime.MinValue;
            UserName = string.Empty;
            Password = string.Empty;
        }
        // Fazer segundo construtor com inserção parâmetros obrigatórios

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
                new User("Claudia Souza", new DateTime(1992, 12, 18), "clasi", UserUtility.EncryptPassword("12345678")),
                new User("Paula Magalhães", new DateTime(1984, 12, 08), "paufa", UserUtility.EncryptPassword("87654321"))
            };
            return usersList;
        }

        // Function to login and return the user
        internal static User LogInUser(List<User> usersList)
        {
            RSGymUtility.WriteTitle("Login", "", "\n\n");

            string userName = UserUtility.AskUserName();

            if (!UserUtility.CheckUserName(usersList, userName))
            {
                RSGymUtility.WriteMessage("Nome de utilizador inválido ou inexistente.", "", "\n");

                RSGymUtility.PauseConsole();
                return null;
            }

            string password = UserUtility.AskUserPassword();

            User user = UserUtility.ValidateUser(usersList, userName, UserUtility.EncryptPassword(password));

            if (user == null)
            {
                RSGymUtility.WriteMessage("Palavra-passe inválida.", "\n", "\n");

                RSGymUtility.PauseConsole();
                return user;
            }

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
