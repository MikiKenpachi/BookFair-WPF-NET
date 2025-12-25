using Core.Storage;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DAO
{
    public class IzdavacDAO
    {
        private readonly Storage<Izdavac> _storage;
        private List<Izdavac> listaIzdavac;
        public IzdavacDAO()
        {
            // storage će čuvati podatke u autor.txt fajlu
            _storage = new Storage<Izdavac>("izdavac.txt");
            // učitaj sve autore iz fajla u memorijsku listu
            listaIzdavac = _storage.Load();
        }



        public List<Izdavac> GetAll()
        {
            return listaIzdavac;
        }

        public void Add(Izdavac a)
        {
            listaIzdavac.Add(a);
            _storage.Save(listaIzdavac);
        }

        public void Remove(Izdavac a)
        {
            listaIzdavac.Remove(a);
            _storage.Save(listaIzdavac);
        }
    }
}
