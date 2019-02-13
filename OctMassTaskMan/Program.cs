using System;

using OctMassTaskMan.Models.Utillities;

namespace OctMassTaskMan
{
    class Program
    {

        static void Main(string[] args)
        {
            var task = ConsoleExecution.ConsoleTask(args);
            task.Wait();
        }
    }
}
