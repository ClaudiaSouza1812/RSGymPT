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

            
            User user01 = new User("Claudia Simone de Souza", new DateTime(1992, 12, 18), "clasi", "12345678");
            users.Add(user01);

            User user02 = new User();
            user02.Name = "Paula de Fátima Vallim Magalhães";
            user02.Birth = new DateTime(1984, 12, 08);
            user02.UserName = "paufa";
            user02.Password = "87654321";

            users.Add(user02);

            User.ListUser(users);

            RSGymUtility.TerminateConsole();
        }
    }
}
