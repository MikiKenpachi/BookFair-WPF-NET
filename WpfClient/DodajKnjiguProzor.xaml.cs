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

        public Knjiga NovaKnjiga { get; private set; }
        private KnjigaDTO _validator = new KnjigaDTO();
        private ObservableCollection<Autor> _odabraniAutori = new ObservableCollection<Autor>();

        private readonly List<Autor> _sviAutori;
        private readonly List<Izdavac> _sviIzdavaci;
        private readonly bool _jeIzmena;

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

            this.DataContext = _validator;

            // Pretplata na promenu u validatoru da bismo osvežili dugme
            _validator.PropertyChanged += (s, e) => OsveziDugmePotvrdi();

            OsveziDugmadAutora();
            OsveziDugmePotvrdi();
        }

        private void PopuniZanrove()
        {
            cbZanr.ItemsSource = Enum.GetValues(typeof(Knjiga.Zanrovi));
        }

        private void PopuniIzdavace()
        {
            cbIzdavac.ItemsSource = _sviIzdavaci;
        }

        private void PopuniPolja()
        {
            if (NovaKnjiga == null) return;

            _validator.ISBN = NovaKnjiga.ISBN;
            _validator.Naziv = NovaKnjiga.Naziv;
            _validator.Cena = NovaKnjiga.Cena;
            _validator.BrojStrana = NovaKnjiga.Broj_strana;
            _validator.GodinaIzdanja = NovaKnjiga.Godina_izdanja;
            _validator.Zanr = NovaKnjiga.Zanr;
            _validator.Izdavac = _sviIzdavaci.FirstOrDefault(i => i.Sifra == NovaKnjiga.Izdavac?.Sifra);

            txtISBN.IsReadOnly = true;
            txtISBN.Background = System.Windows.Media.Brushes.WhiteSmoke;

            _odabraniAutori.Clear();
            if (NovaKnjiga.ListaAutora != null)
            {
                foreach (var stub in NovaKnjiga.ListaAutora)
                {
                    var pravi = _sviAutori.FirstOrDefault(a => a.Broj_lk == stub.Broj_lk);
                    if (pravi != null) _odabraniAutori.Add(pravi);
                }
            }
            OsveziPrikazAutora();
        }

        // --- VALIDACIJA ---

        private void OsveziDugmePotvrdi()
        {
            if (btnPotvrdi != null)
                btnPotvrdi.IsEnabled = FormaJeValidna();
        }

        private bool FormaJeValidna()
        {
            // 1. Provera polja iz DTO (mora vratiti string.Empty da bi bilo validno)
            string[] polja = {
                nameof(_validator.ISBN), nameof(_validator.Naziv), nameof(_validator.Cena),
                nameof(_validator.BrojStrana), nameof(_validator.GodinaIzdanja),
                nameof(_validator.Zanr), nameof(_validator.Izdavac)
            };

            foreach (var p in polja)
            {
                // Kod izmene ignorišemo ISBN jer je sivo polje
                if (_jeIzmena && p == nameof(_validator.ISBN)) continue;

                if (_validator[p] == "X") return false;
            }

            // 2. Dodatna provera: Mora postojati bar jedan autor
            if (_odabraniAutori.Count == 0) return false;

            // 3. Provera da osnovne vrednosti nisu null (za inicijalni ulazak)
            return !string.IsNullOrWhiteSpace(_validator.Naziv) &&
                   !string.IsNullOrWhiteSpace(_validator.ISBN) &&
                   _validator.Zanr != null &&
                   _validator.Izdavac != null;
        }

        // --- DOGAĐAJI ---

        private void BtnDodajAutora_Click(object sender, RoutedEventArgs e)
        {
            var dostupni = _sviAutori.Where(a => !_odabraniAutori.Any(x => x.Broj_lk == a.Broj_lk)).ToList();
            var prozor = new OdaberiAutoraProzor(dostupni) { Owner = this };

            if (prozor.ShowDialog() == true && prozor.OdabraniAutor != null)
            {
                _odabraniAutori.Add(prozor.OdabraniAutor);
                OsveziDugmadAutora();
                OsveziPrikazAutora();
                OsveziDugmePotvrdi(); // Obavezno osvežiti nakon promene liste
            }
        }

        private void BtnUkloniAutora_Click(object sender, RoutedEventArgs e)
        {
            var prozor = new OdaberiAutoraProzor(_odabraniAutori.ToList()) { Owner = this };
            if (prozor.ShowDialog() == true && prozor.OdabraniAutor != null)
            {
                _odabraniAutori.Remove(prozor.OdabraniAutor);
                OsveziDugmadAutora();
                OsveziPrikazAutora();
                OsveziDugmePotvrdi(); // Obavezno osvežiti nakon promene liste
            }
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (NovaKnjiga == null) NovaKnjiga = new Knjiga();

            NovaKnjiga.ISBN = _validator.ISBN?.Trim();
            NovaKnjiga.Naziv = _validator.Naziv?.Trim();
            NovaKnjiga.Cena = _validator.Cena?.Trim();
            NovaKnjiga.Broj_strana = _validator.BrojStrana?.Trim();
            NovaKnjiga.Godina_izdanja = _validator.GodinaIzdanja?.Trim();
            NovaKnjiga.Zanr = _validator.Zanr.Value;
            NovaKnjiga.Izdavac = _validator.Izdavac;

            // Bidirekciona veza
            foreach (var a in _sviAutori) a.SpisakKnjiga?.Remove(NovaKnjiga);

            NovaKnjiga.ListaAutora = _odabraniAutori.ToList();
            foreach (var autor in NovaKnjiga.ListaAutora)
            {
                if (autor.SpisakKnjiga == null) autor.SpisakKnjiga = new List<Knjiga>();
                if (!autor.SpisakKnjiga.Contains(NovaKnjiga)) autor.SpisakKnjiga.Add(NovaKnjiga);
            }

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void OsveziDugmadAutora()
        {
            btnDodajAutora.IsEnabled = _odabraniAutori.Count < _sviAutori.Count;
            btnUkloniAutora.IsEnabled = _odabraniAutori.Count > 0;
        }

        private void OsveziPrikazAutora()
        {
            txtAutor.Text = string.Join(", ", _odabraniAutori.Select(a => a.ImePrezime));
        }
    }
}