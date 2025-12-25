using Core.Storage;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SajamKnjigaProjekat.Core.DAO
{
    public class AutorDAO
    {
        private readonly Storage<Autor> _storage;
        private List<Autor> listaAutora;
        public AutorDAO()
        {
            // storage će čuvati podatke u autor.txt fajlu
            _storage = new Storage<Autor>("autor.txt");
            // učitaj sve autore iz fajla u memorijsku listu
            listaAutora = _storage.Load();
        }

        public List<Autor> GetAll()
        {
            return listaAutora;
        }

        public void Add(Autor autor)
        {
            listaAutora.Add(autor);
            _storage.Save(listaAutora);
        }

        public void Remove(Autor autor)
        {
            listaAutora.Remove(autor);
            _storage.Save(listaAutora);
        }

        public void Save()
        {
            _storage.Save(listaAutora);
        }

        public Autor GetByLicnaKarta(string lk)
        {
            foreach (var p in listaAutora)
            {
                if (p.Broj_lk == lk)
                    return p;
            }
            return null;
        }

    }
}
