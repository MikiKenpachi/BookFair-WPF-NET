using Core.DAO;
using Core.DTO;
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
        private readonly AutorDAO _autorDao = new AutorDAO();
        private readonly IzdavacDAO _izdavacDao = new IzdavacDAO();

        // ── Rezultat ─────────────────────────────────────────────────────
        public Knjiga NovaKnjiga { get; private set; }

        // ── Validator ────────────────────────────────────────────────────
        private KnjigaDTO _validator = new KnjigaDTO();

        private ObservableCollection<Autor> _odabraniAutori = new ObservableCollection<Autor>();

        // ── Sve liste za izbor ───────────────────────────────────────────
        private readonly List<Autor> _sviAutori;
        private readonly List<Izdavac> _sviIzdavaci;

        // ── Da li je ovo izmena (true) ili dodavanje (false) ─────────────
        private readonly bool _jeIzmena;

        // ================================================================
        // Konstruktor
        // ================================================================
        public DodajKnjiguProzor(Knjiga knjiga = null)
        {
            InitializeComponent();

            _sviIzdavaci = _izdavacDao.GetAll().ToList();
            _sviAutori = _autorDao.GetAll().ToList();
            PopuniZanrove();
            PopuniIzdavace();

            if (knjiga != null)
            {
                _jeIzmena = true;
                NovaKnjiga = knjiga;
                PopuniPolja();
                this.Title = Application.Current.FindResource("titleIzmeniKnjigu").ToString();
            }
            else
            {
                _jeIzmena = false;
                NovaKnjiga = null;
                this.Title = Application.Current.FindResource("titleDodajKnjigu").ToString();
            }
            OsveziDugmadAutora();
        }

        // ================================================================
        // Popunjavanje forme
        // ================================================================

        private void PopuniZanrove()
        {
            cbZanr.ItemsSource = Enum.GetValues(typeof(Knjiga.Zanrovi));
        }

        private void PopuniIzdavace()
        {
            cbIzdavac.ItemsSource = _sviIzdavaci;
            cbIzdavac.DisplayMemberPath = "Naziv";
        }

        private void PopuniPolja()
        {
            if (NovaKnjiga == null) return;

            txtISBN.Text = NovaKnjiga.ISBN;
            txtISBN.IsReadOnly = true;   // ISBN se ne menja pri izmeni
            txtISBN.Background = System.Windows.Media.Brushes.WhiteSmoke;
            txtNaziv.Text = NovaKnjiga.Naziv;
            txtCena.Text = NovaKnjiga.Cena;
            txtBrojStrana.Text = NovaKnjiga.Broj_strana;
            txtGodinaIzdanja.Text = NovaKnjiga.Godina_izdanja;

            cbZanr.SelectedItem = NovaKnjiga.Zanr;

            if (NovaKnjiga.Izdavac != null)
                cbIzdavac.SelectedItem = _sviIzdavaci.FirstOrDefault(i => i.Sifra == NovaKnjiga.Izdavac.Sifra);

            _odabraniAutori.Clear();
            if (NovaKnjiga.ListaAutora != null)
            {
                foreach (var stub in NovaKnjiga.ListaAutora)
                {
                    var pravi = _sviAutori.FirstOrDefault(a => a.Broj_lk == stub.Broj_lk);
                    if (pravi != null && !_odabraniAutori.Contains(pravi))
                        _odabraniAutori.Add(pravi);
                }
            }
            OsveziPrikazAutora();
        }

        // ================================================================
        // Validacija pomoću DTO
        // ================================================================

        private void SinhronizujValidatorIzForme()
        {
            // Pri izmeni ISBN ne validiramo jer je readonly i već validan
            _validator.ISBN = _jeIzmena ? "0000000000" : txtISBN.Text;
            _validator.Naziv = txtNaziv.Text;
            _validator.Cena = txtCena.Text;
            _validator.BrojStrana = txtBrojStrana.Text;
            _validator.GodinaIzdanja = txtGodinaIzdanja.Text;
            _validator.Zanr = cbZanr.SelectedItem as Knjiga.Zanrovi?;
            _validator.Izdavac = cbIzdavac.SelectedItem as Izdavac;
            _validator.Autori = _odabraniAutori.ToList();
        }

        private bool JeLiValidno()
        {
            // Koje kolone proveravamo — ISBN preskačemo ako je izmena
            var polja = new List<string>();
            if (!_jeIzmena) polja.Add(nameof(KnjigaDTO.ISBN));
            polja.Add(nameof(KnjigaDTO.Naziv));
            polja.Add(nameof(KnjigaDTO.Cena));
            polja.Add(nameof(KnjigaDTO.BrojStrana));
            polja.Add(nameof(KnjigaDTO.GodinaIzdanja));
            polja.Add(nameof(KnjigaDTO.Zanr));
            polja.Add(nameof(KnjigaDTO.Izdavac));

            // Validno je samo ako NIJEDNO polje ne vraća poruku o grešci
            foreach (var p in polja)
            {
                if (!string.IsNullOrEmpty(_validator[p])) return false;
            }
            return true;
        }

        // ================================================================
        // Logika dugmadi [+] i [-] za autora
        // ================================================================

        private void OsveziDugmadAutora()
        {
            btnDodajAutora.IsEnabled = _odabraniAutori.Count < _sviAutori.Count;
            btnUkloniAutora.IsEnabled = _odabraniAutori.Count > 0;
        }

        private void BtnDodajAutora_Click(object sender, RoutedEventArgs e)
        {
            var dostupni = _sviAutori
                .Where(a => !_odabraniAutori.Any(x => x.Broj_lk == a.Broj_lk))
                .ToList();

            if (!dostupni.Any())
            {
                MessageBox.Show(Application.Current.FindResource("msgSviAutoriDodati").ToString(),
                    Application.Current.FindResource("titleInfo").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var prozor = new OdaberiAutoraProzor(dostupni) { Owner = this };

            if (prozor.ShowDialog() == true && prozor.OdabraniAutor != null)
            {
                _odabraniAutori.Add(prozor.OdabraniAutor);
                OsveziDugmadAutora();
                OsveziPrikazAutora();
            }
        }

        private void BtnUkloniAutora_Click(object sender, RoutedEventArgs e)
        {
            var prozor = new OdaberiAutoraProzor(_odabraniAutori.ToList()) { Owner = this };

            if (prozor.ShowDialog() == true && prozor.OdabraniAutor != null)
            {
                string porukaStart = Application.Current.FindResource("msgUklonitiAutora").ToString();
                string porukaKraj = Application.Current.FindResource("msgUklonitiAutoraPitanje").ToString();
                string naslov = Application.Current.FindResource("titleUklanjanjeAutora").ToString();

                var potvrda = MessageBox.Show($"{porukaStart} {prozor.OdabraniAutor.ImePrezime} {porukaKraj}",
                    naslov, MessageBoxButton.YesNo, MessageBoxImage.Question);

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
            // 1. Sinhronizuj UI -> Validator
            SinhronizujValidatorIzForme();

            // 2. Proveri validnost i prikaži konkretnu poruku za prvo nevalidno polje
            if (!_jeIzmena && !string.IsNullOrEmpty(_validator[nameof(KnjigaDTO.ISBN)]))
            {
                MessageBox.Show(_validator[nameof(KnjigaDTO.ISBN)],
                    Application.Current.FindResource("titleValidacija").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(_validator[nameof(KnjigaDTO.Naziv)]))
            {
                MessageBox.Show(_validator[nameof(KnjigaDTO.Naziv)],
                    Application.Current.FindResource("titleValidacija").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(_validator[nameof(KnjigaDTO.Cena)]))
            {
                MessageBox.Show(_validator[nameof(KnjigaDTO.Cena)],
                    Application.Current.FindResource("titleValidacija").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(_validator[nameof(KnjigaDTO.BrojStrana)]))
            {
                MessageBox.Show(_validator[nameof(KnjigaDTO.BrojStrana)],
                    Application.Current.FindResource("titleValidacija").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(_validator[nameof(KnjigaDTO.GodinaIzdanja)]))
            {
                MessageBox.Show(_validator[nameof(KnjigaDTO.GodinaIzdanja)],
                    Application.Current.FindResource("titleValidacija").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(_validator[nameof(KnjigaDTO.Zanr)]))
            {
                MessageBox.Show(_validator[nameof(KnjigaDTO.Zanr)],
                    Application.Current.FindResource("titleValidacija").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!string.IsNullOrEmpty(_validator[nameof(KnjigaDTO.Izdavac)]))
            {
                MessageBox.Show(_validator[nameof(KnjigaDTO.Izdavac)],
                    Application.Current.FindResource("titleValidacija").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 3. Kreiraj knjigu ako je dodavanje
            if (NovaKnjiga == null)
                NovaKnjiga = new Knjiga();

            // 4. Mapiranje iz DTO u Model
            if (!_jeIzmena)
                NovaKnjiga.ISBN = _validator.ISBN.Trim();
            NovaKnjiga.Naziv = _validator.Naziv.Trim();
            NovaKnjiga.Cena = _validator.Cena.Trim();
            NovaKnjiga.Broj_strana = _validator.BrojStrana.Trim();
            NovaKnjiga.Godina_izdanja = _validator.GodinaIzdanja.Trim();
            NovaKnjiga.Zanr = _validator.Zanr.Value;
            NovaKnjiga.Izdavac = _validator.Izdavac;

            // 5. Upravljanje listom autora (bidirekciona veza)
            if (NovaKnjiga.ListaAutora == null)
                NovaKnjiga.ListaAutora = new List<Autor>();

            var uklonjeni = NovaKnjiga.ListaAutora
                .Where(a => !_odabraniAutori.Any(x => x.Broj_lk == a.Broj_lk))
                .ToList();

            foreach (var autor in uklonjeni)
                autor.SpisakKnjiga?.Remove(NovaKnjiga);

            NovaKnjiga.ListaAutora = _odabraniAutori.ToList();

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

        private void OsveziPrikazAutora()
        {
            txtAutor.Text = string.Join(", ", _odabraniAutori.Select(a => a.ImePrezime));
        }
    }
}