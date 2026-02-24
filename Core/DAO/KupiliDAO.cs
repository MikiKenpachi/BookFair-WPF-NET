using Core.Storage;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class KupiliDAO
    {
        private readonly Storage<Kupovina> _storage;
        private List<Kupovina> listaKupovina;

        public KupiliDAO()
        {
            _storage = new Storage<Kupovina>("kupovina.txt");
            listaKupovina = _storage.Load();
        }

        public List<Kupovina> GetAll()
        {
            return listaKupovina;
        }

        // Sve kupovine jednog posetioca
        public List<Kupovina> GetByPosetilac(string brClanskeKarte)
        {
            return listaKupovina
                .Where(k => k.Posetilac?.BrClanskeKarte == brClanskeKarte)
                .ToList();
        }

        // Sve kupovine za odredjenu knjigu
        public List<Kupovina> GetByKnjiga(string isbn)
        {
            return listaKupovina
                .Where(k => k.Knjiga?.ISBN == isbn)
                .ToList();
        }

        // Da li je posjetilac vec kupio tu knjigu
        public bool PostojiKupovina(string brClanskeKarte, string isbn)
        {
            return listaKupovina.Any(k =>
                k.Posetilac?.BrClanskeKarte == brClanskeKarte &&
                k.Knjiga?.ISBN == isbn);
        }

        public void Add(Kupovina k)
        {
            listaKupovina.Add(k);
            _storage.Save(listaKupovina);
        }

        // Uklanja kupovinu po posetiocu i knjizi
        public void Remove(string brClanskeKarte, string isbn)
        {
            listaKupovina = listaKupovina
                .Where(k => !(k.Posetilac?.BrClanskeKarte == brClanskeKarte &&
                              k.Knjiga?.ISBN == isbn))
                .ToList();
            _storage.Save(listaKupovina);
        }

        // Uklanja konkretan objekat kupovine
        public void Remove(Kupovina kupovina)
        {
            listaKupovina.Remove(kupovina);
            _storage.Save(listaKupovina);
        }

        // Azurira prosecnu ocenu posetioca na osnovu svih njegovih kupovina
        public double IzracunajProsecnuOcenu(string brClanskeKarte)
        {
            var kupovine = GetByPosetilac(brClanskeKarte);
            if (!kupovine.Any()) return 0;
            return kupovine.Average(k => k.Ocena);
        }

        // Ukupno potroseno od strane posetioca
        public double IzracunajUkupnoPotroseno(string brClanskeKarte)
        {
            var kupovine = GetByPosetilac(brClanskeKarte);
            double ukupno = 0;
            foreach (var k in kupovine)
            {
                if (k.Knjiga != null && double.TryParse(k.Knjiga.Cena, out double cena))
                    ukupno += cena;
            }
            return ukupno;
        }

        public void Save()
        {
            _storage.Save(listaKupovina);
        }
    }
}
