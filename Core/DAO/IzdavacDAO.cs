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

        public void Save()
        {
            _storage.Save(listaIzdavac);
        }

        public void Update(Izdavac izmenjeniIzdavac)
        {
            // 1. Prvo učitamo trenutno stanje svih izdavača iz fajla
            List<Izdavac> sviIzdavaci = GetAll();

            // 2. Pronađemo starog izdavača u toj listi (koristimo Šifru kao jedinstveni ključ)
            for (int i = 0; i < sviIzdavaci.Count; i++)
            {
                if (sviIzdavaci[i].Sifra == izmenjeniIzdavac.Sifra)
                {
                    // 3. Menjamo stari objekat u listi novim podacima
                    sviIzdavaci[i] = izmenjeniIzdavac;
                    break;
                }
            }

            // 4. Sada tu osveženu listu snimamo nazad u fajl
            listaIzdavac = sviIzdavaci;
            Save();
        }

        // Pomoćna metoda za snimanje cele liste (da ne ponavljaš kod i kod Delete metode)
        

        public void Remove(Izdavac a)
        {
            listaIzdavac.Remove(a);
            _storage.Save(listaIzdavac);
        }

    }
}
