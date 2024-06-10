/*
    CLASS ELEMENTS:
        Attributes or fields    = variáveis privadas da classe (suporte às propriedades)
        Properties              = caraterísticas
        Methods                 = funcionalidades
        Constructors            = funcionalidade invocada aquando da criação do objeto
        Destructor              = funcionalidade que permite indicar como é que o objeto é destruído
    EXEMPLO
        Classe: Produto
        Objects (instâncias da classe): Produto1, Produto2, Produto3...
        Properties: Nome, Cor, Unidade, ...
        Methods: Inserir, Pesquisar, Editar, Apagar, ...
        Constructor: Cor = verde
        Destructor (log): informar que o objeto vai ser destruído

 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
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

        #region Classic properties 1.0
        /* 
        Exemplo de uma propriedade usando Classic properties

        internal double Value01
        {
            get { return value01; }     // Ler o valor da propriedade
            set { value01 = value; }    // escrever o valor da propriedade
        }
        */

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
        internal static List<User> CreateusersList()
        {
            List<User> usersList = new List<User>()
            {
                new User("Claudia Simone de Souza", new DateTime(1992, 12, 18), "clasi", "12345678"),
                new User("Paula de Fátima Vallim Magalhães", new DateTime(1984, 12, 08), "paufa", "87654321")
            };

            return usersList;
        }


        /*internal string EncryptPassword(string password)
        {
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            };
            
        }*/

        // Function to login and return the user
        internal static User LogInUser(List<User> usersList)
        {
            string userName = AskUserName();

            if (!CheckUserName(usersList, userName))
            {
                RSGymUtility.WriteMessage("Nome de utilizador inválido ou inexistente.", "", "\n");

                RSGymUtility.PauseConsole();
                return null;
            }

            string password = AskUserPassword();

            User user = ValidateUser(usersList, userName, password);

            if (user == null)
            {
                RSGymUtility.WriteMessage("Palavra-passe inválida.", "", "\n");

                RSGymUtility.PauseConsole();
                return user;
            }

            return user;
        }

        // Function to ask and return the user username
        internal static string AskUserName()
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Login", "", "\n\n");

            RSGymUtility.WriteMessage("Insira seu nome de utilizador: ", "", "\n");

            string userName = Console.ReadLine().ToLower();
            return userName;
        }

        // Function to check if the username is valid, returning true or false
        internal static bool CheckUserName(List<User> usersList, string userName)
        {
            bool isValid = usersList.Any(u => u.UserName == userName);
            return isValid;
        }

        // Function to ask and return the user password 
        internal static string AskUserPassword()
        {
            RSGymUtility.WriteMessage("Insira sua palavra-passe: ", "", "\n");

            string password = Console.ReadLine().ToLower();
            return password;
        }

        // Function to validate and return the user 
        internal static User ValidateUser(List<User> usersList, string userName, string password)
        {
            User user = usersList.FirstOrDefault(u => u.UserName == userName && u.Password == password);
            return user;
        }

        // Method to list usersList properties
        internal static void ListUser(List<User> list) 
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Users - List", "\n", "\n\n");

            foreach (User user in list)
            {
                RSGymUtility.WriteMessage($"{user.FullUser}", "\n", "\n");
            }
            RSGymUtility.PauseConsole();
        }


        #endregion
    }
}
