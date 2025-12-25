using System;
using System.Collections.Generic;

namespace SajamKnjigaProjekat.Core.Models
{
    public class Knjiga
    {
        public string ISBN { get; set; }
        public string Naziv { get; set; }
        public string Zanr { get; set; }
        public string Godina_izdanja { get; set; }
        public string Cena { get; set; }
        public string Broj_strana { get; set; }
        public List<Autor> ListaAutora { get; set; } = new List<Autor>();
        public string Izdavac { get; set; }
        public List<Posetilac> Kupili { get; set; } = new List<Posetilac>();
        public List<Posetilac> Na_listi_zelja { get; set; } = new List<Posetilac>();

        public Knjiga() { }

        public Knjiga(string iSBN, string naziv, string zanr, string godina_izdanja, string cena, string broj_strana, List<Autor> listaAutora, string izdavac, List<Posetilac> kupili, List<Posetilac> na_listi_zelja)
        {
            ISBN = iSBN;
            Naziv = naziv;
            Zanr = zanr;
            Godina_izdanja = godina_izdanja;
            Cena = cena;
            Broj_strana = broj_strana;
            ListaAutora = listaAutora;
            Izdavac = izdavac;
            Kupili = kupili;
            Na_listi_zelja = na_listi_zelja;
        }

        public void DodajListuAutora(Autor autor)
        {
            if (!ListaAutora.Contains(autor))
                ListaAutora.Add(autor);
        }

        public void DodajuKupili(Posetilac posetilac)
        {
            if (!Kupili.Contains(posetilac))
                Kupili.Add(posetilac);
        }
        public void DodajuListuZelja(Posetilac posetilac)
        {
            if (!Na_listi_zelja.Contains(posetilac))
                Na_listi_zelja.Add(posetilac);
        }


    }
}
