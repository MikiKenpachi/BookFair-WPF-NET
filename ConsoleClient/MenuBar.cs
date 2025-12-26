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
            Console.WriteLine("7. Dodaj autora");
            Console.WriteLine("8. Prikazi autora");
            Console.WriteLine("9. Izmeni autora");
            Console.WriteLine("10. Obrisi autora");
            Console.WriteLine("11. Prikaz knjige");
            Console.WriteLine("12. Obrisi knjigu");
            Console.WriteLine("13. Prikazi adrese");
            Console.WriteLine("0. Izlaz");
            Console.WriteLine("=========================");
            Console.Write("IZABERITE OPCIJU : ");

        }

        public static int UcitajOpciju()
        {
            while (true)
            {
                string unos = Console.ReadLine();

                if (int.TryParse(unos, out int opcija) && opcija >= 0 && opcija <= 13) //change upper limit when more options are added
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
