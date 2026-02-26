using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;
using System.Windows;

namespace WpfClient
{
    /// <summary>
    /// Dijalog za odabir knjige koja se dodaje autoru.
    /// Prikazuje samo knjige koje autor još nema u svom spisku.
    /// </summary>
    public partial class OdaberiKnjiguProzor : Window
    {
        /// <summary>
        /// Knjiga koju je referent odabrao. Null ako je kliknuo Odustani.
        /// </summary>
        public Knjiga OdabranaKnjiga { get; private set; }

        /// <param name="dostupneKnjige">
        ///   Lista knjiga koje autor još nema — filtrirana pre poziva.
        /// </param>
        public OdaberiKnjiguProzor(List<Knjiga> dostupneKnjige)
        {
            InitializeComponent();

            // Punimo ListBox dostupnim knjigama
            listKnjige.ItemsSource = dostupneKnjige;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (listKnjige.SelectedItem is Knjiga odabrana)
            {
                OdabranaKnjiga = odabrana;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                // Izvlačimo prevode iz Dictionary-ja
                string poruka = Application.Current.FindResource("msgOdaberiKnjiguUpozorenje").ToString();
                string naslov = Application.Current.FindResource("titleObavestenje").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
