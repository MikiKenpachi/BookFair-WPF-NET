using Core.Languages;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class AutorDTO : IDataErrorInfo
{
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public DateTime? DatumRodjenja { get; set; }
    public string Telefon { get; set; }
    public string Email { get; set; }
    public int GodineIskustva { get; set; }
    public string BrojLk { get; set; }
    public string Ulica { get; set; }
    public string Broj { get; set; }
    public string Grad { get; set; }

    public string Drzava { get; set; }

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
                    if (DatumRodjenja == null || DatumRodjenja > DateTime.Now) return "X";
                    break;

                case nameof(Telefon):
                    if (string.IsNullOrWhiteSpace(Telefon)) return "X";
                    break;


                case nameof(Drzava):
                    if (string.IsNullOrWhiteSpace(Telefon)) return "X";
                    break;

                case nameof(Email):
                    if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@")) return "X";
                    break;

                case nameof(GodineIskustva):
                    // Iskustvo obično ne može biti negativno
                    if (GodineIskustva < 0) return "X";
                    break;

                case nameof(BrojLk):
                    // Provera da li je uneto i da li ima npr. 9 cifara
                    if (string.IsNullOrWhiteSpace(BrojLk) || BrojLk.Length != 9) return "X";
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
            }
            return string.Empty;
        }
    }
}