using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SajamKnjigaProjekat.Core.DAO
{
    public class KnjigaDAO
    {
        private List<Knjiga> listaKnjiga = new List<Knjiga>();

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
