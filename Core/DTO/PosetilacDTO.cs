using Core.Languages;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class PosetilacDTO : IDataErrorInfo
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime? DatumRodjenja { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string GodinaClanstva { get; set; }
        public string Ulica { get; set; }
        public string Broj { get; set; }
        public string Grad { get; set; }
        public string Drzava { get; set; }
        public StatusPosetioca Status { get; set; }

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Ime):
                        if (string.IsNullOrWhiteSpace(Ime)) return "X";
                        break;
                    case nameof(Prezime):
                        if (string.IsNullOrWhiteSpace(Prezime)) return "X";
                        break;
                    case nameof(DatumRodjenja):
                        if (DatumRodjenja == null)
                            return "Datum rođenja je obavezan.";

                        return null;
                    case nameof(Telefon):
                        if (string.IsNullOrWhiteSpace(Telefon)) return "X";
                        break;
                    case nameof(Email):
                        if (string.IsNullOrWhiteSpace(Email)) return "X";
                        break;
                    case nameof(GodinaClanstva):
                        if (string.IsNullOrWhiteSpace(GodinaClanstva)) return "X";
                        break;
                    case nameof(Ulica):
                        if (string.IsNullOrWhiteSpace(Ulica)) return "X";
                        break;
                    case nameof(Broj):
                        if (string.IsNullOrWhiteSpace(Broj)) return "X";
                        break;
                    case nameof(Grad):
                        if (string.IsNullOrWhiteSpace(Grad)) return "X";
                        break;
                    case nameof(Drzava):
                        if (string.IsNullOrWhiteSpace(Drzava)) return "X";
                        break;
                }
                return string.Empty;
            }
        }
    }
}
