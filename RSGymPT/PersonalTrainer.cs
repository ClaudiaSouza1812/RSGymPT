using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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

        // Function to create and return 3 initial Personal Trainers
        internal static List<PersonalTrainer> CreatePersonalTrainers()
        {
            List<PersonalTrainer> personalTrainers = new List<PersonalTrainer>()
            {
                new PersonalTrainer("Eduardo Cabrita", "999888777", "PT001"),
                new PersonalTrainer("Perseu Antunes", "999777666", "PT002"),
                new PersonalTrainer("Klaus Ofner", "999777666", "PT003")
            };

            return personalTrainers;
        }

        
        internal static void FindPersonalTrainerByCode(List<PersonalTrainer> personalTrainers) 
        {
            Console.Clear();

            RSGymUtility.WriteTitle("Pesquisar Código do Personal Trainer", "", "\n\n");

            string ptCode = AskPtCode();

            PersonalTrainer personalTrainer = ValidatePersonalTrainer(personalTrainers, ptCode);

            if (personalTrainer == null)
            {
                RSGymUtility.WriteMessage("Código inexistente.", "", "\n");
                return;
            }

            personalTrainer.ShowPersonalTrainer();
        }

        // Function to ask and return the PT code
        internal static string AskPtCode()
        {
            RSGymUtility.WriteMessage("Digite o código do PT: ", "", "\n");

            string ptCode = Console.ReadLine().ToUpper();

            return ptCode;

        }

        internal static PersonalTrainer ValidatePersonalTrainer(List<PersonalTrainer> personalTrainers, string code)
        {
            var personalTrainer = personalTrainers.FirstOrDefault(c => c.PtCode == code);
            return personalTrainer;
        }

        internal void ShowPersonalTrainer()
        {
            RSGymUtility.WriteMessage($"O código {PtCode} pertence ao Personal Trainer: \nNome: {FullName}\nTelemóvel: {CellPhone}", "\n", "\n");
        }

        #endregion
    }
}

