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

        public void Remove(Adresa a)
        {
            listaAdresa.Remove(a);
            _storage.Save(listaAdresa);
        }
    }
}
