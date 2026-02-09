using Core.Storage.Serialization;
using System;
using System.Collections.Generic;
using System.IO;


namespace Core.Storage
{
    /*
     * Klasa zadužena za trajno čuvanje podataka u fajl sistemu.
     * Koristi Serializer za CSV konverziju i radi sa generičkim tipom T.
     */
    public class Storage<T> where T : ISerializable, new()
    {
        private readonly string _filePath;
        private readonly Serializer<T> _serializer = new Serializer<T>();

        public Storage(string fileName)
        {
            // Uvek stabilna polazna tačka (Console + WPF)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // Penjemo se dok ne nadjemo Core folder
            DirectoryInfo dir = new DirectoryInfo(baseDir);

            while (dir != null && !Directory.Exists(Path.Combine(dir.FullName, "Core")))
            {
                dir = dir.Parent;
            }

            if (dir == null)
                throw new Exception("Core folder nije pronadjen. Proveri strukturu projekta.");

            // Core/Data folder
            string dataPath = Path.Combine(dir.FullName, "Core", "Data");

            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            _filePath = Path.Combine(dataPath, fileName);

            Console.WriteLine($"Fajl ce biti upisan u: {_filePath}");
        }

        public List<T> Load()
        {
            if (!File.Exists(_filePath))
            {
                // File.WriteAllText kreira fajl ako ne postoji
                File.WriteAllText(_filePath, string.Empty);
                return new List<T>();
            }

            var lines = File.ReadLines(_filePath);
            return _serializer.FromCSV(lines);
        }

        public void Save(List<T> objects)
        {
            string serializedObjects = _serializer.ToCSV(objects);
            File.WriteAllText(_filePath, serializedObjects);
            Console.WriteLine($"Podaci upisani u fajl: {_filePath}");
        }

    }
}
