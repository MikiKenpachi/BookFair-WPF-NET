using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    public static class StatusBar
    {
        public static void PrikaziPoruku(string poruka)
        {
            Console.WriteLine("\n");
            Console.WriteLine($"STATUS :  {poruka}");
            Console.WriteLine("- - - - - - - - - - - - -");
            Console.WriteLine("Pritisni ENTER za nastavak...");
            Console.ReadLine();
        }
    }
}
