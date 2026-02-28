using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace WpfClient
{
    /// <summary>
    /// Prikazuje sve autore čije knjige se nalaze na listi želja
    /// izabranog posjetioca. (#posetilac_autori)
    /// </summary>
    public partial class Autoriposetilacaprozor : Window
    {
        private ICollectionView _view;

        public Autoriposetilacaprozor(List<Autor> autori, string imePrezimePosetioca)
        {
            InitializeComponent();

            txtNaslov.Text += $" {imePrezimePosetioca}";

            _view = CollectionViewSource.GetDefaultView(autori);
            dgAutori.ItemsSource = _view;
        }

        // Pretraga — ista pravila kao i za autore:
        //   1 rec  → prezime sadrzi rec
        //   2 reci → prezime, ime
        private void BtnPretrazi_Click(object sender, RoutedEventArgs e)
        {
            string upit = txtPretraga.Text.ToLower().Trim();

            if (string.IsNullOrWhiteSpace(upit))
            {
                _view.Filter = null;
            }
            else
            {
                string[] delovi = upit.Split(',');
                for (int i = 0; i < delovi.Length; i++)
                    delovi[i] = delovi[i].Trim();

                _view.Filter = obj =>
                {
                    var a = obj as Autor;
                    if (a == null) return false;

                    string prezime = a.Prezime?.ToLower() ?? "";
                    string ime = a.Ime?.ToLower() ?? "";

                    if (delovi.Length == 1)
                        return prezime.Contains(delovi[0]);
                    else
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);
                };
            }

            _view.Refresh();
        }

        private void BtnZatvori_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}