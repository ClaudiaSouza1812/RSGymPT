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
        internal string FullOrder => $"(Pedido): {OrderId}, (PT): {PtCode}, (Data e hora da sessão): {TrainingDateTime}, (Estado): {OrderStatus}.";

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
            OrderStatus = string.Empty;
        }
        // Fazer segundo construtor com inserção parâmetros obrigatórios

        internal Order(int userId, string ptCode, DateTime trainingDateTime, string orderStatus)
        {
            OrderId = NextId++;
            UserId = userId;
            PtCode = ptCode;
            TrainingDateTime = trainingDateTime;
            OrderStatus = orderStatus;
        }
        #endregion


        #region Methods (public or internal)

        // 
        internal static void AddOrder(List<PersonalTrainer> personalTrainersList, User user)
        {
            Order order = new Order();

            CreateOrder(ordersList, personalTrainersList, user, order);

            ordersList.Add(order);

        }

        internal static void CreateOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user, Order order) 
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Registar Pedido de um PT", "", "\n\n");

            #region UserID

            order.UserId = user.UserId;

            #endregion

            #region PtCode and TrainingDateTime

            bool isAvailable;
            
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um PT", "", "\n\n");

                order.PtCode = GetPersonalTrainerCode(personalTrainersList);

                order.TrainingDateTime = GetTrainingDateTime(order.PtCode);

                isAvailable = CheckPtAvailability(order.TrainingDateTime, order.PtCode);

                if (!isAvailable)
                {
                    RSGymUtility.WriteMessage("PT indisponível, escolha outro PT ou outra Data e hora.", "\n", "\n");
                    ShowPersonalTrainerAvailableSessions(ordersList, order.PtCode, order.TrainingDateTime);
                    RSGymUtility.PauseConsole();
                
                }
                
            } while (!isAvailable);

            order.OrderStatus = "Agendado";

            RSGymUtility.WriteMessage($"{order.FullOrder}", "\n", "\n\n");

            #endregion

        }

        internal static string GetPersonalTrainerCode(List<PersonalTrainer> personalTrainersList)
        {
            PersonalTrainer personalTrainer;
            do
            {
                personalTrainer = PersonalTrainer.FindPersonalTrainerByCode(personalTrainersList);

            } while (personalTrainer == null);

            return personalTrainer.PtCode;
        }


        internal static DateTime[] CreateHoursRange(DateTime dateTime)
        {
            DateTime minHour = dateTime.Date.AddHours(9);
            DateTime maxHour = dateTime.Date.AddHours(21);
            int range = maxHour.Hour - minHour.Hour;
            DateTime[] sessionsRange = new DateTime[range + 1];
            

            for (int i = 0; i <= range; i++)
            {
                sessionsRange[i] = minHour;
                minHour = minHour.AddHours(1);
            }
            return sessionsRange;
        }

        internal static void ShowPersonalTrainerAvailableSessions(List<Order> odersList, string ptCode, DateTime dateTime)
        {
            bool available = false;
            DateTime[] sessionsRange = CreateHoursRange(dateTime);

            DateTime[] ptSessions = ordersList.Where(pt => pt.PtCode == ptCode && pt.TrainingDateTime.Date == dateTime.Date).Select(pt => pt.TrainingDateTime).ToArray();

            if (ptSessions.Length != 0)
            {
                RSGymUtility.WriteMessage($"({ptCode}) Sessões disponíveis: {dateTime.ToShortDateString()}", "", "\n");

                foreach (DateTime date in sessionsRange)
                {
                    // tratar sessões do mesmo dia
                    if (dateTime.Date == DateTime.Today && !ptSessions.Contains(date) && date.TimeOfDay > DateTime.Now.TimeOfDay)
                    {
                        RSGymUtility.WriteMessage($"({date.ToShortTimeString()}) ");
                        available = true;
                    }
                    // tratar sessões dos dias seguintes
                    else if (!ptSessions.Contains(date))
                    {
                        RSGymUtility.WriteMessage($"({date.ToShortTimeString()}) ");
                        available = true;
                    }
                }

                if (!available)
                {
                    RSGymUtility.WriteMessage("Nenhuma sessão disponível para hoje.", "", "\n");
                }
            }
        }

        internal static DateTime GetTrainingDateTime(string ptCode)
        {
            bool isDateTime;
            DateTime dateTime;
            DateTime minTime = DateTime.Today.AddHours(9);
            DateTime maxTime = DateTime.Today.AddHours(21);
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um PT", "", "\n\n");
                RSGymUtility.WriteTitle("Data e Hora da Sessão", "", "\n\n");

                ShowPersonalTrainerAvailableSessions(ordersList, ptCode, DateTime.Now);

                RSGymUtility.WriteMessage($"Insira a data e hora da sessão, ex ({DateTime.Now.ToShortDateString()} {DateTime.Now.Hour + 1}:00): ", "\n\n", "\n");

                
                string answer = Console.ReadLine();
                isDateTime = DateTime.TryParse(answer, out dateTime);
                DateTime nextHour = dateTime.AddHours(1);

                if (!isDateTime)
                {
                    RSGymUtility.WriteMessage("Data inválida. Inserir data e hora conforme informado acima.", "", "\n");
                    RSGymUtility.PauseConsole();
                }
                else if (dateTime < DateTime.Now)
                {
                    RSGymUtility.WriteMessage($"Agendamento de sessões apenas á partir de {nextHour.Hour}:00.", "", "\n");
                    isDateTime = false;
                    RSGymUtility.PauseConsole();
                }
                else if (dateTime.TimeOfDay < minTime.TimeOfDay || dateTime.TimeOfDay > maxTime.TimeOfDay || dateTime.TimeOfDay < nextHour.TimeOfDay)
                {
                    RSGymUtility.WriteMessage($"Agendamento de sessões das {minTime.TimeOfDay} ás {maxTime.TimeOfDay}.", "", "\n");
                    isDateTime = false;
                    RSGymUtility.PauseConsole();
                }

            } while (!isDateTime);
            
            return dateTime;
        }


        internal static bool CheckPtAvailability(DateTime dateTime, string ptCode)
        {
            foreach (Order order in ordersList)
            {
                DateTime sessionDuration = order.TrainingDateTime.AddMinutes(50);

                if (ptCode == order.PtCode && dateTime == order.TrainingDateTime && dateTime.TimeOfDay < sessionDuration.TimeOfDay)
                {
                    return false;
                }
            }
            return true;
        }

        internal static void ListOrdersByUser(User user)
        {
            bool haveOrder = false;

            foreach (Order order in ordersList)
            {
                if (user.UserId == order.UserId)
                {
                    RSGymUtility.WriteMessage($"{order.FullOrder}", "", "\n\n");
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
