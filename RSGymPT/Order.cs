// Purpose: Contains the Order class and its methods.
// The Order class is responsible for creating, changing, deleting, and listing orders.
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
        internal static List<Order> ordersList = new List<Order>();

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

                DateTime? trainingDateTime = OrderUtility.GetTrainingDateTime(ptCode, user);

                if (trainingDateTime == null)
                {
                    return ordersList;
                }

                if (OrderUtility.CheckUserAvailability((DateTime)trainingDateTime, user) && OrderUtility.CheckPtAvailability((DateTime)trainingDateTime, ptCode))
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

                if (order != null && OrderUtility.CheckSessionConclusion(order.TrainingDateTime))
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

            bool ptIsAvailable = OrderUtility.CheckPtAvailability(order.TrainingDateTime, ptCode);

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
            DateTime? trainingDateTime = OrderUtility.GetTrainingDateTime(order.PtCode, user);

            if (trainingDateTime == null)
            {
                return;
            }

            bool ptIsAvailable = false;
            bool userIsAvailable = false;

            userIsAvailable = OrderUtility.CheckUserAvailability((DateTime)trainingDateTime, user);

            if (!userIsAvailable)
            {
                return;
            }

            ptIsAvailable = OrderUtility.CheckPtAvailability((DateTime)trainingDateTime, order.PtCode);

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
            if (order != null && !OrderUtility.CheckSessionConclusion(order.TrainingDateTime))
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

            if (order != null && !OrderUtility.CheckSessionConclusion(order.TrainingDateTime))
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
