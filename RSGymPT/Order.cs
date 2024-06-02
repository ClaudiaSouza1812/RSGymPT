using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;

namespace RSGymPT
{
    internal class Order
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

        internal int OrderId { get; }
        internal int NextId { get; set; } = 1;
        internal int UserId { get; }
        internal string PtCode { get; }
        internal DateTime TrainingDateTime { get; set; }
        internal string OrderStatus { get; set; }

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

        internal Order()
        {
            OrderId = NextId++;
            UserId = 0;
            PtCode = string.Empty;
            TrainingDateTime = DateTime.Now;
            OrderStatus = string.Empty;
        }
        // Fazer segundo construtor com inserção parâmetros obrigatórios

        internal Order(int userId, string ptCode, DateTime trainingDateTime,  string orderStatus)
        {
            OrderId = NextId++;
            UserId = userId;
            PtCode = ptCode;
            TrainingDateTime = trainingDateTime;
            OrderStatus = orderStatus;
        }
        #endregion


        #region Methods (public or internal)

        // Parei aqui, preciso receber o usuário
        internal static void AddOrder(List<PersonalTrainer> personalTrainers)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Registar Pedido de um Personal Trainer", "\n", "\n\n");

            Order order = new Order();

            PersonalTrainer personalTrainer = PersonalTrainer.FindCode(personalTrainers);


        }

        internal void AksOrder() 
        {
            RSGymUtility.WriteTitle("Registar pedido", "", "\n\n");

            RSGymUtility.WriteMessage("Digite o código do Personal Trainer ", "", "\n");
        }


        #endregion  
    }
}
