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

        private static Dictionary<string, string> orderStatusOptions = new Dictionary<string, string>()
        {
            { "1", "Agendado" },
            { "2", "Cancelado" },
            { "3", "Terminado" }
        };

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
        internal static List<Order> AddOrder(List<PersonalTrainer> personalTrainersList, User user)
        {
            Order order = new Order();

            CreateOrder(ordersList, personalTrainersList, user, order);

            ordersList.Add(order);

            return ordersList;
        }

        internal static void CreateOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user, Order order) 
        {
            #region UserID

            order.UserId = user.UserId;

            #endregion

            #region PtCode and TrainingDateTime

            bool userIsAvailable, ptIsAvailable = false;
            
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um Personal Trainer (PT)", "", "\n\n");
                RSGymUtility.WriteMessage($"{user.Name}, Digite o número da \nopção desejada e aperte 'Enter'", "", "\n\n");

                order.PtCode = GetPersonalTrainerCode(personalTrainersList, user.Name);

                order.TrainingDateTime = GetTrainingDateTime(order.PtCode);

                userIsAvailable = CheckUserAvailability(order.TrainingDateTime, user);

                if (userIsAvailable)
                {
                    ptIsAvailable = CheckPtAvailability(order.TrainingDateTime, order.PtCode);
                }

            } while (!userIsAvailable || !ptIsAvailable);

            order.OrderStatus = orderStatusOptions.Values.ElementAt(0);

            RSGymUtility.WriteMessage($"{order.FullOrder}", "\n", "\n\n");

            RSGymUtility.WriteMessage("Pedido agendado com sucesso.", "", "\n");

            #endregion

        }

        internal static string GetPersonalTrainerCode(List<PersonalTrainer> personalTrainersList, string userName)
        {
            PersonalTrainer personalTrainer;
            do
            {
                personalTrainer = PersonalTrainer.FindPersonalTrainerByCode(personalTrainersList, userName);

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

            DateTime[] ptSessions = ordersList.Where(pt => pt.PtCode == ptCode && pt.TrainingDateTime.Date == dateTime.Date && pt.OrderStatus == "Agendado").Select(pt => pt.TrainingDateTime).ToArray();

            
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
                else if (dateTime.Date != DateTime.Today && !ptSessions.Contains(date))
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

        internal static DateTime GetTrainingDateTime(string ptCode)
        {
            bool isDateTime;
            DateTime dateTime;
            DateTime minTime = DateTime.Today.AddHours(9);
            DateTime maxTime = DateTime.Today.AddHours(21);
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um Personal trainer (PT)", "", "\n\n");
                RSGymUtility.WriteTitle("Data e Hora da Sessão", "", "\n\n");

                ShowPersonalTrainerAvailableSessions(ordersList, ptCode, DateTime.Now);

                RSGymUtility.WriteMessage($"Insira a data e hora da sessão, ex ({DateTime.Now.ToShortDateString()} {DateTime.Now.Hour + 1}:00): ", "\n\n", "\n");

                
                string answer = Console.ReadLine();
                isDateTime = DateTime.TryParse(answer, out dateTime);
                DateTime nextHour = DateTime.Now.AddHours(1);

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
                else if (dateTime.TimeOfDay < minTime.TimeOfDay || dateTime.TimeOfDay > maxTime.TimeOfDay)
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

                if (ptCode == order.PtCode && dateTime == order.TrainingDateTime && order.OrderStatus != "Cancelado" || ptCode == order.PtCode && dateTime == order.TrainingDateTime && order.OrderStatus != "Cancelado" && dateTime < sessionDuration)
                {
                    ShowPersonalTrainerAvailableSessions(ordersList, order.PtCode, order.TrainingDateTime);
                    RSGymUtility.WriteMessage("PT indisponível, escolha outro PT ou outra Data e hora.", "\n", "\n");
                    RSGymUtility.PauseConsole();
                    return false;
                }
            }
            return true;
        }

        internal static bool CheckUserAvailability(DateTime dateTime, User user)
        {
            foreach (Order order in ordersList)
            {
                DateTime sessionDuration = order.TrainingDateTime.AddMinutes(50);

                if (user.UserId == order.UserId && dateTime == order.TrainingDateTime && order.OrderStatus != "Cancelado" || user.UserId == order.UserId && dateTime == order.TrainingDateTime && order.OrderStatus != "Cancelado" && dateTime < sessionDuration)
                {
                    ListOrdersByUser(user);
                    RSGymUtility.WriteMessage("Você já tem uma sessão agendada nesta data e horário, escolha outra Data e hora.", "", "\n");
                    RSGymUtility.PauseConsole();
                    return false;
                }
            }
            return true;
        }

        
        internal static List<Order> ChangeOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user)
        {
            bool isNumber;
            do
            {
                ListBookedOrders(ordersList, user);

                RSGymUtility.WriteTitle("Alterar pedidos", "\n", "\n\n");

                RSGymUtility.WriteMessage("Insira o número do pedido que deseja alterar: ", "", "\n");

                string answer = Console.ReadLine();

                isNumber = int.TryParse(answer, out int orderNumber);

                Order order = ordersList.FirstOrDefault(o => o.OrderId == orderNumber);

                if (!isNumber || order == null)
                {
                    RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                    RSGymUtility.PauseConsole();
                }
                else
                {
                    order.PtCode = GetPersonalTrainerCode(personalTrainersList, user.Name);

                    order.TrainingDateTime = GetTrainingDateTime(order.PtCode);

                    CheckUserAvailability(order.TrainingDateTime, user);

                    RSGymUtility.WriteMessage($"{order.FullOrder}", "", "\n\n");

                    RSGymUtility.WriteMessage("Pedido alterado com sucesso.", "", "\n");
                }
            } while (!isNumber);
            


            return ordersList; 
        
        }


        internal static List<Order> CancelOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user)
        {
            bool isNumber;
            do
            {
                ListBookedOrders(ordersList, user);

                RSGymUtility.WriteTitle("Cancelar pedidos", "\n", "\n\n");

                RSGymUtility.WriteMessage("Insira o número do pedido que deseja cancelar: ", "", "\n");

                string answer = Console.ReadLine();

                isNumber = int.TryParse(answer, out int orderNumber);

                Order order = ordersList.FirstOrDefault(o => o.OrderId == orderNumber);

                if (!isNumber || order == null)
                {
                    RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                    RSGymUtility.PauseConsole();
                }
                else
                {
                    order.OrderStatus = orderStatusOptions.Values.ElementAt(1);
                    RSGymUtility.WriteMessage("Pedido cancelado com sucesso.", "", "\n");
                }
            } while (!isNumber);

            return ordersList;
        }


        internal static List<Order> FinishOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user)
        {
            bool isNumber;
            do
            {
                ListBookedOrders(ordersList, user);

                RSGymUtility.WriteTitle("Terminar Pedidos", "\n", "\n\n");

                RSGymUtility.WriteMessage("Insira o número do pedido que deseja finalizar: ", "", "\n");

                string answer = Console.ReadLine();

                isNumber = int.TryParse(answer, out int orderNumber);

                Order order = ordersList.FirstOrDefault(o => o.OrderId == orderNumber);

                if (!isNumber || order == null)
                {
                    RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                    RSGymUtility.PauseConsole();
                }
                else
                {
                    order.OrderStatus = orderStatusOptions.Values.ElementAt(2);
                    RSGymUtility.WriteMessage("Pedido finalizado com sucesso.", "", "\n");
                }
            } while (!isNumber);

            return ordersList;
        }


        internal static void ListBookedOrders(List<Order> ordersList, User user)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Pedidos registados", "\n", "\n\n");

            foreach (Order order in ordersList)
            {
                if (order.OrderStatus == "Agendado")
                {
                    RSGymUtility.WriteMessage($"{order.FullOrder}", "", "\n");
                }
            }
        }


        internal static void ListOrdersByUser(User user)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Pedidos - Lista", "\n", "\n\n");

            bool haveOrder = false;

            foreach (Order order in ordersList)
            {
                if (user.UserId == order.UserId)
                {
                    RSGymUtility.WriteMessage($"{order.FullOrder}", "", "\n");
                    haveOrder = true;
                }
            }

            if (!haveOrder)
            {
                RSGymUtility.WriteMessage($"{user.Name}, você ainda não tem sessões agendadas.", "", "\n\n");
                RSGymUtility.PauseConsole();
            }
            else
            {
                RSGymUtility.PauseConsole();
            }
        }

        #endregion
    }
}
