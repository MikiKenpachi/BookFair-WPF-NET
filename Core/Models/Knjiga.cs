using System;
using System.Collections.Generic;
using Core.Storage.Serialization;

namespace SajamKnjigaProjekat.Core.Models
{
    public class Knjiga : ISerializable
    {

        public string ISBN;
        public string Naziv;
        public string Zanr;
        public string Godina_izdanja;
        public string Cena;
        public string Broj_strana;
        public List<Autor> ListaAutora;
        public string Izdavac;
        public List<string> Kupili;
        public List<string> Na_listi_zelja;

        // Implement ISerializable members here
        public string[] ToCSV()
        {
            return new string[]
            {
                ISBN,
                Naziv,
                Zanr,
                Godina_izdanja,
                Cena,
                Broj_strana,
                Izdavac,
                ListaAutora != null ? string.Join(";", ListaAutora) : "",
                Kupili != null ? string.Join(";", Kupili) : "",
                Na_listi_zelja != null ? string.Join(";", Na_listi_zelja) : ""
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null || values.Length < 10)
                throw new ArgumentException("Invalid CSV data for Knjiga.");

            ISBN = values[0];
            Naziv = values[1];
            Zanr = values[2];
            Godina_izdanja = values[3];
            Cena = values[4];
            Broj_strana = values[5];
            Izdavac = values[6];
            // ListaAutora deserialization would require more info about Autor
            Kupili = new List<string>(values[8].Split(';'));
            Na_listi_zelja = new List<string>(values[9].Split(';'));

       
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
            var posetilacString = posetilac.ToStringRepresentation();
            if (!Na_listi_zelja.Contains(posetilacString))
                Na_listi_zelja.Add(posetilacString);
        }


    }
}
