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
        }

        public void Remove(Knjiga knjiga)
        {
            listaKnjiga.Remove(knjiga);
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
        public void Update(Knjiga knjiga)
        {
            var stara = GetByISBN(knjiga.ISBN);
            if (stara != null)
            {
                stara.Naziv = knjiga.Naziv;
                stara.Zanr = knjiga.Zanr;
                stara.Godina_izdanja = knjiga.Godina_izdanja;
                stara.Cena = knjiga.Cena;
                stara.Broj_strana = knjiga.Broj_strana;
                stara.ListaAutora = knjiga.ListaAutora;
                stara.Izdavac = knjiga.Izdavac;
                stara.Kupili = knjiga.Kupili;
                stara.Na_listi_zelja = knjiga.Na_listi_zelja;
            }
        }
    }
}
