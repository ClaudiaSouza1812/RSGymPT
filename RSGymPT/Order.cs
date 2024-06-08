using System;
using System.Collections.Generic;
using System.Deployment.Internal;
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
        private static List<Order> ordersList = new List<Order>();

        #endregion


        #region Properties (public or internal)
        #region Auto-implemented properties 2.0
        /* 
        Exemplo de uma propriedade usando Auto-implemented properties
        internal string Operator { get; set; } // propriedade no singular
        */

        internal int OrderId { get; }
        private static int NextId { get; set; } = 1;
        internal int UserId { get; set; }
        internal string PtCode { get; set; }
        internal DateTime TrainingDateTime { get; set; }
        internal string ordersListtatus { get; set; }

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
            TrainingDateTime = DateTime.Today;
            ordersListtatus = string.Empty;
        }
        // Fazer segundo construtor com inserção parâmetros obrigatórios

        internal Order(int userId, string ptCode, DateTime trainingDateTime, string ordersListtatus)
        {
            OrderId = NextId++;
            UserId = userId;
            PtCode = ptCode;
            TrainingDateTime = trainingDateTime;
            ordersListtatus = ordersListtatus;
        }
        #endregion


        #region Methods (public or internal)

        // 
        internal static void AddOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user)
        {
            Order order = new Order();

            CreateOrder(ordersList, personalTrainersList, user, order);


            ordersList.Add(order);

        }

        internal static void CreateOrder(Order order, List<PersonalTrainer> personalTrainersList, User user) 
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Registar Pedido de um PT", "", "\n\n");

            #region UserID

            order.UserId = user.UserId;

            #endregion

            #region PtCode

            PersonalTrainer personalTrainer;

            do
            {
                personalTrainer = PersonalTrainer.FindPersonalTrainerByCode(personalTrainersList);

                if (personalTrainer != null)
                {
                    order.PtCode = personalTrainer.PtCode;
                }

            } while (personalTrainer == null);
            

            #endregion

            #region TrainingDateTime

            bool isDateTime, isAvailable;
            DateTime dateTime;
            DateTime minTime = DateTime.Today.AddHours(9);
            DateTime maxTime = DateTime.Today.AddHours(21);
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um PT", "", "\n\n");

                RSGymUtility.WriteMessage($"Insira a data e hora da sessão, ex ({DateTime.Now.ToShortDateString()} {DateTime.Now.AddHours(1).ToShortTimeString()}): ", "", "\n");

                string answer = Console.ReadLine();

                isDateTime = DateTime.TryParse(answer, out dateTime);
                isAvailable = SetordersListtatus(dateTime, order.PtCode);

                if (!isDateTime)
                {
                    RSGymUtility.WriteMessage("Inserção inválida. Insira data e hora conforme informado acima.", "", "\n");
                }
                else if (!isAvailable)
                {
                    RSGymUtility.WriteMessage("PT indisponível, escolha outro PT ou outra Data e hora");
                }
                else if (dateTime < DateTime.Now.AddHours(1))
                {
                    RSGymUtility.WriteMessage($"Agendamento de sessões apenas á partir de {DateTime.Now.AddHours(1)}.", "", "\n");
                    isDateTime = false;
                }
                else if (dateTime.TimeOfDay < minTime.TimeOfDay || dateTime.TimeOfDay > maxTime.TimeOfDay)
                {
                    RSGymUtility.WriteMessage($"Agendamento de sessões apenas das {minTime.TimeOfDay} ás {maxTime.TimeOfDay}.", "", "\n");
                    isDateTime = false;
                }

            } while (!isDateTime && !isAvailable);

            order.TrainingDateTime = dateTime;
            order.ordersListtatus = "Agendado";

            #endregion

        }

        internal static bool SetordersListtatus(DateTime dateTime, string ptCode)
        {
            foreach (Order order in ordersList)
            {
                if (ptCode == order.PtCode && dateTime.Date == order.TrainingDateTime.Date && dateTime.TimeOfDay == order.TrainingDateTime.TimeOfDay)
                {
                    return false;
                }
            }
            return true;
        }

        internal static void ListordersListByUser(User user)
        {
            bool haveOrder = false;

            foreach (Order order in ordersList)
            {
                if (user.UserId == order.UserId)
                {
                    RSGymUtility.WriteMessage($"Pedido: {order.OrderId}, PT: {order.PtCode}, Data e hora da sessão: {order.TrainingDateTime}, Estado: {order.ordersListtatus}.", "", "\n\n");
                    haveOrder = true;
                }
            }

            if (!haveOrder)
            {
                RSGymUtility.WriteMessage($"{user.Name}, você ainda não tem sessões agendadas.", "", "\n\n");
            }
            #endregion


        }
    }
}
