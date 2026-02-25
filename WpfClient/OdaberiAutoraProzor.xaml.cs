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
            OdabraniAutor = listAutori.SelectedItem as Autor;
            this.DialogResult = OdabraniAutor != null;
            this.Close();
        }

        private void BtnOdustani_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
