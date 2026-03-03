using Core.Languages;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

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
                        if (DatumRodjenja == null) return "X";
                        break;

                    case nameof(Telefon):
                        if (string.IsNullOrWhiteSpace(Telefon)) return "X";
                        break;

                    case nameof(Email):
                        // Obavezan i mora biti u formatu nesto@nesto
                        if (string.IsNullOrWhiteSpace(Email)) return "X";
                        if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+$")) return "X";
                        break;

                    case nameof(GodinaClanstva):
                        if (string.IsNullOrWhiteSpace(GodinaClanstva)) return "X";
                        if (!int.TryParse(GodinaClanstva, out int god) || god <= 0) return "X";
                        // Ne može biti član duže nego što ima godina
                        if (DatumRodjenja.HasValue)
                        {
                            int starost = DateTime.Now.Year - DatumRodjenja.Value.Year;
                            if (god > starost) return "X";
                        }
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
