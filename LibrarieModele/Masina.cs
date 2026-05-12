using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LibrarieModele
{
    public class Masina : INotifyPropertyChanged
    {
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';
        private const char SEPARATOR_DOTARI_FISIER = ',';

        private const int ID = 0;
        private const int MARCA = 1;
        private const int MODEL = 2;
        private const int NUMAR_INMATRICULARE = 3;
        private const int AN_FABRICATIE = 4;
        private const int PRET_PE_ZI = 5;
        private const int DISPONIBILA = 6;
        private const int DOTARI = 7;
        private const int TIP_CARBURANT = 8;
        private const int DATA_INMATRICULARE = 9;
        private const int DATA_ACTUALIZARE = 10;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int idMasina;
        public int IdMasina
        {
            get => idMasina;
            set
            {
                idMasina = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DenumireAfisare));
            }
        }

        private string marca = string.Empty;
        public string Marca
        {
            get => marca;
            set
            {
                marca = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DenumireAfisare));
            }
        }

        private string model = string.Empty;
        public string Model
        {
            get => model;
            set
            {
                model = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DenumireAfisare));
            }
        }

        private string numarInmatriculare = string.Empty;
        public string NumarInmatriculare
        {
            get => numarInmatriculare;
            set
            {
                numarInmatriculare = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DenumireAfisare));
            }
        }

        private int anFabricatie;
        public int AnFabricatie
        {
            get => anFabricatie;
            set
            {
                anFabricatie = value;
                OnPropertyChanged();
            }
        }

        private decimal pretPeZi;
        public decimal PretPeZi
        {
            get => pretPeZi;
            set
            {
                pretPeZi = value;
                OnPropertyChanged();
            }
        }

        private bool disponibila = true;
        public bool Disponibila
        {
            get => disponibila;
            set
            {
                disponibila = value;
                OnPropertyChanged();
            }
        }

        private List<string> dotari = new List<string>();
        public List<string> Dotari
        {
            get => dotari;
            set
            {
                dotari = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DotariAfisare));
            }
        }

        private string tipCarburant = "Diesel";
        public string TipCarburant
        {
            get => tipCarburant;
            set
            {
                tipCarburant = value;
                OnPropertyChanged();
            }
        }

        private DateTime dataInmatriculare = DateTime.Today;
        public DateTime DataInmatriculare
        {
            get => dataInmatriculare;
            set
            {
                dataInmatriculare = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DataInmatriculareAfisare));
            }
        }

        private DateTime dataActualizare = DateTime.Today;
        public DateTime DataActualizare
        {
            get => dataActualizare;
            set
            {
                dataActualizare = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DataActualizareAfisare));
            }
        }

        public string DotariAfisare
        {
            get
            {
                if (Dotari != null && Dotari.Count > 0)
                {
                    return string.Join(", ", Dotari);
                }

                return "-";
            }
        }

        public string DataInmatriculareAfisare
        {
            get
            {
                return DataInmatriculare.ToShortDateString();
            }
        }

        public string DataActualizareAfisare
        {
            get
            {
                return DataActualizare.ToShortDateString();
            }
        }

        public string DenumireAfisare
        {
            get
            {
                return $"ID {IdMasina} - {Marca} {Model} ({NumarInmatriculare})";
            }
        }

        public Masina()
        {
        }

        public Masina(string linieFisier)
        {
            string[] dateFisier = linieFisier.Split(SEPARATOR_PRINCIPAL_FISIER);

            IdMasina = Convert.ToInt32(dateFisier[ID]);
            Marca = dateFisier[MARCA];
            Model = dateFisier[MODEL];
            NumarInmatriculare = dateFisier[NUMAR_INMATRICULARE];
            AnFabricatie = Convert.ToInt32(dateFisier[AN_FABRICATIE]);
            PretPeZi = Convert.ToDecimal(dateFisier[PRET_PE_ZI]);
            Disponibila = Convert.ToBoolean(dateFisier[DISPONIBILA]);

            if (dateFisier.Length > DOTARI && !string.IsNullOrWhiteSpace(dateFisier[DOTARI]))
            {
                Dotari = dateFisier[DOTARI]
                    .Split(SEPARATOR_DOTARI_FISIER, StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim())
                    .ToList();
            }
            else
            {
                Dotari = new List<string>();
            }

            if (dateFisier.Length > TIP_CARBURANT)
            {
                TipCarburant = dateFisier[TIP_CARBURANT];
            }

            if (dateFisier.Length > DATA_INMATRICULARE &&
                DateTime.TryParse(dateFisier[DATA_INMATRICULARE], out DateTime dataInmatriculare))
            {
                DataInmatriculare = dataInmatriculare;
            }

            if (dateFisier.Length > DATA_ACTUALIZARE &&
                DateTime.TryParse(dateFisier[DATA_ACTUALIZARE], out DateTime dataActualizare))
            {
                DataActualizare = dataActualizare;
            }
        }

        public string ConversieLaSirPentruFisier()
        {
            string sDotari = Dotari != null
                ? string.Join(SEPARATOR_DOTARI_FISIER.ToString(), Dotari)
                : string.Empty;

            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}",
                SEPARATOR_PRINCIPAL_FISIER,
                IdMasina,
                Marca,
                Model,
                NumarInmatriculare,
                AnFabricatie,
                PretPeZi,
                Disponibila,
                sDotari,
                TipCarburant,
                DataInmatriculare.ToShortDateString(),
                DataActualizare.ToShortDateString());
        }

        public string Info()
        {
            return $"ID: {IdMasina} | {Marca} {Model} | Nr: {NumarInmatriculare} | An: {AnFabricatie} | Pret/zi: {PretPeZi} lei | Disponibila: {Disponibila} | Carburant: {TipCarburant} | Dotari: {DotariAfisare}";
        }
    }
}