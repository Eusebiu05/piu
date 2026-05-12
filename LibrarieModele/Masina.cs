using System;
using System.Collections.Generic;
using System.Linq;

namespace LibrarieModele
{
    public class Masina
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

        public int IdMasina { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string NumarInmatriculare { get; set; } = string.Empty;
        public int AnFabricatie { get; set; }
        public decimal PretPeZi { get; set; }
        public bool Disponibila { get; set; } = true;
        public List<string> Dotari { get; set; } = new List<string>();
        public string TipCarburant { get; set; } = "Diesel";
        public DateTime DataInmatriculare { get; set; } = DateTime.Today;
        public DateTime DataActualizare { get; set; } = DateTime.Today;

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