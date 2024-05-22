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

        internal int Id { get; }
        internal static int NextId { get; set; } = 1;
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
            Id = NextId++;
            Name = string.Empty;
            Birth = DateTime.MinValue;
            UserName = string.Empty;
            Password = string.Empty;
        }
        // Fazer segundo construtor com inserção parâmetros obrigatórios

        internal User(string name, DateTime birth, string userName, string password)
        {
            Id = NextId++;
            Name = name;
            Birth = birth;
            UserName = userName;
            Password = password;
        }
        #endregion

        #region Methods (public or internal)

        internal static void ListUser(List<User> list) 
        {
            RSGymUtility.WriteTitle("User - List", "\n", "\n\n");

            foreach (User item in list)
            {
                RSGymUtility.WriteMessage($"Id: {item.Id}\nNome: {item.Name}\nData de nascimento: {item.Birth}", "", "\n");
            }
        }



        #endregion

        #region Destructor

        #endregion
    }
}
