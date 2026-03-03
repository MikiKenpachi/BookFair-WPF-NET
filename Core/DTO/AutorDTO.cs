using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

public class AutorDTO : IDataErrorInfo
{
    public string Ime { get; set; }
    public string Prezime { get; set; }
    public DateTime? DatumRodjenja { get; set; }
    public string Telefon { get; set; }
    public string Email { get; set; }
    public string GodineIskustva { get; set; }
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
                    if (Ime.Trim().Length < 2) return "X";
                    break;

                case nameof(Prezime):
                    if (string.IsNullOrWhiteSpace(Prezime)) return "X";
                    if (Prezime.Trim().Length < 2) return "X";
                    break;

                case nameof(DatumRodjenja):
                    if (DatumRodjenja == null) return "X";
                    if (DatumRodjenja > DateTime.Now) return "X";
                    break;

                case nameof(Telefon):
                    if (string.IsNullOrWhiteSpace(Telefon)) return "X";
                    if (!Regex.IsMatch(Telefon.Trim(), @"^[0-9+\-\s\(\)]{6,20}$")) return "X";
                    break;

                case nameof(Email):
                    if (string.IsNullOrWhiteSpace(Email)) return "X";
                    if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+$")) return "X";
                    break;

                case nameof(GodineIskustva):
                    if (string.IsNullOrWhiteSpace(GodineIskustva)) return "X";
                    if (!int.TryParse(GodineIskustva, out int god) || god < 0) return "X";
                    if (god > 70) return "X";
                    break;

                case nameof(BrojLk):
                    if (string.IsNullOrWhiteSpace(BrojLk)) return "X";
                    if (!Regex.IsMatch(BrojLk.Trim(), @"^\d{8}$")) return "X";
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