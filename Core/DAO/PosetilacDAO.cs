using Core.Storage;
using Core.Storage.Serialization;
using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;

namespace SajamKnjigaProjekat.Core.DAO
{
    public class PosetilacDAO
    {
        private readonly Storage<Posetilac> _storage;
        private List<Posetilac> listaPosetilaca;

        public PosetilacDAO()
        {
            // storage će čuvati podatke u posetioci.txt fajlu
            _storage = new Storage<Posetilac>("posetilac.txt");

            // učitaj sve posetioce iz fajla u memorijsku listu
            listaPosetilaca = _storage.Load();
        }

        public List<Posetilac> GetAll()
        {
            return listaPosetilaca;
        }

        public void Save()
        {
            _storage.Save(listaPosetilaca);
        }

        public void SaveAll(List<Posetilac> lista)
        {
            listaPosetilaca = lista;
            _storage.Save(lista);
        }


        public void Add(Posetilac p)
        {
            listaPosetilaca.Add(p);
        }

        public void Remove(Posetilac p)
        {
            listaPosetilaca.Remove(p);
        }

        public void Update(Posetilac p)
        {
            // 1. Učitamo sve (ako već nisu u memoriji)
            List<Posetilac> svi = GetAll();

            // 2. Pronađemo index postojećeg posetioca
            int index = svi.FindIndex(x => x.BrClanskeKarte == p.BrClanskeKarte);

            if (index != -1)
            {
                // 3. Zamenimo starog novim
                svi[index] = p;

                // 4. Snimimo nazad u fajl (pozivamo tvoju postojeću metodu za snimanje)
                listaPosetilaca = svi;
            }
        }

        public Posetilac GetByClanskaKarta(string brKarte)
        {
            foreach (var p in listaPosetilaca)
            {
                if (p.BrClanskeKarte == brKarte)
                    return p;
            }
            return null;
        }
    }
}

