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

        // Method to create fictitious users
        internal static List<User> CreateUser()
        {
            List<User> users = new List<User>(); 

            User user01 = new User();

            user01.Name = "Claudia Simone de Souza";
            user01.Birth = new DateTime(1992, 12, 18);
            user01.UserName = "clasi";
            user01.Password = "12345678";

            users.Add(user01);

            User user02 = new User("Paula de Fátima Vallim Magalhães", new DateTime(1984, 12, 08), "paufa", "87654321");

            users.Add(user02);

            return users;
        }


        internal static bool LogInUser(List<User> users)
        {
            string userName = AskUserName();

            bool isValidUser = CheckUserName(users, userName);

            bool isValidPassword = false;

            if (isValidUser)
            {
                string password = AskUserPassword(users);

                isValidPassword = CheckUserPassword(users, password);
            }

            if (isValidPassword)
            {
                return true;
            }

            return false;
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


        internal static string AskUserPassword(List<User> users)
        {
            RSGymUtility.WriteMessage("Insira sua palavra-passe: ", "", "\n");

            string password = Console.ReadLine().ToLower();

            return password;

        }

        internal static bool CheckUserPassword(List<User> users, string password)
        {
            foreach (User user in users)
            {
                if (user.Password == password)
                {
                    //Password = password;

                    return true;
                }
            }

            RSGymUtility.WriteMessage("Palavra-passe inválida.", "", "\n");

            RSGymUtility.PauseConsole();

            return false;
        }


        internal static void ListUser(List<User> list) 
        {
            RSGymUtility.WriteTitle("Users - List", "\n", "\n\n");

            foreach (User item in list)
            {
                RSGymUtility.WriteMessage($"Id: {item.Id}\nNome: {item.Name}\nData de nascimento: {item.Birth.ToShortDateString()}", "", "\n\n");
            }
        }



        #endregion

        #region Destructor

        #endregion
    }
}
