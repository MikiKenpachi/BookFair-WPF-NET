using Core.DAO;
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

        //private Autor _trenutniAutor = null;   --->  ne koristimo?

        private ObservableCollection<Autor> _odabraniAutori = new ObservableCollection<Autor>();

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

            listAutori.ItemsSource = _odabraniAutori;

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

            // Učitaj SVE autore knjige u kolekciju
            _odabraniAutori.Clear();
            if (NovaKnjiga.ListaAutora != null)
            {
                foreach (var stub in NovaKnjiga.ListaAutora)
                {
                    // Pronađi pravi objekat iz sviAutori (stub ima samo Broj_lk)
                    var pravi = _sviAutori.FirstOrDefault(a => a.Broj_lk == stub.Broj_lk);
                    if (pravi != null && !_odabraniAutori.Contains(pravi))
                        _odabraniAutori.Add(pravi);
                }
            }
            OsveziPrikazAutora(); // Show authors in TextBox
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
            // [+] je aktivan sve dok ima autora koji nisu dodati
            btnDodajAutora.IsEnabled = _odabraniAutori.Count < _sviAutori.Count;

            // [-] je aktivan samo ako ima barem jedan autor na listi
            btnUkloniAutora.IsEnabled = _odabraniAutori.Count > 0;
        }

        // ── Dugme [+] — dodaj autora ─────────────────────────────────────
        private void BtnDodajAutora_Click(object sender, RoutedEventArgs e)
        {
            // Prikaži samo autore koji NISU već dodati
            var dostupni = _sviAutori
                .Where(a => !_odabraniAutori.Any(x => x.Broj_lk == a.Broj_lk))
                .ToList();

            if (!dostupni.Any())
            {
                MessageBox.Show("Svi autori su već dodati knjizi.",
                    "Informacija", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var prozor = new OdaberiAutoraProzor(dostupni)
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (prozor.ShowDialog() == true && prozor.OdabraniAutor != null)
            {
                _odabraniAutori.Add(prozor.OdabraniAutor);
                OsveziDugmadAutora();
                OsveziPrikazAutora(); // Update the TextBox
            }
        }

        // ── Dugme [-] — ukloni autora ────────────────────────────────────
        private void BtnUkloniAutora_Click(object sender, RoutedEventArgs e)
        {
            // Prikaži prozor za izbor autora za uklanjanje
            var prozor = new OdaberiAutoraProzor(_odabraniAutori.ToList())
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            if (prozor.ShowDialog() == true && prozor.OdabraniAutor != null)
            {
                var potvrda = MessageBox.Show(
                    $"Ukloniti autora {prozor.OdabraniAutor.ImePrezime} sa knjige?",
                    "Uklanjanje autora",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (potvrda != MessageBoxResult.Yes) return;

                _odabraniAutori.Remove(prozor.OdabraniAutor);
                OsveziDugmadAutora();
                OsveziPrikazAutora();
            }
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

            // Postavi sve odabrane autore
            if (NovaKnjiga.ListaAutora == null)
                NovaKnjiga.ListaAutora = new List<Autor>();

            // Ukloni stare autore koji više nisu na listi
            var uklonjeni = NovaKnjiga.ListaAutora
                .Where(a => !_odabraniAutori.Any(x => x.Broj_lk == a.Broj_lk))
                .ToList();

            foreach (var autor in uklonjeni)
                autor.SpisakKnjiga?.Remove(NovaKnjiga);

            // Postavi novu listu
            NovaKnjiga.ListaAutora = _odabraniAutori.ToList();

            // Bidirekciona veza — svaki autor dobija referencu na ovu knjigu
            foreach (var autor in NovaKnjiga.ListaAutora)
            {
                if (autor.SpisakKnjiga == null)
                    autor.SpisakKnjiga = new List<Knjiga>();

                if (!autor.SpisakKnjiga.Contains(NovaKnjiga))
                    autor.SpisakKnjiga.Add(NovaKnjiga);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Add or update this method to show all authors separated by comma and space
        private void OsveziPrikazAutora()
        {
            txtAutor.Text = string.Join(", ", _odabraniAutori.Select(a => a.ImePrezime));
        }
    }
}