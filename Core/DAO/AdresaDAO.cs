using Core.Storage;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SajamKnjigaProjekat.Core.DAO
{
    public class AdresaDAO
    {
        private readonly Storage<Adresa> _storage;
        private List<Adresa> listaAdresa;
        public AdresaDAO()
        {
            // storage će čuvati podatke u autor.txt fajlu
            _storage = new Storage<Adresa>("adresa.txt");
            // učitaj sve autore iz fajla u memorijsku listu
            listaAdresa = _storage.Load();
        }

        public List<Adresa> GetAll()
        {
            return listaAdresa;
        }

        public void Add(Adresa a)
        {
            listaAdresa.Add(a);
            _storage.Save(listaAdresa);
        }

        public void Remove(string vlasnikID)
        {
            // filtriramo listu da izbacimo adresu čiji je VlasnikID jednak datom ID-ju
            listaAdresa = listaAdresa
                .Where(a => a.VlasnikID != vlasnikID)
                .ToList();

            // odmah snimamo fajl
            _storage.Save(listaAdresa);
        }

        // Opcionalno: ažuriraj adresu postojećeg vlasnika
        public void Update(Adresa a)
        {
            var index = listaAdresa.FindIndex(x => x.VlasnikID == a.VlasnikID);
            if (index >= 0)
            {
                listaAdresa[index] = a;
                _storage.Save(listaAdresa);
            }
        }

        public void SaveAll(List<Adresa> lista)
        {
            listaAdresa = lista;
            _storage.Save(lista);
        }

        // Pronadji adresu po ID-ju 
        public Adresa GetByVlasnikID(string vlasnikID)
        {
            return listaAdresa.FirstOrDefault(a => a.VlasnikID == vlasnikID);
        }
    }
}
