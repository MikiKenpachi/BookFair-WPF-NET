using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace WpfClient
{
    /// <summary>
    /// Prikazuje sve knjige odabranog izdavača u DataGrid-u.
    /// Podržava pretragu po nazivu ili ISBN-u.
    /// </summary>
    public partial class KnjigeIzdavacaProzor : Window
    {
        // ICollectionView omogućava filtriranje bez menjanja originalne liste
        public ICollectionView KnjigeView { get; set; }

        /// <summary>
        /// Konstruktor prima listu knjiga izdavača i naziv izdavača za naslov.
        /// Lista je već filtrirana u IzdavaciProzor — ovde samo prikazujemo.
        /// </summary>
        public KnjigeIzdavacaProzor(List<Knjiga> knjige, string nazivIzdavaca)
        {
            InitializeComponent();

            // Kreiramo view iznad prosleđene liste — original se ne menja
            KnjigeView = CollectionViewSource.GetDefaultView(knjige);

            dgKnjige.ItemsSource = KnjigeView;

            // Dopunjavamo naslov imenom izdavača
            txtNaslov.Text += " : " + nazivIzdavaca;
        }

        // ================================================================
        // Pretraga
        // ================================================================

        private void BtnPretrazi_Click(object sender, RoutedEventArgs e)
        {
            FiltrirajKnjige(txtPretraga.Text);
        }

        /// <summary>
        /// Filtrira knjige po nazivu ili ISBN-u.
        /// Ako je polje prazno, prikazuju se sve knjige.
        /// </summary>
        private void FiltrirajKnjige(string upit)
        {
            if (KnjigeView == null) return;
            if (string.IsNullOrWhiteSpace(upit))
            {
                KnjigeView.Filter = null;
            }
            else
            {
                string f = upit.ToLower().Trim();
                KnjigeView.Filter = obj =>
                {
                    var k = obj as Knjiga;
                    if (k == null) return false;

                    return (k.ISBN?.ToLower().Contains(f) == true) ||
                           (k.Naziv?.ToLower().Contains(f) == true) ||
                           (k.Zanr.ToString().ToLower().Contains(f)) ||
                           (k.Godina_izdanja?.ToLower().Contains(f) == true) ||
                           (k.Cena?.ToLower().Contains(f) == true) ||
                           (k.Broj_strana?.ToLower().Contains(f) == true);
                };
            }
            KnjigeView.Refresh();
        }

        // ================================================================
        // Zatvori
        // ================================================================

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
