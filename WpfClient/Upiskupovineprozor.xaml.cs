using SajamKnjigaProjekat.Core.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfClient
{
    /// <summary>
    /// Dijalog za upis kupovine knjige sa liste zelja
    /// </summary>
    public partial class UpisKupovineProzor : Window
    {
        public Kupovina NovaKupovina { get; private set; }

        private readonly Knjiga _knjiga;
        private readonly Posetilac _posetilac;

        public UpisKupovineProzor(Posetilac posetilac, Knjiga knjiga)
        {
            InitializeComponent();
            _posetilac = posetilac;
            _knjiga = knjiga;

            // ISBN i naziv su read-only, upisujemo programski
            txtISBN.Text = knjiga.ISBN;
            txtNaziv.Text = knjiga.Naziv;

            // Podrazumevani datum je danas
            dpDatumKupovine.SelectedDate = DateTime.Today;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            // Validacija ocene
            // Validacija ocene
            if (!int.TryParse(txtOcena.Text, out int ocena) || ocena < 1 || ocena > 5)
            {
                // Povlačimo lokalizovane tekstove iz rečnika
                string poruka = Application.Current.FindResource("msgNevalidnaOcena").ToString();
                string naslov = Application.Current.FindResource("titleGreska").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validacija datuma
            if (!dpDatumKupovine.SelectedDate.HasValue)
            {
                // Dodajemo novi ključ "msgUnesiteDatum" u rečnike
                string poruka = Application.Current.FindResource("msgUnesiteDatum").ToString();
                string naslov = Application.Current.FindResource("titleGreska").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            NovaKupovina = new Kupovina(
                _posetilac,
                _knjiga,
                dpDatumKupovine.SelectedDate.Value,
                ocena,
                txtKomentar.Text
            );

            this.DialogResult = true;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        // Validacija ocene u realnom vremenu - aktivira/deaktivira dugme Potvrdi
        private void txtOcena_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (btnPotvrdi == null) return;
            bool ocenaOk = int.TryParse(txtOcena.Text, out int o) && o >= 1 && o <= 5;
            btnPotvrdi.IsEnabled = ocenaOk;
        }
    }
}
