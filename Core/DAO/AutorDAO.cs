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

        private List<Autor> listaAutora = new List<Autor>();

        public List<Autor> GetAll()
        {
            return listaAutora;
        }

        public void Add(Autor autor)
        {
            listaAutora.Add(autor);
        }

        public void Remove(Autor autor)
        {
                    listaAutora.Remove(autor);
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

        public void Update(Autor autor)
        {
            var stara = GetByLicnaKarta(autor.Broj_lk);
            if (stara != null)
            {
                stara.Ime = autor.Ime;
                stara.Prezime = autor.Prezime;
                stara.Datum_rodjenja=autor.Datum_rodjenja;
                stara.Adresa = autor.Adresa;
                stara.Telefon=autor.Telefon; 
                stara.Godine_iskustva= autor.Godine_iskustva;
                stara.Email=autor.Email;
                stara.SpisakKnjiga=autor.SpisakKnjiga;
            }
        }

    }
}
