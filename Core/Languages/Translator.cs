using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Languages
{
    public static class Translator
    {
        // Funkcija koju ćemo dodeliti iz WPF-a
        public static Func<string, string> Prevedi { get; set; } = (kljuc) => kljuc;
    }
}
