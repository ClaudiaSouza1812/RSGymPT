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

            Dictionary<User, User> usersDict = new Dictionary<User, User>();

            User user01 = new User("Claudia Simone de Souza", DateTime(1992, 12, 18), "clasi", "12345678");
            RSGymUtility.TerminateConsole();
        }
    }
}
