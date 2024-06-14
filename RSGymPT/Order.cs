// Purpose: Class to manage the orders of the Personal Trainers (PT) and Users.
// The class contains properties, constructors, and methods to create, validate, and list orders.
// The class also contains methods to change, cancel, and finish orders.
// Restriction: The class is internal
// Version 1.0

// Libraries to be used in the class
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace RSGymPT
{
    internal class Order
    {
        #region Fields (properties, private variables)
        // List to store the orders
        private static List<Order> ordersList = new List<Order>();

        // Dictionary to store the order status options
        private static Dictionary<string, string> orderStatusOptions = new Dictionary<string, string>()
        {
            { "1", "Agendado" },
            { "2", "Cancelado" },
            { "3", "Terminado" }
        };

        // Dictionary to store the change submenu options
        private static Dictionary<string, string> changeSubmenuOptions = new Dictionary<string, string>()
        {
            { "1", "Personal Trainer (PT)" },
            { "2", "Data e Horário" },
            { "3", "Estado do pedido" }
        };

        #endregion


        #region Properties (public or internal)
        #region Auto-implemented properties 2.0
        
        internal int OrderId { get; }
        private static int NextId { get; set; } = 1;
        internal int UserId { get; set; }
        internal string PtCode { get; set; }
        internal DateTime TrainingDateTime { get; set; }
        internal string OrderStatus { get; set; }
        internal string Reason { get; set; }
        internal string RealTime { get; set; } = string.Empty;

        #endregion

        #region Classic properties 1.0
      
        #endregion

        #region Bodied-expression properties 3.0
        // Property to show the full order
        internal string FullOrder => $"(Pedido): {OrderId}, (PT): {PtCode}, (Data e hora da sessão): {TrainingDateTime}, (Estado): {OrderStatus} ";

        #endregion

        #endregion

        #region Constructors (public or internal)

        // Default constructor substitute
        internal Order()
        {
            OrderId = NextId++;
            UserId = 0;
            PtCode = string.Empty;
            TrainingDateTime = DateTime.Today;
            OrderStatus = string.Empty;
        }

        // Second constructor with mandatory parameters
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

        // Function to create an order
        internal static List<Order> CreateOrder(List<PersonalTrainer> personalTrainersList, User user) 
        {
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um Personal Trainer (PT)", "", "\n\n");
                RSGymUtility.WriteMessage($"{user.Name}, Digite o número da \nopção desejada e aperte 'Enter'", "", "\n\n");

                PersonalTrainer personalTrainer = PersonalTrainer.FindPersonalTrainerByCode(personalTrainersList, user.Name);

                if (personalTrainer == null)
                {
                    continue;
                }

                string ptCode = personalTrainer.PtCode;

                if (string.IsNullOrEmpty(ptCode))
                {
                    return ordersList;
                }

                DateTime? trainingDateTime = GetTrainingDateTime(ptCode, user);

                if (trainingDateTime == null)
                {
                    return ordersList;
                }

                if (CheckUserAvailability((DateTime)trainingDateTime, user) && CheckPtAvailability((DateTime)trainingDateTime, ptCode))
                {
                    Order order = new Order();
                    order.UserId = user.UserId;
                    order.PtCode = ptCode;
                    order.TrainingDateTime = trainingDateTime.Value;
                    order.OrderStatus = orderStatusOptions.Values.ElementAt(0);

                    ordersList.Add(order);

                    RSGymUtility.WriteMessage($"{order.FullOrder}", "\n", "\n\n");

                    RSGymUtility.WriteMessage("Pedido agendado com sucesso.", "", "\n");
                }

            } while (UserUtility.KeepGoing());

            return ordersList;
        }

        // Function to show the sessions range of hours
        internal static List<DateTime> CreateHoursRange(DateTime dateTime)
        {
            DateTime minHour = dateTime.Date.AddHours(9);
            DateTime maxHour = dateTime.Date.AddHours(21);
            int range = maxHour.Hour - minHour.Hour;
            List<DateTime> sessionsRange = new List<DateTime>();


            for (int i = 0; i <= range; i++)
            {
                sessionsRange.Add(minHour.AddHours(i));
            }
            return sessionsRange;
        }

        // Function to show the available sessions for the Personal Trainer (PT)
        internal static void ShowPersonalTrainerAvailableSessions(List<Order> odersList, string ptCode, DateTime dateTime)
        {
            List<DateTime> sessionsRange = CreateHoursRange(dateTime);

            List<DateTime> ptSessions = GetPtSessionsForDate(ptCode, dateTime);


            RSGymUtility.WriteMessage($"({ptCode}) - Sessões disponíveis: {dateTime.ToShortDateString()}", "\n", "\n");

            List<string> availableSessions = new List<string>();

            foreach (DateTime session in sessionsRange)
            {
                if (CheckIfSessionIsAvailable(session, dateTime, ptSessions))
                {
                    availableSessions.Add($"({session.ToShortTimeString()})");
                }
            }

            if (availableSessions.Any())
            {
                RSGymUtility.WriteMessage(string.Join(" ", availableSessions), "", "\n");
            }
            else
            {
                RSGymUtility.WriteMessage("Nenhum horário disponível para hoje.", "", "\n");
            }
        }

        // Function to show the available sessions for the User
        internal static void ShowUserAvailableSessions(List<Order> odersList, User user, DateTime dateTime)
        {
            List<DateTime> sessionsRange = CreateHoursRange(dateTime);

            List<DateTime> userSessions = GetUserSessionsForDate(user, dateTime); ;


            RSGymUtility.WriteMessage($"({user.Name}) Sessões disponíveis: {dateTime.ToShortDateString()}", "\n\n", "\n");

            List<string> availableSessions = new List<string>();

            foreach (DateTime session in sessionsRange)
            {
                if (CheckIfSessionIsAvailable(session, dateTime, userSessions))
                {
                    availableSessions.Add($"({session.ToShortTimeString()})");
                }
            }

            if (availableSessions.Any())
            {
                RSGymUtility.WriteMessage(string.Join(" ", availableSessions), "", "\n");
            }
            else
            {
                RSGymUtility.WriteMessage("Nenhum horário disponível para hoje.", "", "\n");
            }
        }

        // Function to get the Personal Trainer (PT) sessions for a specific date
        private static List<DateTime> GetPtSessionsForDate(string ptCode, DateTime dateTime)
        {
            List<DateTime> ptSessions =  ordersList.Where(pt => pt.PtCode == ptCode && pt.TrainingDateTime.Date == dateTime.Date && pt.OrderStatus == "Agendado").Select(pt => pt.TrainingDateTime).ToList();

            return ptSessions;
        }

        // Function to get the User sessions for a specific date
        private static List<DateTime> GetUserSessionsForDate(User user, DateTime dateTime)
        {
            List<DateTime> userSessions = ordersList.Where(u => u.UserId == user.UserId && u.TrainingDateTime.Date == dateTime.Date && u.OrderStatus == "Agendado").Select(u => u.TrainingDateTime).ToList();

            return userSessions;
        }

        // Function to check if the session is available
        private static bool CheckIfSessionIsAvailable(DateTime session, DateTime dateTime, List<DateTime> sessions)
        {
            bool isAvailableToday = dateTime.Date == DateTime.Today && !sessions.Contains(session) && session.TimeOfDay > DateTime.Now.TimeOfDay;
            bool isAvailableFuture = dateTime.Date != DateTime.Today && !sessions.Contains(session);

            return isAvailableToday || isAvailableFuture;
        }

        // Function to get the date and time of the training session
        internal static DateTime? GetTrainingDateTime(string ptCode, User user)
        {
            DateTime dateTime;
            DateTime minTime = DateTime.Today.AddHours(9);
            DateTime maxTime = DateTime.Today.AddHours(21);
            bool isDateTime = false;
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um Personal trainer (PT)", "", "\n\n");
                RSGymUtility.WriteTitle("Data e Hora da Sessão", "", "\n\n");

                ShowAvailableSessions(ptCode, user);

                RSGymUtility.WriteMessage($"{user.Name}, Insira a data e hora da sessão, ex ({DateTime.Now.ToShortDateString()} {DateTime.Now.Hour + 1}:00): ", "\n\n", "\n");
                string answer = Console.ReadLine();

                isDateTime = DateTime.TryParse(answer, out dateTime);
                DateTime nextHour = DateTime.Now.AddHours(1);

                if (!isDateTime)
                {
                    RSGymUtility.WriteMessage("Data inválida. Inserir data e hora conforme informado acima.", "", "\n");
                    
                }
                else if (dateTime < DateTime.Now)
                {
                    RSGymUtility.WriteMessage("Não é possível agendar sessões no passado. Insira uma data e hora futura.", "", "\n");
                    isDateTime = false;
                }
                else if (dateTime.TimeOfDay < minTime.TimeOfDay || dateTime.TimeOfDay > maxTime.TimeOfDay)
                {
                    RSGymUtility.WriteMessage($"As sessões devem ser agendadas entre {minTime.ToShortTimeString()} e {maxTime.ToShortTimeString()}.", "", "\n");
                    isDateTime = false;
                }
                else if (dateTime.Minute != 0)
                {
                    RSGymUtility.WriteMessage("Por favor, insira uma hora cheia (ex: 10:00).", "\n", "\n");
                    isDateTime = false;
                }

                if (!isDateTime && !UserUtility.KeepGoing())
                {
                    return null;
                }

            } while (!isDateTime);
            
            return dateTime;
        }

        // Function to show the available sessions
        internal static void ShowAvailableSessions(string ptCode, User user)
        {
            ShowPersonalTrainerAvailableSessions(ordersList, ptCode, DateTime.Now);
            ShowUserAvailableSessions(ordersList, user, DateTime.Now);
        }

        // Function to check the Personal Trainer (PT) availability
        internal static bool CheckPtAvailability(DateTime dateTime, string ptCode)
        {
            foreach (Order order in ordersList)
            {
                DateTime sessionEnd = order.TrainingDateTime.AddMinutes(60);

                if (order.PtCode == ptCode && dateTime >= order.TrainingDateTime && dateTime < sessionEnd && order.OrderStatus != "Cancelado")
                {
                    RSGymUtility.WriteMessage("PT indisponível, escolha outro PT ou outra Data e hora.", "\n\n", "\n");
                    return false;
                }
            }
            return true;
        }

        // Function to check the User availability
        internal static bool CheckUserAvailability(DateTime dateTime, User user)
        {
            foreach (Order order in ordersList)
            {
                DateTime sessionEnd = order.TrainingDateTime.AddMinutes(50);

                if (user.UserId == order.UserId && dateTime >= order.TrainingDateTime && dateTime < sessionEnd && order.OrderStatus != "Cancelado")
                {
                    ListOrdersByUser(user);
                    RSGymUtility.WriteMessage("Sessões com duração de 50 minutos. Você já tem uma sessão agendada nesta data e horário, escolha outra Data e hora.", "\n", "\n");
                    return false;
                }
            }
            return true;
        }

        // Function to show the change submenu options
        internal static void ShowChangeSubmenuOptions()
        {
            foreach (KeyValuePair<string, string> option in changeSubmenuOptions)
            {
                RSGymUtility.WriteMessage($"({option.Key}) {option.Value}", "", "\n");
            }
        }

        // Function to change the order
        internal static List<Order> ChangeOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user)
        {
            Order order;
            do
            {
                List<Order> bookedOrders = ShowBookedOrders(ordersList, user, orderStatusOptions.Values.ElementAt(0));

                if (bookedOrders.Count == 0)
                {
                    RSGymUtility.WriteMessage("Você não tem sessões agendadas.", "", "\n");
                    RSGymUtility.PauseConsole();
                    return ordersList;
                }
                
                string action = GetChangeAction(user);

                if (string.IsNullOrEmpty(action))
                {
                    return ordersList;
                }

                order = GetOrderToChange(ordersList, action, user);

                if (order == null)
                {
                    continue;
                }

                if (order != null && CheckSessionConclusion(order.TrainingDateTime))
                {
                    RSGymUtility.WriteMessage("Sessão em andamento ou concluída, pedido deve ser finalizado.", "", "\n");
                    RSGymUtility.PauseConsole();
                    return ordersList;
                }

                RunChangeSubmenu(action, order, personalTrainersList, user);

            } while (UserUtility.KeepGoing());
            
            return ordersList; 
        }

        // Function to get the change action
        internal static string GetChangeAction(User user)
        {
            bool isValidAction = false;
            bool keepGoing = true;
            string action;
            do
            {
                Console.Clear();
                RSGymUtility.WriteTitle("Alterar pedidos", "\n", "\n\n");
                RSGymUtility.WriteMessage($"{user.Name}, insira o número do que deseja alterar no pedido", "", "\n\n");

                ShowChangeSubmenuOptions();

                RSGymUtility.WriteMessage($"Número: ", "\n", "");
                string answer = Console.ReadLine();

                isValidAction = changeSubmenuOptions.TryGetValue(answer, out action);

                if (!isValidAction)
                {
                    RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                    keepGoing = UserUtility.KeepGoing();
                }
                
            } while (!isValidAction && UserUtility.KeepGoing());

            if (!keepGoing)
            {
                return string.Empty;
            };

            return action;
        }

        // Function to get the order to change
        internal static Order GetOrderToChange(List<Order> ordersList, string action, User user)
        {
            bool isNumber;
            Order order;
            do
            {
                Console.Clear();
                RSGymUtility.WriteTitle("Alterar pedidos", "\n", "\n\n");

                ShowBookedOrders(ordersList, user, orderStatusOptions.Values.ElementAt(0));

                RSGymUtility.WriteMessage($"Insira o número do pedido que deseja alterar", "\n", "\n\n");
                RSGymUtility.WriteMessage($"Número: ", "\n", "");

                string answer = Console.ReadLine();

                isNumber = int.TryParse(answer, out int orderNumber);

                order = ordersList.FirstOrDefault(o => o.OrderId == orderNumber);

                if (!isNumber || order == null || string.IsNullOrEmpty(answer))
                {
                    RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                }
            } while (!isNumber && UserUtility.KeepGoing());

            return order;
        }
        
        // Function to check the session conclusion
        internal static bool CheckSessionConclusion(DateTime trainingDateTime)
        {
            if (trainingDateTime.Date > DateTime.Now.Date)
            {
                return false;
            }

            if (trainingDateTime.Date == DateTime.Now.Date && DateTime.Now >= trainingDateTime)
            {
                return true;
            }
            return false;
        }

        // Function to run the change submenu
        internal static void RunChangeSubmenu(string action, Order order, List<PersonalTrainer> personalTrainersList, User user)
        {
            switch (action)
            {
                case "Personal Trainer (PT)":
                    RunPersonalTrainerChange(order, personalTrainersList, user);
                    break;
                case "Data e Horário":
                    RunDateTimeChange(order, user);
                    break;
                case "Estado do pedido":
                    RunStateChange(order, user);
                    break;
            }
        }

        // Function to run the state change
        internal static void RunStateChange(Order order, User user)
        {
            RSGymUtility.WriteMessage("Pedido: ", "\n", "");
            RSGymUtility.WriteMessage($"{order.FullOrder}", "", "\n\n");
            RSGymUtility.WriteMessage("Digite o número do novo estado do pedido:\n(2) - Cancelado\n(3) - Terminado ", "", "\n");
            RSGymUtility.WriteMessage("Número: ", "\n", "");

            string newStatus = Console.ReadLine();

            bool isValid = orderStatusOptions.TryGetValue(newStatus, out string status);

            if (!isValid)
            {
                RSGymUtility.WriteMessage("Número inválido.", "", "\n");
            }
            else
            {
                if (newStatus == "2")
                {
                    CancelOrder(order, newStatus);
                    return;
                }
                else if (newStatus == "3")
                {
                    FinishOrder(order, newStatus);
                    return;
                }   
            }
        }

        // Function to run the Personal Trainer (PT) change
        internal static void RunPersonalTrainerChange(Order order, List<PersonalTrainer> personalTrainersList, User user)
        {
            PersonalTrainer personalTrainer = PersonalTrainer.FindPersonalTrainerByCode(personalTrainersList, user.Name);

            if (personalTrainer == null)
            {
                return;
            }

            string ptCode = personalTrainer.PtCode;

            if (string.IsNullOrEmpty(ptCode))
            {
                return;
            }

            bool ptIsAvailable = CheckPtAvailability(order.TrainingDateTime, ptCode);

            if (ptIsAvailable)
            {
                order.PtCode = ptCode;

                ordersList.Remove(ordersList.FirstOrDefault(o => o.OrderId == order.OrderId));
                ordersList.Add(order);

                RSGymUtility.WriteMessage($"{order.FullOrder}", "\n", "\n\n");

                RSGymUtility.WriteMessage("Personal Trainer (PT) alterado com sucesso.", "", "\n");
            }
        }

        // Function to run the date and time change
        internal static void RunDateTimeChange(Order order, User user)
        {
            DateTime? trainingDateTime = GetTrainingDateTime(order.PtCode, user);

            if (trainingDateTime == null)
            {
                return;
            }

            bool ptIsAvailable = false;
            bool userIsAvailable = false;

            userIsAvailable = CheckUserAvailability((DateTime)trainingDateTime, user);

            if (!userIsAvailable)
            {
                return;
            }

            ptIsAvailable = CheckPtAvailability((DateTime)trainingDateTime, order.PtCode);

            if (!ptIsAvailable)
            {
                return;
            }

            order.TrainingDateTime = trainingDateTime.Value;

            ordersList.Remove(ordersList.FirstOrDefault(o => o.OrderId == order.OrderId));
            ordersList.Add(order);

            RSGymUtility.WriteMessage($"{order.FullOrder}", "\n", "\n\n");

            RSGymUtility.WriteMessage("Data e hora alterados com sucesso.", "", "\n");
        }

        // Function to delete an order
        internal static List<Order> DeleteOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user)
        {
            bool isNumber;
            bool keepGoing = true;
            Order order;
            do
            {
                do
                {
                    List<Order> bookedOrders = ShowBookedOrders(ordersList, user, orderStatusOptions.Values.ElementAt(0));

                    if (bookedOrders.Count == 0)
                    {
                        RSGymUtility.WriteMessage("Você não tem sessões agendadas.", "", "\n");
                        RSGymUtility.PauseConsole();
                        return ordersList;
                    }

                    RSGymUtility.WriteTitle("Eliminar pedidos", "\n", "\n\n");
                    RSGymUtility.WriteMessage($"{user.Name}, Digite o código do pedido\ne aperte 'Enter'", "", "\n\n");
                    RSGymUtility.WriteMessage("Insira o número do pedido que deseja eliminar: ", "", "\n");
                    RSGymUtility.WriteMessage("Número: ", "\n", "");

                    string answer = Console.ReadLine();

                    isNumber = int.TryParse(answer, out int orderNumber);

                    order = ordersList.FirstOrDefault(o => o.OrderId == orderNumber);

                    if (!isNumber || order == null)
                    {
                        RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                        keepGoing = UserUtility.KeepGoing();
                    }

                } while (!isNumber && keepGoing);

                if (!keepGoing)
                {
                    return ordersList;
                }

                if (UserUtility.CheckDelete())
                {
                    ordersList.Remove(ordersList.FirstOrDefault(o => o.OrderId == order.OrderId));
                    RSGymUtility.WriteMessage("Pedido eliminado com sucesso.", "\n", "\n\n");
                    keepGoing = false;
                    RSGymUtility.PauseConsole();
                }
                else
                {
                    keepGoing = false;
                }

            } while (keepGoing);
            
            return ordersList;
        }

        // Function to cancel an order
        internal static void CancelOrder(Order order, string newStatus)
        {
            string reason;
            do
            {
                RSGymUtility.WriteMessage("Informe o motivo do cancelamento: ", "", "");
                reason = Console.ReadLine();
            } while (reason == string.Empty);

            order.Reason = reason;
            order.OrderStatus = orderStatusOptions[newStatus];
            order.RealTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            RSGymUtility.WriteMessage("Estado do pedido alterado com sucesso.", "\n", "\n");
            RSGymUtility.WriteMessage($"{order.FullOrder} ({order.Reason}) {order.RealTime}", "", "\n\n");
        }

        internal static void FinishOrder(Order order, string newStatus)
        {
            if (order != null && !CheckSessionConclusion(order.TrainingDateTime))
            {
                RSGymUtility.WriteMessage("Sessão não pode ser terminada por que ainda não foi realizada.", "", "\n");
                return;
            }

            order.OrderStatus = orderStatusOptions.Values.ElementAt(2);
            order.RealTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            ordersList.Remove(ordersList.FirstOrDefault(o => o.OrderId == order.OrderId));
            ordersList.Add(order);

            RSGymUtility.WriteMessage($"{order.FullOrder} ({order.Reason}) {order.RealTime}", "", "\n\n");
            RSGymUtility.WriteMessage("Pedido finalizado com sucesso.", "", "\n");
        }

        // Function to finish an order
        internal static List<Order> FinishOrder(User user)
        {
            bool isNumber;
            bool keepGoing = true;
            Order order;

            do
            {
                Console.Clear();

                List<Order> bookedOrders = ShowBookedOrders(ordersList, user, orderStatusOptions.Values.ElementAt(0));

                if (bookedOrders.Count == 0)
                {
                    RSGymUtility.WriteMessage("Você não tem sessões agendadas.", "", "\n");
                    RSGymUtility.PauseConsole();
                    return ordersList;
                }

                RSGymUtility.WriteTitle("Terminar Pedidos", "\n", "\n\n");
                RSGymUtility.WriteMessage($"{user.Name}, Digite o código do pedido\ne aperte 'Enter'", "", "\n\n");
                RSGymUtility.WriteMessage("Insira o número do pedido que deseja terminar: ", "", "\n");

                string answer = Console.ReadLine();

                isNumber = int.TryParse(answer, out int orderNumber);

                order = ordersList.FirstOrDefault(o => o.OrderId == orderNumber);

                if (!isNumber || order == null)
                {
                    RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                }

            } while (!isNumber && keepGoing);

            if (order != null && !CheckSessionConclusion(order.TrainingDateTime))
            {
                RSGymUtility.WriteMessage("Sessão não pode ser terminada por que ainda não foi realizada.", "\n", "\n");
                UserUtility.KeepGoing();
                return ordersList;
            }

            if (!keepGoing)
            { 
                return ordersList;
            }
                
            order.OrderStatus = orderStatusOptions.Values.ElementAt(2);
            order.RealTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            ordersList.Remove(ordersList.FirstOrDefault(o => o.OrderId == order.OrderId));
            ordersList.Add(order);

            RSGymUtility.WriteMessage($"{order.FullOrder} ({order.Reason}) {order.RealTime}", "", "\n\n");
            RSGymUtility.WriteMessage("Sessão terminada com sucesso.", "", "\n");
            RSGymUtility.PauseConsole();
            
            return ordersList;
        }

        // Function to show the booked orders
        internal static List<Order> ShowBookedOrders(List<Order> ordersList, User user, string status)
        {
            Console.Clear();

            List<Order> bookedOrders = ordersList.Where(o => o.UserId == user.UserId && o.OrderStatus == status).ToList();

            RSGymUtility.WriteTitle("Pedidos registados", "\n", "\n");
            RSGymUtility.WriteMessage($"{user.Name},", "", "\n\n");

            foreach (Order order in bookedOrders)
            {
                RSGymUtility.WriteMessage($"{order.FullOrder}", "", "\n");
            }

            return bookedOrders;
        }

        // Function to list the orders by user
        internal static void ListOrdersByUser(User user)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Lista de Pedidos", "\n", "\n\n");
            RSGymUtility.WriteMessage($"{user.Name}, ", "", "\n\n");

            bool haveOrder = false;

            foreach (Order order in ordersList)
            {
                if (user.UserId == order.UserId)
                {
                    RSGymUtility.WriteMessage($"{order.FullOrder} ({order.Reason}) {order.RealTime}", "", "\n");
                    haveOrder = true;
                }
            }
            if (!haveOrder)
            {
                RSGymUtility.WriteMessage($"Você ainda não tem sessões agendadas.", "", "\n\n");
            }
        }

        #endregion
    }
}
