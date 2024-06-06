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
        internal int NextId { get; set; } = 1;
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
        /* 
        Exemplo de uma propriedade usando Bodied-expression properties
        internal double Value02
        {
            get => value02;         // => lambda operator
            set => value02 = value;
        }
        */

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

        // Method to create 2 initial users
        internal static List<User> CreateUser()
        {
            List<User> users = new List<User>()
            {
                new User("Claudia Simone de Souza", new DateTime(1992, 12, 18), "clasi", "12345678"),
                new User("Paula de Fátima Vallim Magalhães", new DateTime(1984, 12, 08), "paufa", "87654321")
            };

            return users;
        }


        /*internal string EncryptPassword(string password)
        {
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            };
            
        }*/

        internal static User LogInUser(List<User> users)
        {
            User user = new User();

            string userName = AskUserName();

            bool isValidUser = CheckUserName(users, userName);

            if (isValidUser)
            {
                string password = AskUserPassword();

                user = CheckUserPassword(users, password);

                return user;
            }

            return null;
        }

        internal static string AskUserName()
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Login", "", "\n\n");

            RSGymUtility.WriteMessage("Insira seu nome de utilizador: ", "", "\n");

            string userName = Console.ReadLine().ToLower();

            return userName;
        }


        internal static bool CheckUserName(List<User> users, string userName)
        {
            foreach (User user in users)
            {
                if (user.UserName == userName)
                {
                    //UserName = userName;

                    return true;
                }
            }

            RSGymUtility.WriteMessage("Nome de utilizador inválido.", "", "\n");

            RSGymUtility.PauseConsole();

            return false;
        }


        internal static string AskUserPassword()
        {
            RSGymUtility.WriteMessage("Insira sua palavra-passe: ", "", "\n");

            string password = Console.ReadLine().ToLower();

            return password;

        }

        internal static User CheckUserPassword(List<User> users, string password)
        {
            foreach (User user in users)
            {
                if (user.Password == password)
                {
                    return user;
                }
            }

            RSGymUtility.WriteMessage("Palavra-passe inválida.", "", "\n");

            RSGymUtility.PauseConsole();

            return null;
        }


        internal static void ListUser(List<User> list) 
        {
            RSGymUtility.WriteTitle("Users - List", "\n", "\n\n");

            foreach (User user in list)
            {
                RSGymUtility.WriteMessage($"Id: {user.UserId}\nNome: {user.Name}\nData de nascimento: {user.Birth.ToShortDateString()}", "", "\n\n");
            }
        }



        #endregion

        #region Destructor

        #endregion
    }
}
