using SajamKnjigaProjekat.Core.Models;
using System.Collections.Generic;
using System.Windows;

namespace WpfClient
{
    /// <summary>
    /// Dijalog za odabir autora koji se dodaje knjizi.
    /// Prikazuje sve autore registrovane u sistemu.
    /// </summary>
    public partial class OdaberiAutoraProzor : Window
    {
        /// <summary>
        /// Autor kojeg je referent odabrao. Null ako je kliknuo Odustani.
        /// </summary>
        public Autor OdabraniAutor { get; private set; }

        public OdaberiAutoraProzor(List<Autor> sviAutori)
        {
            InitializeComponent();
            listAutori.ItemsSource = sviAutori;
        }

        private void BtnPotvrdi_Click(object sender, RoutedEventArgs e)
        {
            if (listAutori.SelectedItem is Autor odabran)
            {
                OdabraniAutor = odabran;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Molimo odaberite autora iz liste.",
                    "Obaveštenje", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
