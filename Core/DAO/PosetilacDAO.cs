using SajamKnjigaProjekat.Core.Models;
using Core.Storage;
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

        public List<Posetilac> GetALL()
        {
            return listaPosetilaca;
        }

        public void Add(Posetilac p)
        {
            listaPosetilaca.Add(p);
            _storage.Save(listaPosetilaca);
        }

        public void Remove(Posetilac p)
        {
            listaPosetilaca.Remove(p);
            _storage.Save(listaPosetilaca); 
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

