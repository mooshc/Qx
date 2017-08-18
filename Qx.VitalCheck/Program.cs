using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qx.Common;

namespace Qx.VitalCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.Out.WriteLine("Trying to connect the server...");
                var check = RemoteObjectProvider.GetLiteUserAccess().IsLoginCorrect("moosh", "thirnzho");
                if (check == null)
                    throw new Exception("User returned empty!");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Out.WriteLine("Service is alive!");
            }
            catch(Exception e)
            {
                Console.Out.WriteLine("\nError:");
                Console.Out.WriteLine(e.Message);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Out.WriteLine("\nService is dead!!!");
            }
            Console.ResetColor();
            Console.Out.WriteLine("Press any key to exit...");
            Console.In.Read();
        }
    }
}
