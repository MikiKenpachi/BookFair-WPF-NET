using SajamKnjigaProjekat.Core.Models;
using Core.Storage;
using System.Collections.Generic;


namespace SajamKnjigaProjekat.Core.DAO
{
    public class KnjigaDAO
    {
        private readonly Storage<Knjiga> _storage;
        private List<Knjiga> listaKnjiga;

        public KnjigaDAO() {
            _storage = new Storage<Knjiga>("knjiga.txt");

            // učitaj sve posetioce iz fajla u memorijsku listu
            listaKnjiga = _storage.Load();

        }

        public List<Knjiga> GetAll()
        {
            return listaKnjiga;
        }

        public void Add(Knjiga knjiga)
        {
            listaKnjiga.Add(knjiga);
            _storage.Save(listaKnjiga);
        }

        public void Remove(Knjiga knjiga)
        {
            listaKnjiga.Remove(knjiga);
            _storage.Save(listaKnjiga);
        }

        public void Save()
        {
            _storage.Save(listaKnjiga);
        }
        public Knjiga GetByISBN(string isbn)
        {
            foreach (var k in listaKnjiga)
            {
                if (k.ISBN == isbn)
                    return k;
            }
            return null;
        }
        
    }
}
