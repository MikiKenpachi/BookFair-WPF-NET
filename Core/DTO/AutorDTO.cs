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
            string rezultat = string.Empty;

            switch (columnName)
            {
                case nameof(Ime):
                    if (string.IsNullOrWhiteSpace(Ime))
                        rezultat = Core.Languages.Translator.Prevedi("errImeObavezno");
                    else if (Ime.Trim().Length < 2)
                        rezultat = Core.Languages.Translator.Prevedi("errImeKratko");
                    break;

                case nameof(Prezime):
                    if (string.IsNullOrWhiteSpace(Prezime))
                        rezultat = Core.Languages.Translator.Prevedi("errPrezimeObavezno");
                    break;

                case nameof(DatumRodjenja):
                    if (DatumRodjenja == null)
                        rezultat = Core.Languages.Translator.Prevedi("errDatumObavezan");
                    else if (DatumRodjenja > DateTime.Now)
                        rezultat = Core.Languages.Translator.Prevedi("errDatumBuducnost");
                    break;

                case nameof(Email):
                    if (string.IsNullOrWhiteSpace(Email))
                        rezultat = Core.Languages.Translator.Prevedi("errEmailObavezan");
                    else if (!Regex.IsMatch(Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                        rezultat = Core.Languages.Translator.Prevedi("errEmailFormat");
                    break;

                case nameof(BrojLk):
                    if (string.IsNullOrWhiteSpace(BrojLk))
                        rezultat = Core.Languages.Translator.Prevedi("errBrojLkObavezan");
                    else if (!Regex.IsMatch(BrojLk.Trim(), @"^\d{8}$"))
                        rezultat = Core.Languages.Translator.Prevedi("errBrojLkFormat");
                    break;

                case nameof(GodineIskustva):
                    if (string.IsNullOrWhiteSpace(GodineIskustva))
                        rezultat = Core.Languages.Translator.Prevedi("errIskustvoObavezno");
                    else if (!int.TryParse(GodineIskustva, out int god) || god < 0 || god > 70)
                        rezultat = Core.Languages.Translator.Prevedi("errIskustvoRaspon");
                    break;

                case nameof(Grad):
                    if (string.IsNullOrWhiteSpace(Grad))
                        rezultat = Core.Languages.Translator.Prevedi("errGradObavezan");
                    break;

                    // Dodaj ostala polja (Ulica, Broj, Drzava) po istom principu ako su obavezna
            }
            return rezultat;
        }
    }

}