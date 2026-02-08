using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Storage
{
    public static class DataBinding
    {
        public static void PoveziSve(
            List<Posetilac> posetioci,
            List<Knjiga> knjige,
            List<Autor> autori,
            List<Izdavac> izdavaci,
            List<Kupovina> kupovine)
        {
            PoveziKnjigeIAutore(knjige, autori);
            PoveziKnjigeIIzdavace(knjige, izdavaci);
            PoveziKupovine(posetioci, knjige, kupovine);
            PoveziIzdavace(izdavaci, autori, knjige);
        }

        // -----------------------------

        private static void PoveziKnjigeIAutore(List<Knjiga> knjige, List<Autor> autori)
        {
            foreach (var knjiga in knjige)
            {
                var povezaniAutori = knjiga.ListaAutora
                    .Select(a => autori.FirstOrDefault(x => x.Broj_lk == a.Broj_lk))
                    .Where(a => a != null)
                    .ToList();

                knjiga.ListaAutora = povezaniAutori;

                foreach (var autor in povezaniAutori)
                {
                    autor.DodajSpisakKnjiga(knjiga);
                }
            }
        }

        // -----------------------------

        private static void PoveziKnjigeIIzdavace(List<Knjiga> knjige, List<Izdavac> izdavaci)
        {
            foreach (var knjiga in knjige)
            {
                if (knjiga.Izdavac == null) continue;

                var izdavac = izdavaci
                    .FirstOrDefault(i => i.Sifra == knjiga.Izdavac.Sifra);

                if (izdavac != null)
                {
                    knjiga.Izdavac = izdavac;
                    izdavac.DodajKnjigu(knjiga);
                }
            }
        }

        // -----------------------------

        private static void PoveziKupovine(
            List<Posetilac> posetioci,
            List<Knjiga> knjige,
            List<Kupovina> kupovine)
        {
            foreach (var kupovina in kupovine)
            {
                var posetilac = posetioci
                    .FirstOrDefault(p => p.BrClanskeKarte == kupovina.Posetilac.BrClanskeKarte);

                var knjiga = knjige
                    .FirstOrDefault(k => k.ISBN == kupovina.Knjiga.ISBN);

                if (posetilac == null || knjiga == null)
                    continue;

                kupovina.Posetilac = posetilac;
                kupovina.Knjiga = knjiga;

                posetilac.DodajKupovinu(knjiga);
                knjiga.DodajuKupili(posetilac);

                // lista želja (ako želiš kasnije)
                // knjiga.DodajuListuZelja(posetilac);
            }
        }

        // -----------------------------

        private static void PoveziIzdavace(
            List<Izdavac> izdavaci,
            List<Autor> autori,
            List<Knjiga> knjige)
        {
            foreach (var izdavac in izdavaci)
            {
                if (izdavac.SefIzdavaca != null)
                {
                    var sef = autori
                        .FirstOrDefault(a => a.Broj_lk == izdavac.SefIzdavaca.Broj_lk);

                    if (sef != null)
                        izdavac.SefIzdavaca = sef;
                }

                izdavac.ListaAutora = autori
                    .Where(a => a.SpisakKnjiga.Any(k => k.Izdavac == izdavac))
                    .Distinct()
                    .ToList();
            }
        }
    }
}
