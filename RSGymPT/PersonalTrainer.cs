// Purpose: Contains the PersonalTrainer class, which represents a personal trainer in the system.
// The class contains properties, constructors, and methods to create, find, validate, and list personal trainers.
// The class also contains a method to create and return 3 initial personal trainers.
// Restriction: The class is internal
// Version 1.0

// libraries to be used in the class
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace RSGymPT
{
    internal class PersonalTrainer
    {
        #region Fields (properties, private variables)
        
        #endregion

        #region Properties (public or internal)
        #region Auto-implemented properties 2.0
        
        internal int PersonalTrainerId { get; }
        private static int NextId { get; set; } = 1;
        internal string FullName { get; set; }
        internal string CellPhone { get; set; }
        internal string PtCode { get; set; }

        #endregion

        #region Bodied-expression properties 3.0
        // Property to show the full PT
        internal string FullPersonalTrainer => $"{PersonalTrainerId} - (Código): {PtCode}\n(Nome): {FullName}\n(Telemóvel): {CellPhone}";

        #endregion

        #endregion

        #region Constructors (public or internal)

        // Default constructor substitute
        internal PersonalTrainer()
        {
            PersonalTrainerId = NextId++;
            FullName = string.Empty;
            CellPhone = string.Empty;   
            PtCode = string.Empty;
        }

        // Second constructor with mandatory parameters
        internal PersonalTrainer(string fullName, string cellPhone, string ptCode)
        {
            PersonalTrainerId = NextId++;
            FullName = fullName;
            CellPhone = cellPhone;
            PtCode = ptCode;
        }

        #endregion

        #region Methods (public or internal)

        // Function to create and return 3 initial Personal Trainers
        internal static List<PersonalTrainer> CreatePersonalTrainersList()
        {
            List<PersonalTrainer> personalTrainersList = new List<PersonalTrainer>()
            {
                new PersonalTrainer("Eduardo Cabrita", "999888777", "PT001"),
                new PersonalTrainer("Perseu Antunes", "999777666", "PT002"),
                new PersonalTrainer("Grada Ofner", "999777666", "PT003")
            };

            return personalTrainersList;
        }

        // Function to find and return PTs
        internal static PersonalTrainer FindPersonalTrainerByCode(List<PersonalTrainer> personalTrainersList, string userName)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Pesquisar Código do Personal Trainer (PT)", "", "\n\n");
            RSGymUtility.WriteMessage($"{userName}, Digite o código de um dos\n(PT) disponíveis abaixo e aperte 'Enter'", "", "\n\n");

            ListPartialPersonalTrainer(personalTrainersList);

            string ptCode = PersonalTrainerUtility.AskPtCode();

            PersonalTrainer personalTrainer = ValidatePersonalTrainer(personalTrainersList, ptCode);

            if (personalTrainer == null)
            {
                RSGymUtility.WriteMessage("Código inexistente.", "\n", "\n");
            }

            return personalTrainer;
        }

        // Function to validate and return the PT 
        internal static PersonalTrainer ValidatePersonalTrainer(List<PersonalTrainer> personalTrainersList, string code)
        {
            PersonalTrainer personalTrainer = personalTrainersList.FirstOrDefault(c => c.PtCode == code);
            return personalTrainer;
        }

        // Method to show the full PT
        internal static void ShowFullPersonalTrainer(PersonalTrainer personalTrainer)
        {
            if (personalTrainer != null)
            {
                RSGymUtility.WriteMessage($"{personalTrainer.FullPersonalTrainer}", "\n", "\n");
            }   
        }

        // Method to list certain properties of PTs from a list
        internal static void ListPartialPersonalTrainer(List<PersonalTrainer> personalTrainersList)
        {
            foreach (PersonalTrainer pt in personalTrainersList)
            {
                RSGymUtility.WriteMessage($"({pt.PtCode}) - {pt.FullName} ");
            }
        }

        // Method to list all PTs from a list
        internal static void ListFullPersonalTrainers(List<PersonalTrainer> personalTrainersList, User user)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Lista de Personal Trainers", "\n", "\n\n");
            RSGymUtility.WriteMessage($"{user.Name}, Personal Trainers disponíveis: ", "", "\n\n");

            personalTrainersList.Sort((pt01, pt02) => pt01.FullName.CompareTo(pt02.FullName));

            foreach (PersonalTrainer personalTrainer in personalTrainersList)
            {
                RSGymUtility.WriteMessage(personalTrainer.FullPersonalTrainer, "", "\n\n");
            }
            RSGymUtility.PauseConsole();
        }

        #endregion
    }
}

