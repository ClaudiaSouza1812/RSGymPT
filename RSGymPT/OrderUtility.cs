// Purpose: Contains the OrderUtility class. This class contains the functions to show the available sessions for the Personal Trainer (PT) and the User, to get the date and time of the training session, to check the Personal Trainer (PT) and User availability, and to check the session conclusion.
// Restriction: The class is internal
// Version 1.0

// Libraries to be used in the class
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace RSGymPT
{
    internal class OrderUtility
    {
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
            List<DateTime> ptSessions = Order.ordersList.Where(pt => pt.PtCode == ptCode && pt.TrainingDateTime.Date == dateTime.Date && pt.OrderStatus == "Agendado").Select(pt => pt.TrainingDateTime).ToList();

            return ptSessions;
        }

        // Function to get the User sessions for a specific date
        private static List<DateTime> GetUserSessionsForDate(User user, DateTime dateTime)
        {
            List<DateTime> userSessions = Order.ordersList.Where(u => u.UserId == user.UserId && u.TrainingDateTime.Date == dateTime.Date && u.OrderStatus == "Agendado").Select(u => u.TrainingDateTime).ToList();

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
            ShowPersonalTrainerAvailableSessions(Order.ordersList, ptCode, DateTime.Now);
            ShowUserAvailableSessions(Order.ordersList, user, DateTime.Now);
        }

        // Function to check the Personal Trainer (PT) availability
        internal static bool CheckPtAvailability(DateTime dateTime, string ptCode)
        {
            foreach (Order order in Order.ordersList)
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
            foreach (Order order in Order.ordersList)
            {
                DateTime sessionEnd = order.TrainingDateTime.AddMinutes(50);

                if (user.UserId == order.UserId && dateTime >= order.TrainingDateTime && dateTime < sessionEnd && order.OrderStatus != "Cancelado")
                {
                    Order.ListOrdersByUser(user);
                    RSGymUtility.WriteMessage("Sessões com duração de 50 minutos. Você já tem uma sessão agendada nesta data e horário, escolha outra Data e hora.", "\n", "\n");
                    return false;
                }
            }
            return true;
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
    }
}
