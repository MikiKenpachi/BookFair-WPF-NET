using Core.Storage;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
    internal class ConsoleStarter
    {
        public static void Main(string[] args)
        {
            ConsoleApp app = new ConsoleApp();
           /* var marko = new Posetilac { Ime = "Marko", Prezime = "Markovic", BrClanskeKarte = "001" };
            var ana = new Posetilac { Ime = "Ana", Prezime = "Antic", BrClanskeKarte = "002" };
            var petar = new Posetilac { Ime = "Petar", Prezime = "Petrovic", BrClanskeKarte = "003" };

            var posetioci = new List<Posetilac> { marko, ana, petar };

            // -----------------------------
            // 2. Kreiramo autore
            var ivo = new Autor { Ime = "Ivo", Prezime = "Andric", Broj_lk = "11" };
            var mesa = new Autor { Ime = "Mesa", Prezime = "Selimovic", Broj_lk = "22" };

            var autori = new List<Autor> { ivo, mesa };

            // -----------------------------
            // 3. Kreiramo izdavace
            var laguna = new Izdavac { Sifra = "IZD01", Naziv = "Laguna" };
            var vulkan = new Izdavac { Sifra = "IZD02", Naziv = "Vulkan" };

            var izdavaci = new List<Izdavac> { laguna, vulkan };

            // -----------------------------
            // 4. Kreiramo knjige i dodeljujemo im autore/izdavace
            var knjiga1 = new Knjiga { ISBN = "1", Naziv = "Na Drini Cuprija", Izdavac = laguna };
            knjiga1.ListaAutora.Add(ivo);

            var knjiga2 = new Knjiga { ISBN = "2", Naziv = "Prokleta Avlija", Izdavac = vulkan };
            knjiga2.ListaAutora.Add(mesa);

            var knjige = new List<Knjiga> { knjiga1, knjiga2 };

            // -----------------------------
            // 5. Kreiramo kupovine
            var kupovina1 = new Kupovina { Posetilac = marko, Knjiga = knjiga1 };
            var kupovina2 = new Kupovina { Posetilac = marko, Knjiga = knjiga2 };
            var kupovina3 = new Kupovina { Posetilac = ana, Knjiga = knjiga2 };

            var kupovine = new List<Kupovina> { kupovina1, kupovina2, kupovina3 };

            // -----------------------------
            // 6. Pozivamo DataBinding
            DataBinding.PoveziSve(posetioci, knjige, autori, izdavaci, kupovine);

            // -----------------------------
            // 7. Proveravamo rezultate

            Console.WriteLine("=== Posetioci ===");
            foreach (var p in posetioci)
            {
                Console.WriteLine($"{p.Ime} {p.Prezime}, broj kupovina: {p.ListaKupovina.Count}");
            }

            Console.WriteLine("\n=== Knjige ===");
            foreach (var k in knjige)
            {
                var autoriStr = k.ListaAutora.Count > 0 ? string.Join(", ", k.ListaAutora.ConvertAll(a => a.Ime)) : "Nema autora";
                var kupiliStr = k.Kupili.Count > 0 ? string.Join(", ", k.Kupili.ConvertAll(p => p.Ime)) : "Niko nije kupio";
                var izdavacStr = k.Izdavac != null ? k.Izdavac.Naziv : "Nema izdavaca";

                Console.WriteLine($"Knjiga: {k.Naziv}, Autori: {autoriStr}, Kupili: {kupiliStr}, Izdavac: {izdavacStr}");
            }

            Console.WriteLine("\n=== Izdavaci ===");
            foreach (var i in izdavaci)
            {
                var autoriStr = i.ListaAutora.Count > 0 ? string.Join(", ", i.ListaAutora.ConvertAll(a => a.Ime)) : "Nema autora";
                Console.WriteLine($"Izdavac: {i.Naziv}, broj autora: {i.ListaAutora.Count}, Autori: {autoriStr}");
            }*/
            app.Run();
            Console.ReadLine();

        }
    }
}
