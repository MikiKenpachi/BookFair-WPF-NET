using Core.DAO;
using SajamKnjigaProjekat.Core.DAO;
using SajamKnjigaProjekat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    /// <summary>
    /// Dijalog za dodavanje i izmenu knjige.
    /// Autor se postavlja/uklanja dugmadima [+] i [-] prema specifikaciji (sekcija 8.24, 8.25).
    /// </summary>
    public partial class DodajKnjiguProzor : Window
    {
        // ── DAO ─────────────────────────────────────────────────────────
        private readonly AutorDAO _autorDao = new AutorDAO();
        private readonly IzdavacDAO _izdavacDao = new IzdavacDAO();

        // ── Rezultat ─────────────────────────────────────────────────────
        public Knjiga NovaKnjiga { get; private set; }

        // ── Trenutno postavljeni autor (samo jedan, per spec) ────────────
        // Čuvamo referencu u memoriji; na Potvrdi je upisujemo u NovaKnjiga
        private Autor _trenutniAutor = null;

        // ── Sve liste za izbor ───────────────────────────────────────────
        private readonly List<Autor> _sviAutori;
        private readonly List<Izdavac> _sviIzdavaci;

        // ================================================================
        // Konstruktor
        // ================================================================
        public DodajKnjiguProzor(Knjiga knjiga = null)
        {
            InitializeComponent();

            // Učitaj autore i izdavače
            _sviAutori = _autorDao.GetAll();
            _sviIzdavaci = _izdavacDao.GetAll();

            // Popuni ComboBox za izdavače
            cbIzdavac.ItemsSource = _sviIzdavaci;

            // Popuni ComboBox za žanr
            PopuniZanrove();

            if (knjiga != null)
            {
                // REŽIM IZMENE
                NovaKnjiga = knjiga;
                PopuniPolja();
                this.Title = "Izmeni knjigu";
            }
            else
            {
                // REŽIM DODAVANJA
                NovaKnjiga = null;
                this.Title = "Dodaj novu knjigu";
            }

            OsveziDugmadAutora();
        }

        // ================================================================
        // Popunjavanje forme
        // ================================================================

        private void PopuniZanrove()
        {
            // Enum vrednosti direktno kao stavke ComboBox-a
            cbZanr.ItemsSource = Enum.GetValues(typeof(Knjiga.Zanrovi));
        }

        private void PopuniPolja()
        {
            txtISBN.Text = NovaKnjiga.ISBN;
            txtISBN.IsReadOnly = true;   // ISBN se ne menja pri izmeni
            txtISBN.Background = System.Windows.Media.Brushes.WhiteSmoke;
            txtNaziv.Text = NovaKnjiga.Naziv;
            txtCena.Text = NovaKnjiga.Cena;
            txtBrojStrana.Text = NovaKnjiga.Broj_strana;
            txtGodinaIzdanja.Text = NovaKnjiga.Godina_izdanja;

            // Žanr
            cbZanr.SelectedItem = NovaKnjiga.Zanr;

            // Izdavač
            if (NovaKnjiga.Izdavac != null)
                cbIzdavac.SelectedItem = _sviIzdavaci
                    .FirstOrDefault(i => i.Sifra == NovaKnjiga.Izdavac.Sifra);

            // Autor — uzimamo prvog (spec: jedno polje za autora)
            if (NovaKnjiga.ListaAutora != null && NovaKnjiga.ListaAutora.Count > 0)
            {
                _trenutniAutor = _sviAutori
                    .FirstOrDefault(a => a.Broj_lk == NovaKnjiga.ListaAutora[0].Broj_lk)
                    ?? NovaKnjiga.ListaAutora[0];

                txtAutor.Text = _trenutniAutor.ImePrezime;
            }
        }

        // ================================================================
        // Logika dugmadi [+] i [-] za autora
        // ================================================================

        /// <summary>
        /// [+] je aktivan samo ako knjiga NEMA autora.
        /// [-] je aktivan samo ako knjiga IMA autora.
        /// </summary>
        private void OsveziDugmadAutora()
        {
            bool imaAutora = _trenutniAutor != null;
            btnDodajAutora.IsEnabled = !imaAutora;
            btnUkloniAutora.IsEnabled = imaAutora;
        }

        // ── Dugme [+] — dodaj autora ─────────────────────────────────────
        private void BtnDodajAutora_Click(object sender, RoutedEventArgs e)
        {
            var prozor = new OdaberiAutoraProzor(_sviAutori)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (prozor.ShowDialog() == true && prozor.OdabraniAutor != null)
            {
                _trenutniAutor = prozor.OdabraniAutor;
                txtAutor.Text = _trenutniAutor.ImePrezime;
                OsveziDugmadAutora();
            }
        }

        // ── Dugme [-] — ukloni autora ────────────────────────────────────
        private void BtnUkloniAutora_Click(object sender, RoutedEventArgs e)
        {
            var potvrda = MessageBox.Show(
                $"Da li ste sigurni da želite da uklonite autora {_trenutniAutor?.ImePrezime} sa knjige?",
                "Uklanjanje autora",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (potvrda != MessageBoxResult.Yes) return;

            _trenutniAutor = null;
            txtAutor.Text = string.Empty;
            OsveziDugmadAutora();
        }

        // ================================================================
        // Potvrdi / Odustani
        // ================================================================

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Validacija obaveznih polja
            if (string.IsNullOrWhiteSpace(txtISBN.Text) ||
                string.IsNullOrWhiteSpace(txtNaziv.Text) ||
                string.IsNullOrWhiteSpace(txtCena.Text) ||
                string.IsNullOrWhiteSpace(txtGodinaIzdanja.Text))
            {
                MessageBox.Show("Molimo popunite sva obavezna polja (ISBN, Naziv, Cena, Godina).",
                    "Greška", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (cbZanr.SelectedItem == null)
            {
                MessageBox.Show("Molimo odaberite žanr.", "Greška",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kreiraj knjigu ako je dodavanje
            if (NovaKnjiga == null)
                NovaKnjiga = new Knjiga();

            // Popuni polja
            NovaKnjiga.ISBN = txtISBN.Text.Trim();
            NovaKnjiga.Naziv = txtNaziv.Text.Trim();
            NovaKnjiga.Cena = txtCena.Text.Trim();
            NovaKnjiga.Broj_strana = txtBrojStrana.Text.Trim();
            NovaKnjiga.Godina_izdanja = txtGodinaIzdanja.Text.Trim();
            NovaKnjiga.Zanr = (Knjiga.Zanrovi)cbZanr.SelectedItem;

            // Izdavač
            NovaKnjiga.Izdavac = cbIzdavac.SelectedItem as Izdavac;

            // Autor — postavljamo listu sa jednim autorom (ili praznu)
            if (NovaKnjiga.ListaAutora == null)
                NovaKnjiga.ListaAutora = new List<Autor>();

            // Uklonimo prethodnog autora pa dodamo novog (ako postoji)
            NovaKnjiga.ListaAutora.Clear();
            if (_trenutniAutor != null)
            {
                NovaKnjiga.ListaAutora.Add(_trenutniAutor);

                // Bidirekciona veza — dodaj knjigu u spisak autora
                if (_trenutniAutor.SpisakKnjiga == null)
                    _trenutniAutor.SpisakKnjiga = new List<Knjiga>();

                if (!_trenutniAutor.SpisakKnjiga.Contains(NovaKnjiga))
                    _trenutniAutor.SpisakKnjiga.Add(NovaKnjiga);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}