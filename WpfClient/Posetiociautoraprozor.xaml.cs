using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace WpfClient
{
    /// <summary>
    /// Prikazuje sve posetioce koji na listi zelja imaju barem jednu
    /// knjigu izabranog autora. (#autor_posetioci)
    /// </summary>
    public partial class Posetiociautoraprozor : Window
    {
        // ICollectionView omogucava filtriranje bez promene originalne liste
        private ICollectionView _view;

        public Posetiociautoraprozor(List<Posetilac> posetioci, string imenAutora)
        {
            InitializeComponent(); // This requires the class to be 'partial' and a corresponding XAML file

            // Dopuni naslov imenom autora
            txtNaslov.Text += $" {imenAutora}";

            // Postavi view za pretragu
            _view = CollectionViewSource.GetDefaultView(posetioci);
            dgPosetioci.ItemsSource = _view;
        }

        // ----------------------------------------------------------------
        // Pretraga — ista pravila kao i u MainWindow.FiltrirajPosetioce():
        //   1 rec  → prezime sadrzi rec
        //   2 reci → prezime, ime
        //   3 reci → broj karte, ime, prezime
        // ----------------------------------------------------------------
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
                    var p = obj as Posetilac;
                    if (p == null) return false;

                    string prezime = p.Prezime?.ToLower() ?? "";
                    string ime = p.Ime?.ToLower() ?? "";
                    string karta = p.BrClanskeKarte?.ToLower() ?? "";

                    if (delovi.Length == 1)
                        return prezime.Contains(delovi[0]);
                    else if (delovi.Length == 2)
                        return prezime.Contains(delovi[0]) && ime.Contains(delovi[1]);
                    else // 3+
                        return karta.Contains(delovi[0]) && ime.Contains(delovi[1]) && prezime.Contains(delovi[2]);
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