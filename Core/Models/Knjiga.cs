using System;
using System.Collections.Generic;

namespace SajamKnjigaProjekat.Core.Models
{
    public class Knjiga
    {


        public string ISBN;
        public string Naziv;
        public string Zanr;
        public string Godina_izdanja;
        public string Cena;
        public string Broj_strana;
        public List<Autor> ListaAutora= new List<Autor>();
        public string Izdavac;
        public List<string> Kupili;
        public List<string> Na_listi_zelja;

        public Knjiga()
        {

        }

        public Knjiga(string iSBN, string naziv, string zanr, string godina_izdanja, string cena, string broj_strana, List<Autor> autori, string izdavac, List<string> kupili, List<string> na_Listi_Zelja)
        {
            ISBN = iSBN;
            Naziv = naziv;
            Zanr = zanr;
            Godina_izdanja = godina_izdanja;
            Cena = cena;
            Broj_strana = broj_strana;
            ListaAutora = autori;
            Izdavac = izdavac;
            Kupili = kupili;
            Na_listi_zelja = na_Listi_Zelja;
        }
    }
}
