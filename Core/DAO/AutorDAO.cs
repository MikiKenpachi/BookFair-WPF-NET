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
            //_storage.Save(listaAutora);
        }

        public void Remove(Autor autor)
        {
            listaAutora.Remove(autor);
            //_storage.Save(listaAutora);
        }

        public void Save()
        {
            _storage.Save(listaAutora);
        }

        public void SaveAll(List<Autor> lista)
        {
            listaAutora = lista;
            _storage.Save(lista);
        }
        public void Update(Autor a)
        {
            // 1. Učitamo sve (ako već nisu u memoriji)
            List<Autor> svi = GetAll();

            // 2. Pronađemo index postojećeg posetioca
            int index = svi.FindIndex(x => x.Broj_lk == a.Broj_lk);

            if (index != -1)
            {
                // 3. Zamenimo starog novim
                svi[index] = a;

                // 4. Snimimo nazad u fajl (pozivamo tvoju postojeću metodu za snimanje)
                listaAutora = svi;
            }
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
