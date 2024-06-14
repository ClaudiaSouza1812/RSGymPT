using System;
using System.Collections.Generic;
using System.Linq;
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

        private static Dictionary<string, string> changeSubmenuOptions = new Dictionary<string, string>()
        {
            { "1", "Personal Trainer (PT)" },
            { "2", "Data e Horário" },
            { "3", "Estado do pedido" }
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
        internal string Reason { get; set; }
        internal string RealTime { get; set; } = string.Empty;

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
        internal string FullOrder => $"(Pedido): {OrderId}, (PT): {PtCode}, (Data e hora da sessão): {TrainingDateTime}, (Estado): {OrderStatus} {RealTime}";
        

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

        // Function to create an order
        internal static List<Order> CreateOrder(List<PersonalTrainer> personalTrainersList, User user) 
        {
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um Personal Trainer (PT)", "", "\n\n");
                RSGymUtility.WriteMessage($"{user.Name}, Digite o número da \nopção desejada e aperte 'Enter'", "", "\n\n");

                string ptCode = GetPersonalTrainerCode(personalTrainersList, user.Name);

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

        internal static string GetPersonalTrainerCode(List<PersonalTrainer> personalTrainersList, string userName)
        {
            PersonalTrainer personalTrainer;
            bool keepGoing = false;

            do
            {
                personalTrainer = PersonalTrainer.FindPersonalTrainerByCode(personalTrainersList, userName);

                if (personalTrainer != null)
                {
                    return personalTrainer.PtCode;
                }

                keepGoing = UserUtility.KeepGoing();
                
            } while (keepGoing);

            return null;
        }


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


        internal static void ShowPersonalTrainerAvailableSessions(List<Order> odersList, string ptCode, DateTime dateTime)
        {
            List<DateTime> sessionsRange = CreateHoursRange(dateTime);

            List<DateTime> ptSessions = ordersList.Where(pt => pt.PtCode == ptCode && pt.TrainingDateTime.Date == dateTime.Date && pt.OrderStatus == "Agendado").Select(pt => pt.TrainingDateTime).ToList();

            
            RSGymUtility.WriteMessage($"({ptCode}) - Sessões disponíveis: {dateTime.ToShortDateString()}", "\n", "\n");

            List<string> availableSessions = new List<string>();

            foreach (DateTime session in sessionsRange)
            {
                if (IsSessionAvailable(session, dateTime, ptSessions))
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


        internal static void ShowUserAvailableSessions(List<Order> odersList, User user, DateTime dateTime)
        {
            List<DateTime> sessionsRange = CreateHoursRange(dateTime);

            List<DateTime> userSessions = ordersList.Where(u => u.UserId == user.UserId && u.TrainingDateTime.Date == dateTime.Date && u.OrderStatus == "Agendado").Select(u => u.TrainingDateTime).ToList();


            RSGymUtility.WriteMessage($"({user.Name}) Sessões disponíveis: {dateTime.ToShortDateString()}", "\n\n", "\n");

            List<string> availableSessions = new List<string>();

            foreach (DateTime session in sessionsRange)
            {
                if (IsSessionAvailable(session, dateTime, userSessions))
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

        private static bool IsSessionAvailable(DateTime session, DateTime dateTime, List<DateTime> sessions)
        {
            bool isAvailableToday = dateTime.Date == DateTime.Today && !sessions.Contains(session) && session.TimeOfDay > DateTime.Now.TimeOfDay;
            bool isAvailableFuture = dateTime.Date != DateTime.Today && !sessions.Contains(session);

            return isAvailableToday || isAvailableFuture;
        }

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



        internal static void ShowAvailableSessions(string ptCode, User user)
        {
            ShowPersonalTrainerAvailableSessions(ordersList, ptCode, DateTime.Now);
            ShowUserAvailableSessions(ordersList, user, DateTime.Now);
        }


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


        internal static void ShowChangeSubmenuOptions()
        {
            foreach (KeyValuePair<string, string> option in changeSubmenuOptions)
            {
                RSGymUtility.WriteMessage($"({option.Key}) {option.Value}", "", "\n");
            }
        }


        internal static List<Order> ChangeOrder(List<Order> ordersList, List<PersonalTrainer> personalTrainersList, User user)
        {
            bool isNumber;
            bool keepGoing = true;
            string answer;
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
                
                bool isValidAction = false;
                string action;
                do
                {
                    Console.Clear();
                    RSGymUtility.WriteTitle("Alterar pedidos", "\n", "\n\n");
                    RSGymUtility.WriteMessage($"{user.Name}, insira o número do que deseja alterar no pedido", "", "\n\n");

                    ShowChangeSubmenuOptions();

                    RSGymUtility.WriteMessage($"Número: ", "\n", "");
                    answer = Console.ReadLine();

                    isValidAction = changeSubmenuOptions.TryGetValue(answer, out action);

                    if (!isValidAction)
                    {
                        RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                        keepGoing = UserUtility.KeepGoing();
                    }
                    if (!keepGoing)
                    {
                        return ordersList;
                    };

                } while (!isValidAction);

                if (!keepGoing)
                {
                    return ordersList;
                };

                do
                {
                    Console.Clear();
                    RSGymUtility.WriteTitle("Alterar pedidos", "\n", "\n\n");

                    ShowBookedOrders(ordersList, user, orderStatusOptions.Values.ElementAt(0));

                    RSGymUtility.WriteMessage($"Insira o número do pedido que deseja alterar", "\n", "\n\n");
                    RSGymUtility.WriteMessage($"Número: ", "\n", "");
                    answer = Console.ReadLine();

                    isNumber = int.TryParse(answer, out int orderNumber);

                    order = ordersList.FirstOrDefault(o => o.OrderId == orderNumber);

                    if (!isNumber || order == null)
                    {
                        RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                        keepGoing = UserUtility.KeepGoing();
                    }
                } while (!isNumber && keepGoing);

                if (order != null && CheckSessionConclusion(order.TrainingDateTime))
                {
                    RSGymUtility.WriteMessage("Sessão em andamento ou concluída, pedido deve ser finalizado.", "", "\n");
                    RSGymUtility.PauseConsole();
                    return ordersList;
                }

                if (!keepGoing)
                {
                    return ordersList;
                }

                RunChangeSubmenu(action, order, personalTrainersList, user);

                
            } while (!isNumber || UserUtility.KeepGoing());
            
            return ordersList; 
        }


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

        internal static void RunPersonalTrainerChange(Order order, List<PersonalTrainer> personalTrainersList, User user)
        {
            string ptCode = GetPersonalTrainerCode(personalTrainersList, user.Name);

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

            if (userIsAvailable)
            {
                ptIsAvailable = CheckPtAvailability((DateTime)trainingDateTime, order.PtCode);
            }

            if (ptIsAvailable)
            {
                order.TrainingDateTime = trainingDateTime.Value;

                ordersList.Remove(ordersList.FirstOrDefault(o => o.OrderId == order.OrderId));
                ordersList.Add(order);

                RSGymUtility.WriteMessage($"{order.FullOrder}", "\n", "\n\n");

                RSGymUtility.WriteMessage("Data e hora alterados com sucesso.", "", "\n");
            }
        }


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


        internal static List<Order> ShowBookedOrders(List<Order> ordersList, User user, string status)
        {
            Console.Clear();

            List<Order> bookedOrders = ordersList.Where(o => o.UserId == user.UserId && o.OrderStatus == status).ToList();

            RSGymUtility.WriteTitle("Pedidos registados", "\n", "\n");
            RSGymUtility.WriteMessage($"{user.Name},", "", "\n\n");

            foreach (Order order in ordersList)
            {
                if (order.OrderStatus == status)
                {
                    RSGymUtility.WriteMessage($"{order.FullOrder}", "", "\n");
                }
            }

            return bookedOrders;
        }


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
