using System.Configuration;
using System.Data;
using System.Windows;



namespace WpfClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Forsiramo učitavanje Core biblioteke
            var forcedLoad = typeof(Core.Storage.DataBinding).Assembly;
            Core.Languages.Translator.Prevedi = (kljuc) =>
            {
                return Current.FindResource(kljuc)?.ToString() ?? kljuc;
            };

        }
        public void ChangeLanguage(string langCode)
        {
            ResourceDictionary dict = new ResourceDictionary();
            // Gledamo u lokalni folder Resources
            dict.Source = new Uri($"Resources/Dictionary-{langCode}.xaml", UriKind.Relative);



            this.Resources.MergedDictionaries.Clear();
            this.Resources.MergedDictionaries.Add(dict);
        }
    }

}
