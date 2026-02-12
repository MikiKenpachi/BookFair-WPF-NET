using Core.Storage.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SajamKnjigaProjekat.Core.Models
{
    public class Izdavac : ISerializable
    {
        public string Sifra { get; set; }
        public string Naziv { get; set; }
        public Autor SefIzdavaca { get; set; }

        public List<Autor> ListaAutora { get; set; } = new List<Autor>();
        public List<Knjiga> ListaKnjiga { get; set; } = new List<Knjiga>();

        //public List<string> ListaKnjigaISBN { get; set; } = new List<string>();  nepotrebno ???

        public Izdavac(string sifra, string naziv, Autor sefIzdavaca, List<Autor> listaAutora, List<Knjiga> listaKnjiga)
        {
            Sifra = sifra;
            Naziv = naziv;
            SefIzdavaca = sefIzdavaca;
            ListaAutora = listaAutora;
            ListaKnjiga = listaKnjiga;
        }

        public Izdavac() { }

        public void DodajAutora(Autor autor)
        {
            if (!ListaAutora.Contains(autor))
                ListaAutora.Add(autor);
        }


        public void DodajKnjigu(Knjiga knjiga)
        {
            if(!ListaKnjiga.Contains(knjiga))
                ListaKnjiga.Add(knjiga);    
        }


        public string[] ToCSV()
        {
            return new string[]
            {
                Sifra,
                Naziv,
                SefIzdavaca != null ? SefIzdavaca.Broj_lk : "", 
                string.Join(";", ListaAutora.Select(a => a.Broj_lk)), 
                string.Join(";", ListaKnjiga.Select(k => k.ISBN))     
            };
        }

        public void FromCSV(string[] values)
        {
            Sifra = values[0];
            Naziv = values[1];
            SefIzdavaca = !string.IsNullOrEmpty(values[2]) ? new Autor { Broj_lk = values[2] } : null;

            // Prazne liste, popuniće se kroz DataBinding
            ListaAutora = new List<Autor>();
            ListaKnjiga = new List<Knjiga>();
        }

    }
}