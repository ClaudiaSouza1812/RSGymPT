﻿using System;
using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace RSGymPT
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Method to output characters encoded to UTF-8 
            RSGymUtility.SetUnicodeConsole();

            List<User> users = User.CreateUser();
            List<PersonalTrainer> personalTrainers = PersonalTrainer.CreatePersonalTrainer();

            UserUtility.StartRSGymProgram(users, personalTrainers);


            
            


            



            //Login.LogInUser();

            //User.ListUser(users);

            RSGymUtility.TerminateConsole();
        
        }
    }
}
