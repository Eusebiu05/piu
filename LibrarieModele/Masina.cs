namespace LibrarieModele
{
    public class Masina
    {
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';

        private const int ID = 0;
        private const int MARCA = 1;
        private const int MODEL = 2;
        private const int NUMAR_INMATRICULARE = 3;
        private const int AN_FABRICATIE = 4;
        private const int PRET_PE_ZI = 5;
        private const int DISPONIBILA = 6;

        public int IdMasina { get; set; }
        public string Marca { get; set; }
        public string Model { get; set; }
        public string NumarInmatriculare { get; set; }
        public int AnFabricatie { get; set; }
        public decimal PretPeZi { get; set; }
        public bool Disponibila { get; set; }

        public Masina()
        {
            Marca = string.Empty;
            Model = string.Empty;
            NumarInmatriculare = string.Empty;
            AnFabricatie = 0;
            PretPeZi = 0;
            Disponibila = true;
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
        }

        public string ConversieLaSirPentruFisier()
        {
            return string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                SEPARATOR_PRINCIPAL_FISIER,
                IdMasina,
                Marca,
                Model,
                NumarInmatriculare,
                AnFabricatie,
                PretPeZi,
                Disponibila);
        }

        public string Info()
        {
            return $"ID: {IdMasina} | {Marca} {Model} | Nr: {NumarInmatriculare} | An: {AnFabricatie} | Pret/zi: {PretPeZi} lei | Disponibila: {Disponibila}";
        }
    }
}