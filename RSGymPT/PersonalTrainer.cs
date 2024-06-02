using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace RSGymPT
{
    internal class PersonalTrainer
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

        internal int PersonalTrainerId { get; }
        internal int NextId { get; set; } = 1;
        internal string FullName { get; set; }
        internal string CellPhone { get; set; }
        internal string PtCode { get; set; }

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

        internal PersonalTrainer()
        {
            PersonalTrainerId = NextId++;
            FullName = string.Empty;
            CellPhone = string.Empty;   
            PtCode = string.Empty;
        }
        // Fazer segundo construtor com inserção parâmetros obrigatórios

        internal PersonalTrainer(string fullName, string cellPhone, string ptCode)
        {
            PersonalTrainerId = NextId++;
            FullName = fullName;
            CellPhone = cellPhone;
            PtCode = ptCode;
        }
        #endregion


        #region Methods (public or internal)

        // Method to create fictitious users
        internal static List<PersonalTrainer> CreatePersonalTrainer()
        {
            List<PersonalTrainer> personalTrainers = new List<PersonalTrainer>();

            PersonalTrainer personalTrainer01 = new PersonalTrainer("Eduardo Cabrita", "999888777", "PT001");
            PersonalTrainer personalTrainer02 = new PersonalTrainer("Perseu Antunes", "999777666", "PT002");
            PersonalTrainer personalTrainer03 = new PersonalTrainer("Klaus Ofner", "999777666", "PT003");

            personalTrainers.Add(personalTrainer01);
            personalTrainers.Add(personalTrainer02);
            personalTrainers.Add(personalTrainer03);

            return personalTrainers;
        }

        internal static string AskPtCode()
        {
            RSGymUtility.WriteMessage("Digite o código do PT: ");

            string ptCode = Console.ReadLine();

            return ptCode;

        }

        internal static PersonalTrainer FindCode(List<PersonalTrainer> personalTrainers) 
        {
            string ptCode = AskPtCode().ToUpper();

            foreach (PersonalTrainer ptrainer in personalTrainers)
            {
                if (ptrainer.PtCode == ptCode)
                {
                    return ptrainer;
                }
            }
            return null;
        }

        internal static void FindPersonalTrainer(List<PersonalTrainer> personalTrainers)
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Pesquisar Código do Personal Trainer", "", "\n\n");

            PersonalTrainer personalTrainer = FindCode(personalTrainers);

            if (personalTrainer != null)
            {
                RSGymUtility.WriteMessage($"O código {personalTrainer.PtCode} pertence ao Personal Trainer: \nNome: {personalTrainer.FullName}\nTelemóvel: {personalTrainer.CellPhone}", "\n", "\n");
            }
            else
            {
                RSGymUtility.WriteMessage("Código inexistente.", "", "\n");
            }
        }

        #endregion
    }
}

