using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for IzmenaAutora.xaml
    /// Sadrži dva taba:
    ///   - Informacije : izmena osnovnih podataka autora
    ///   - Knjige      : prikaz, dodavanje i uklanjanje knjiga koje je autor napisao
    /// </summary>
    public partial class IzmenaAutora : Window
    {
        public Autor SelektovaniAutor { get; private set; }
        private ObservableCollection<Knjiga> _knjige;
        private readonly List<Knjiga> _sveKnjige;
        private readonly AutorDAO _autorDao = new AutorDAO();
        private readonly KnjigaDAO _knjigaDao = new KnjigaDAO();

        // ── Dodajemo DTO validator ──
        private AutorDTO _validator = new AutorDTO();

        public IzmenaAutora(Autor autor, List<Knjiga> sveKnjige, AutorDAO autorDao, KnjigaDAO knjigaDao)
        {
            InitializeComponent();

            SelektovaniAutor = autor;
            _sveKnjige = sveKnjige ?? new List<Knjiga>();

            PopuniPolja();
            UcitajKnjige();
        }

        private void PopuniPolja()
        {
            if (SelektovaniAutor == null) return;

            // Punimo UI polja
            txtBrojLK.Text = SelektovaniAutor.Broj_lk;
            txtIme.Text = SelektovaniAutor.Ime;
            txtPrezime.Text = SelektovaniAutor.Prezime;
            txtEmail.Text = SelektovaniAutor.Email;
            txtTelefon.Text = SelektovaniAutor.Telefon;
            txtIskustvo.Text = SelektovaniAutor.Godine_iskustva.ToString();
            dpDatumRodjenja.SelectedDate = SelektovaniAutor.Datum_rodjenja;

            if (SelektovaniAutor.Adresa != null)
            {
                txtUlica.Text = SelektovaniAutor.Adresa.Ulica;
                txtBroj.Text = SelektovaniAutor.Adresa.Broj;
                txtGrad.Text = SelektovaniAutor.Adresa.Grad;
                txtDrzava.Text = SelektovaniAutor.Adresa.Drzava;
            }

            // ── SINHRONIZACIJA SA VALIDATOROM ──
            // Odmah popunjavamo validator da bi "znao" trenutno stanje
            AzurirajValidatorIzForme();
        }

        private void AzurirajValidatorIzForme()
        {
            _validator.Ime = txtIme.Text;
            _validator.Prezime = txtPrezime.Text;
            _validator.Email = txtEmail.Text;
            _validator.Telefon = txtTelefon.Text;
            _validator.BrojLk = txtBrojLK.Text;
            _validator.DatumRodjenja = dpDatumRodjenja.SelectedDate;
            _validator.Ulica = txtUlica.Text;
            _validator.Broj = txtBroj.Text;
            _validator.Grad = txtGrad.Text;

            if (int.TryParse(txtIskustvo.Text, out int iskustvo))
                _validator.GodineIskustva = iskustvo;
        }

        private bool JeLiValidno()
        {
            // Lista svojstava koja proveravamo (mora se poklapati sa switch-om u AutorDTO)
            string[] polja = {
            nameof(AutorDTO.Ime), nameof(AutorDTO.Prezime), nameof(AutorDTO.Email),
            nameof(AutorDTO.BrojLk), nameof(AutorDTO.Ulica), nameof(AutorDTO.Grad)
        };

            foreach (var p in polja)
            {
                if (_validator[p] == "X") return false;
            }
            return true;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // 1. Prvo ažuriramo validator podacima koji su trenutno u TextBox-ovima
            AzurirajValidatorIzForme();

            // 2. Provera preko DTO-a
            if (!JeLiValidno())
            {
                string poruka = Application.Current.FindResource("msgPopuniteSvaPolja").ToString();
                string naslov = Application.Current.FindResource("errorTitle").ToString();
                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Ako je sve OK, prepisujemo u originalni objekat
            SelektovaniAutor.Ime = _validator.Ime.Trim();
            SelektovaniAutor.Prezime = _validator.Prezime.Trim();
            SelektovaniAutor.Email = _validator.Email.Trim();
            SelektovaniAutor.Telefon = _validator.Telefon.Trim();
            SelektovaniAutor.Broj_lk = _validator.BrojLk.Trim();
            SelektovaniAutor.Datum_rodjenja = _validator.DatumRodjenja ?? SelektovaniAutor.Datum_rodjenja;
            SelektovaniAutor.Godine_iskustva = _validator.GodineIskustva;

            if (SelektovaniAutor.Adresa != null)
            {
                SelektovaniAutor.Adresa.Ulica = _validator.Ulica.Trim();
                SelektovaniAutor.Adresa.Broj = _validator.Broj.Trim();
                SelektovaniAutor.Adresa.Grad = _validator.Grad.Trim();
                SelektovaniAutor.Adresa.Drzava = txtDrzava.Text.Trim();
            }

            this.DialogResult = true;
            this.Close();
        }

        // --- Ostatak koda (UcitajKnjige, BtnDodajKnjigu, itd.) ostaje isti ---
        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void UcitajKnjige()
        {
            _knjige = new ObservableCollection<Knjiga>(SelektovaniAutor.SpisakKnjiga ?? new List<Knjiga>());
            dgKnjige.ItemsSource = _knjige;
        }

        // ── Dugme "Dodaj knjigu" ─────────────────────────────────────────
        private void BtnDodajKnjigu_Click(object sender, RoutedEventArgs e)
        {
            // Skup ISBN-ova knjiga koje autor već ima — da ih ne nudimo ponovo
            var autoroveISBN = _knjige.Select(k => k.ISBN).ToHashSet();

            var dostupne = _sveKnjige
                .Where(k => !autoroveISBN.Contains(k.ISBN))
                .ToList();
            if (!dostupne.Any())
            {
                string poruka = Application.Current.FindResource("msgNemaKnjigaZaDodavanje").ToString();
                string naslov = Application.Current.FindResource("titleInfo").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var prozor = new OdaberiKnjiguProzor(dostupne)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (prozor.ShowDialog() == true && prozor.OdabranaKnjiga != null)
            {
                Knjiga odabrana = prozor.OdabranaKnjiga;

                // 1. Dodaj u DataGrid (ObservableCollection obaveštava UI automatski)
                _knjige.Add(odabrana);

                // 2. Dodaj u memorijsku listu autora
                if (SelektovaniAutor.SpisakKnjiga == null)
                    SelektovaniAutor.SpisakKnjiga = new List<Knjiga>();

                if (!SelektovaniAutor.SpisakKnjiga.Contains(odabrana))
                    SelektovaniAutor.SpisakKnjiga.Add(odabrana);

                // 3. Bidirekciona veza — dodaj autora u ListaAutora knjige
                if (odabrana.ListaAutora == null)
                    odabrana.ListaAutora = new List<Autor>();

                if (!odabrana.ListaAutora.Contains(SelektovaniAutor))
                    odabrana.ListaAutora.Add(SelektovaniAutor);

                // 4. Snimi u fajlove
                _autorDao.Update(SelektovaniAutor);
                _knjigaDao.Save();
            }
        }

        // ── Dugme "Ukloni knjigu" ────────────────────────────────────────
        private void BtnUkloniKnjigu_Click(object sender, RoutedEventArgs e)
        {
            var selektovane = dgKnjige.SelectedItems
                .Cast<Knjiga>()
                .ToList();

            if (!selektovane.Any())
            {
                MessageBox.Show(
                    Application.Current.FindResource("msgOdaberiKnjiguIzTabele").ToString(),
                    Application.Current.FindResource("titleObavestenje").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Poruka potvrde
            string start = Application.Current.FindResource("msgPotvrdaUklanjanjaStart").ToString();
            
            string naslov = Application.Current.FindResource("titleUklanjanjeKnjige").ToString();

            string popisKnjiga = string.Join("\n", selektovane.Select(k => $"• {k.Naziv}"));
            string punaPoruka = $"{start}\n{popisKnjiga}\n";

            var potvrda = MessageBox.Show(punaPoruka, naslov, MessageBoxButton.YesNo, MessageBoxImage.None);
            if (potvrda != MessageBoxResult.Yes) return;

            foreach (var knjiga in selektovane)
            {
                // 1. Ukloni iz DataGrid-a (ObservableCollection)
                _knjige.Remove(knjiga);
                // 2. Ukloni iz memorijskog spiska autora
                SelektovaniAutor.SpisakKnjiga?.Remove(knjiga);
                // 3. Bidirekciona veza
                knjiga.ListaAutora?.Remove(SelektovaniAutor);
            }

            // 4. Snimi promene — jednom nakon petlje
            _autorDao.Update(SelektovaniAutor);
            _knjigaDao.Save();
        }
    }
}
