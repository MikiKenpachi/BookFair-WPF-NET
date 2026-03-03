using Core.Storage;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Core.DAO
{
    public class IzdavacDAO
    {
        private readonly Storage<Izdavac> _storage;
        private List<Izdavac> listaIzdavac;

        public IzdavacDAO()
        {
            _storage = new Storage<Izdavac>("izdavac.txt");
            listaIzdavac = _storage.Load();
        }

        public List<Izdavac> GetAll()
        {
            return listaIzdavac;
        }

        // Samo menja in-memory listu — fajl se NE dira.
        // Snimanje isključivo kroz Save() / SaveAll().
        public void Add(Izdavac a)
        {
            listaIzdavac.Add(a);
            //_storage.Save(listaIzdavac);
        }

        public void Remove(Izdavac a)
        {
            listaIzdavac.Remove(a);
            //_storage.Save(listaIzdavac);
        }

        public void Update(Izdavac izmenjeniIzdavac)
        {
            List<Izdavac> sviIzdavaci = GetAll();

            for (int i = 0; i < sviIzdavaci.Count; i++)
            {
                if (sviIzdavaci[i].Sifra == izmenjeniIzdavac.Sifra)
                {
                    sviIzdavaci[i] = izmenjeniIzdavac;
                    break;
                }
            }

            listaIzdavac = sviIzdavaci;
            // NE pozivamo Save() ovde — sačuvaće se na dugme Save u MainWindow.
        }

        public void Save()
        {
            _storage.Save(listaIzdavac);
        }

        public void SaveAll(List<Izdavac> lista)
        {
            listaIzdavac = lista;
            _storage.Save(lista);
        }
    }
}
