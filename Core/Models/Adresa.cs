using System;
using System.Collections.Generic;
using Core.Storage.Serialization;
namespace SajamKnjigaProjekat.Core.Models
{   
    public class Adresa : ISerializable
    {

        public string Ulica { get; set; }
        public string Broj { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }

        public string VlasnikID { get; set; }  // novo polje


        public Adresa(){ }

        public Adresa(string vlasnikID, string ulica, string grad, string broj, string drzava)
        {
            VlasnikID = vlasnikID;
            Ulica = ulica;
            Grad = grad;
            Broj = broj;
            Drzava = drzava;
        }

        public string[] ToCSV()
        {
            return new string[]
            {
                VlasnikID,
                Ulica,
                Grad,
                Broj,
                Drzava
            };
        }

        public void FromCSV(string[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));

            // New format with VlasnikID first
            if (values.Length == 5)
            {
                VlasnikID = values[0];
                Ulica = values[1];
                Grad = values[2];
                Broj = values[3];
                Drzava = values[4];
                return;
            }

            // Legacy format without VlasnikID (older files) - assume VlasnikID missing
            if (values.Length == 4)
            {
                VlasnikID = string.Empty;
                Ulica = values[0];
                Grad = values[1];
                Broj = values[2];
                Drzava = values[3];
                return;
            }

            throw new ArgumentException("Neispravan CSV za adresu.", nameof(values));
        }
    }
}

