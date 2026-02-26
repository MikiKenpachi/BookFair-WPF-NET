using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class IzdavacDTO : IDataErrorInfo
    {
        public string Sifra { get; set; }
        public string Naziv { get; set; }
        public Autor SefIzdavaca { get; set; } // Pretpostavljamo da Autor ima property 'GodineIskustva'

        // Ove liste mogu ostati radi prikaza u DataGrid-u
        public List<Autor> ListaAutora { get; set; } = new List<Autor>();
        public List<Knjiga> ListaKnjiga { get; set; } = new List<Knjiga>();

        // --- IDataErrorInfo Implementacija ---

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string rezultat = string.Empty;

                switch (columnName)
                {
                    case nameof(Sifra):
                        if (string.IsNullOrWhiteSpace(Sifra))
                            // Pozivamo Translator koji će "pitati" WPF za pravi string
                            rezultat = Core.Languages.Translator.Prevedi("errSifraObavezna");
                        break;

                    case nameof(Naziv):
                        if (string.IsNullOrWhiteSpace(Naziv))
                            rezultat = Languages.Translator.Prevedi("errNazivPrazan");
                        else if (Naziv.Length < 3)
                            rezultat = Languages.Translator.Prevedi("errNazivKratak");
                        break;

                    case nameof(SefIzdavaca):
                        if (SefIzdavaca == null)
                        {
                            rezultat = Languages.Translator.Prevedi("errSefObavezan");
                        }
                        else if (SefIzdavaca.Godine_iskustva <= 5)
                        {
                            string poruka = Languages.Translator.Prevedi("errSefIskustvo");
                            rezultat = $"{poruka}{SefIzdavaca.Godine_iskustva}).";
                        }
                        break;
                }

                return rezultat;
            }
        }
    }
}
