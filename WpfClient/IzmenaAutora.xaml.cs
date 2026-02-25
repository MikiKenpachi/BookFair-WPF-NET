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
        // ── Selektovani autor (menjamo direktno u memoriji) ─────────────
        public Autor SelektovaniAutor { get; private set; }

        // ── Lista knjiga koje autor trenutno ima (za DataGrid) ───────────
        private ObservableCollection<Knjiga> _knjige;

        // ── Sve knjige u sistemu (potrebne za dijalog dodavanja) ─────────
        private readonly List<Knjiga> _sveKnjige;

        // ── DAO (za snimanje promena) ─────────────────────────────────────
        private readonly AutorDAO _autorDao = new AutorDAO();
        private readonly KnjigaDAO _knjigaDao = new KnjigaDAO();

        // ================================================================
        // Konstruktor
        // ================================================================
        /// <param name="autor">Autor koji se menja</param>
        /// <param name="sveKnjige">Sve knjige u sistemu — potrebno za tab Knjige</param>
        public IzmenaAutora(Autor autor, List<Knjiga> sveKnjige = null)
        {
            InitializeComponent();

            SelektovaniAutor = autor;
            _sveKnjige = sveKnjige ?? new List<Knjiga>();

            PopuniPolja();
            UcitajKnjige();
        }

        // ================================================================
        // Tab 1 — Informacije
        // ================================================================

        private void PopuniPolja()
        {
            if (SelektovaniAutor == null) return;

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
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtIme.Text) ||
                string.IsNullOrWhiteSpace(txtPrezime.Text))
            {
                MessageBox.Show("Ime i prezime su obavezni.", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SelektovaniAutor.Ime = txtIme.Text.Trim();
            SelektovaniAutor.Prezime = txtPrezime.Text.Trim();
            SelektovaniAutor.Email = txtEmail.Text.Trim();
            SelektovaniAutor.Telefon = txtTelefon.Text.Trim();
            SelektovaniAutor.Datum_rodjenja =
                dpDatumRodjenja.SelectedDate ?? SelektovaniAutor.Datum_rodjenja;

            if (int.TryParse(txtIskustvo.Text, out int iskustvo))
                SelektovaniAutor.Godine_iskustva = iskustvo;

            if (SelektovaniAutor.Adresa != null)
            {
                SelektovaniAutor.Adresa.Ulica = txtUlica.Text.Trim();
                SelektovaniAutor.Adresa.Broj = txtBroj.Text.Trim();
                SelektovaniAutor.Adresa.Grad = txtGrad.Text.Trim();
                SelektovaniAutor.Adresa.Drzava = txtDrzava.Text.Trim();
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // ================================================================
        // Tab 2 — Knjige
        // ================================================================

        /// <summary>
        /// Puni DataGrid knjigama koje autor već ima u svom SpisakKnjiga.
        /// SpisakKnjiga je popunjen u DataBinding.PoveziSve() pri startu aplikacije.
        /// </summary>
        private void UcitajKnjige()
        {
            _knjige = new ObservableCollection<Knjiga>(
                SelektovaniAutor.SpisakKnjiga ?? new List<Knjiga>());

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
                MessageBox.Show(
                    "Ne postoje knjige koje možete dodati ovom autoru.",
                    "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
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
            if (dgKnjige.SelectedItem is not Knjiga odabrana)
            {
                MessageBox.Show(
                    "Najpre odaberite knjigu iz tabele.",
                    "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var potvrda = MessageBox.Show(
                $"Da li ste sigurni da želite da uklonite knjigu \"{odabrana.Naziv}\" " +
                $"iz spiska knjiga autora {SelektovaniAutor.ImePrezime}?",
                "Uklanjanje knjige",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (potvrda != MessageBoxResult.Yes) return;

            // 1. Ukloni iz DataGrid-a
            _knjige.Remove(odabrana);

            // 2. Ukloni iz memorijskog spiska autora
            SelektovaniAutor.SpisakKnjiga?.Remove(odabrana);

            // 3. Bidirekciona veza — ukloni autora iz ListaAutora knjige
            odabrana.ListaAutora?.Remove(SelektovaniAutor);

            // 4. Snimi u fajlove
            _autorDao.Update(SelektovaniAutor);
            _knjigaDao.Save();
        }
    }
}
