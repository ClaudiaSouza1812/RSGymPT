using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Utility;
using static System.Collections.Specialized.BitVector32;

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
        internal string FullOrder => $"(Pedido): {OrderId}, (PT): {PtCode}, (Data e hora da sessão): {TrainingDateTime}, (Estado): {OrderStatus}";
        internal string Reason { get; set; }

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
        internal static List<Order> CreateOrder(List<PersonalTrainer> personalTrainersList, User user) 
        {
            do
            {
                Console.Clear();

                RSGymUtility.WriteTitle("Registar Pedido de um Personal Trainer (PT)", "", "\n\n");
                RSGymUtility.WriteMessage($"{user.Name}, Digite o número da \nopção desejada e aperte 'Enter'", "", "\n\n");

                string ptCode = GetPersonalTrainerCode(personalTrainersList, user.Name);

                if (ptCode == null)
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
                if (personalTrainer == null)
                {
                    keepGoing = UserUtility.KeepGoing();
                }
                else
                {
                    keepGoing = false;
                }

            } while (keepGoing);

            if (personalTrainer == null)
            {
                return null;
            }

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

            
            RSGymUtility.WriteMessage($"({ptCode}) - Sessões disponíveis: {dateTime.ToShortDateString()}", "\n", "\n");

            foreach (DateTime session in sessionsRange)
            {
                bool isAvailableToday = dateTime.Date == DateTime.Today && !ptSessions.Contains(session) && session.TimeOfDay > DateTime.Now.TimeOfDay;
                bool isAvailableFuture = dateTime.Date != DateTime.Today && !ptSessions.Contains(session);

                if (isAvailableToday || isAvailableFuture)
                {
                    RSGymUtility.WriteMessage($"({session.ToShortTimeString()}) ");
                    available = true;
                }

            }

            if (!available)
            {
                RSGymUtility.WriteMessage("Nenhuma sessão disponível para hoje.", "", "\n");
            }
            
        }


        internal static void ShowUserAvailableSessions(List<Order> odersList, User user, DateTime dateTime)
        {
            bool available = false;
            DateTime[] sessionsRange = CreateHoursRange(dateTime);

            DateTime[] userSessions = ordersList.Where(u => u.UserId == user.UserId && u.TrainingDateTime.Date == dateTime.Date && u.OrderStatus == "Agendado").Select(u => u.TrainingDateTime).ToArray();


            RSGymUtility.WriteMessage($"({user.Name}) Sessões disponíveis: {dateTime.ToShortDateString()}", "\n\n", "\n");

            foreach (DateTime session in sessionsRange)
            {
                bool isAvailableToday = dateTime.Date == DateTime.Today && !userSessions.Contains(session) && session.TimeOfDay > DateTime.Now.TimeOfDay;
                bool isAvailableFuture = dateTime.Date != DateTime.Today && !userSessions.Contains(session);

                if (isAvailableToday || isAvailableFuture)
                {
                    RSGymUtility.WriteMessage($"({session.ToShortTimeString()}) ");
                    available = true;
                }
            }

            if (!available)
            {
                RSGymUtility.WriteMessage("Nenhum horário disponível para hoje.", "", "\n");
            }

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
                    if (nextHour < maxTime)
                    {
                        RSGymUtility.WriteMessage($"Agendamento de sessões apenas á partir de {nextHour.Hour}:00.", "", "\n");
                    }
                    else
                    {
                        RSGymUtility.WriteMessage($"Agendamento de sessões das {minTime.TimeOfDay} ás {maxTime.TimeOfDay}.", "", "\n");
                    }
                    isDateTime = false;
                }
                else if (dateTime.TimeOfDay < minTime.TimeOfDay || dateTime.TimeOfDay > maxTime.TimeOfDay)
                {
                    RSGymUtility.WriteMessage($"Agendamento de sessões das {minTime.TimeOfDay} ás {maxTime.TimeOfDay}.", "", "\n");
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
                /*
                if (order.PtCode != ptCode && dateTime >= order.TrainingDateTime && dateTime < sessionEnd && order.OrderStatus != "Cancelado")
                {
                    RSGymUtility.WriteMessage("PT indisponível, escolha outro PT ou outra Data e hora.", "\n\n", "\n");
                    return false;
                }
                */
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

        internal static void ShowchangeSubmenuOptions()
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

                    RSGymUtility.WriteTitle("Alterar pedidos", "\n", "\n\n");
                    RSGymUtility.WriteMessage($"{user.Name}, Digite o código pedido\ne aperte 'Enter'", "", "\n\n");

                    RSGymUtility.WriteMessage("Número: ", "", "");

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

                bool isValid = false;
                string action;
                do
                {
                    Console.Clear();
                    RSGymUtility.WriteTitle("Alterar pedidos", "\n", "\n\n");
                    RSGymUtility.WriteMessage($"{user.Name}, insira o número do que deseja alterar no pedido", "", "\n\n");

                    ShowchangeSubmenuOptions();

                    RSGymUtility.WriteMessage($"Número: ", "\n", "");
                    answer = Console.ReadLine();

                    isValid = changeSubmenuOptions.TryGetValue(answer, out action);

                    if (!isValid)
                    {
                        RSGymUtility.WriteMessage("Número inválido.", "", "\n");
                        keepGoing = UserUtility.KeepGoing();
                    }
                        
                } while (!isValid && keepGoing);

                if (!keepGoing)
                {
                    return ordersList;
                };

                RunChangeSubmenu(action, order, personalTrainersList, user);

                
            } while (!isNumber || UserUtility.KeepGoing());
            
            return ordersList; 
        }


        internal static bool CheckSessionConclusion(DateTime trainingDateTime)
        {
            bool isConcluded = trainingDateTime.Date == DateTime.Now.Date && DateTime.Now >= trainingDateTime;
            return isConcluded;
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
                default:
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
                    FinishOrder(user);
                    return;
                }   
            }
        }

        internal static void RunPersonalTrainerChange(Order order, List<PersonalTrainer> personalTrainersList, User user)
        {
            string ptCode = GetPersonalTrainerCode(personalTrainersList, user.Name);
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

                if (UserUtility.KeepGoing())
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
            RSGymUtility.WriteMessage("Estado do pedido alterado com sucesso.", "\n", "\n");
            RSGymUtility.WriteMessage($"{order.FullOrder} ({order.Reason})", "", "\n\n");
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
                RSGymUtility.WriteMessage("Insira o número do pedido que deseja finalizar: ", "", "\n");

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
                
            order.OrderStatus = orderStatusOptions.Values.ElementAt(2);
            order.TrainingDateTime = order.TrainingDateTime.AddMinutes(60);
            ordersList.Remove(ordersList.FirstOrDefault(o => o.OrderId == order.OrderId));
            ordersList.Add(order);

            RSGymUtility.WriteMessage("Pedido finalizado com sucesso.", "", "\n");
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
                    RSGymUtility.WriteMessage($"{order.FullOrder} ({order.Reason})", "", "\n");
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
