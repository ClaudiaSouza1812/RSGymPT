using System;
using System.Collections.Generic;
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
            RSGymUtility.SetUnicodeConsole();

            List<User> users = new List<User>();

            User.MakeUser(users);

            User.ListUser(users);

            RSGymUtility.TerminateConsole();
        
        }
    }
}
