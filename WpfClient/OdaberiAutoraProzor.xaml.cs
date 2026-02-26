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
            // 1. Pokušaj kasta
            OdabraniAutor = listAutori.SelectedItem as Autor;

            // 2. Provera selekcije
            if (OdabraniAutor == null)
            {
                // Izvlačenje lokalizovanih tekstova
                string poruka = Application.Current.FindResource("msgOdaberiAutora").ToString();
                string naslov = Application.Current.FindResource("titleObavestenje").ToString();

                MessageBox.Show(poruka, naslov, MessageBoxButton.OK, MessageBoxImage.Warning);
                return; // Prekidamo metodu, prozor ostaje otvoren
            }

            // 3. Ako je sve OK
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
