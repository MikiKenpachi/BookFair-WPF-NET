using System;
using System.Collections.Generic;
using Core.Storage.Serialization;
namespace SajamKnjigaProjekat.Core.Models
{
    public class Izdavac :ISerializable
    {
        public string Sifra { get; set; }
        public string Naziv { get; set; }
        public Autor SefIzdavaca { get; set; }

        public List<Autor> ListaAutora { get; set; } = new List<Autor>();
        public List<Knjiga> ListaKnjiga { get; set; } = new List<Knjiga>();

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
                Sifra, Naziv

            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 2)
                throw new ArgumentException("Invalid CSV data for Autor.");

               Sifra=values[0];
               Naziv=values[1];   
              
         
        }
    }
}