using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    public static class MenuBar
    {
        public static void PrikaziMeni()
        {
            Console.Clear();
            Console.WriteLine("=========================");
            Console.WriteLine(" SAJAM KNJIGA - MENI ");
            Console.WriteLine("1. Dodavanje posetioca");
            Console.WriteLine("2. Prikaz posetioca");
            Console.WriteLine("3. Izmena posetioca");
            Console.WriteLine("4. Brisanje posetioca");
            Console.WriteLine("5. Dodavanje knjige");
            Console.WriteLine("6. Izmena knjige");
            //TODO: implement remaining menu options
            Console.WriteLine("0. Izlaz");
            Console.WriteLine("=========================");
            Console.Write("IZABERITE OPCIJU : ");

        }

        public static int UcitajOpciju()
        {
            while (true)
            {
                string unos = Console.ReadLine();

                if (int.TryParse(unos, out int opcija) && opcija >= 0 && opcija <= 6) //change upper limit when more options are added
                {
                    return opcija;
                }

                else
                    Console.Write("\nNEVAZECA OPCIJA - ");
                    Console.Write("IZABERITE OPCIJU : ");

            }
        }
    } 
}
