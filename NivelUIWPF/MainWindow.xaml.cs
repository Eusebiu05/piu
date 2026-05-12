using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibrarieModele;
using NivelStocareDate;

namespace NivelUIWPF
{
    public partial class MainWindow : Window
    {
        private readonly IStocareMasini adminMasini;

        private const int DATE_VALIDE = 0;

        private const int EROARE_MARCA_GOALA = 1;
        private const int EROARE_MARCA_LUNGA = 2;

        private const int EROARE_MODEL_GOL = 4;
        private const int EROARE_MODEL_LUNG = 8;

        private const int EROARE_NUMAR_GOL = 16;
        private const int EROARE_NUMAR_LUNG = 32;

        private const int EROARE_AN_INVALID = 64;
        private const int EROARE_PRET_INVALID = 128;

        private const int LUNGIME_MAXIMA_TEXT = 15;
        private const int AN_MINIM = 1950;

        private readonly Brush culoareNormalaLabel = Brushes.WhiteSmoke;
        private readonly Brush culoareEroare = Brushes.OrangeRed;
        private readonly Brush culoareBorderNormala = new SolidColorBrush(Color.FromRgb(102, 102, 138));

        public MainWindow()
        {
            InitializeComponent();

            adminMasini = new AdministrareMasiniFisierText("Masini.txt");

            ReseteazaErori();
            AfiseazaToateMasinile();
        }

        private void BtnAdauga_Click(object sender, RoutedEventArgs e)
        {
            int codEroare = ValideazaDateMasina();

            AfiseazaErori(codEroare);

            if (codEroare != DATE_VALIDE)
            {
                lblStatus.Content = "Status: există date invalide. Corectează câmpurile marcate.";
                return;
            }

            int.TryParse(txtAn.Text.Trim(), out int anFabricatie);
            decimal.TryParse(txtPret.Text.Trim(), out decimal pretPeZi);

            Masina masina = new Masina
            {
                Marca = txtMarca.Text.Trim(),
                Model = txtModel.Text.Trim(),
                NumarInmatriculare = txtNumar.Text.Trim(),
                AnFabricatie = anFabricatie,
                PretPeZi = pretPeZi,
                Disponibila = true
            };

            adminMasini.AddMasina(masina);

            CurataCampuri();
            ReseteazaErori();
            AfiseazaToateMasinile();

            lblStatus.Content = "Status: mașina a fost adăugată cu succes.";
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            CurataCampuri();
            ReseteazaErori();

            lblStatus.Content = "Status: câmpurile au fost resetate.";
        }

        private void BtnAfiseazaToate_Click(object sender, RoutedEventArgs e)
        {
            AfiseazaToateMasinile();
        }

        private void BtnAfiseazaDisponibile_Click(object sender, RoutedEventArgs e)
        {
            List<Masina> masiniDisponibile = adminMasini.GetMasini()
                .Where(m => m.Disponibila)
                .ToList();

            AfiseazaLista(masiniDisponibile);

            lblTitluLista.Content = "Mașini disponibile";
            lblStatus.Content = $"Status: {masiniDisponibile.Count} mașini disponibile.";
        }

        private int ValideazaDateMasina()
        {
            int codEroare = DATE_VALIDE;

            string marca = txtMarca.Text.Trim();
            string model = txtModel.Text.Trim();
            string numar = txtNumar.Text.Trim();
            string anText = txtAn.Text.Trim();
            string pretText = txtPret.Text.Trim();

            if (string.IsNullOrWhiteSpace(marca))
            {
                codEroare |= EROARE_MARCA_GOALA;
            }
            else if (marca.Length > LUNGIME_MAXIMA_TEXT)
            {
                codEroare |= EROARE_MARCA_LUNGA;
            }

            if (string.IsNullOrWhiteSpace(model))
            {
                codEroare |= EROARE_MODEL_GOL;
            }
            else if (model.Length > LUNGIME_MAXIMA_TEXT)
            {
                codEroare |= EROARE_MODEL_LUNG;
            }

            if (string.IsNullOrWhiteSpace(numar))
            {
                codEroare |= EROARE_NUMAR_GOL;
            }
            else if (numar.Length > LUNGIME_MAXIMA_TEXT)
            {
                codEroare |= EROARE_NUMAR_LUNG;
            }

            if (!int.TryParse(anText, out int anFabricatie) ||
                anFabricatie < AN_MINIM ||
                anFabricatie > DateTime.Now.Year + 1)
            {
                codEroare |= EROARE_AN_INVALID;
            }

            if (!decimal.TryParse(pretText, out decimal pretPeZi) ||
                pretPeZi <= 0)
            {
                codEroare |= EROARE_PRET_INVALID;
            }

            return codEroare;
        }

        private void AfiseazaErori(int codEroare)
        {
            ReseteazaErori();

            if (AreEroare(codEroare, EROARE_MARCA_GOALA))
            {
                MarcheazaEroare(lblMarcaInput, txtMarca, tbErrMarca, "Marca este obligatorie.");
            }
            else if (AreEroare(codEroare, EROARE_MARCA_LUNGA))
            {
                MarcheazaEroare(lblMarcaInput, txtMarca, tbErrMarca,
                    $"Marca nu poate avea mai mult de {LUNGIME_MAXIMA_TEXT} caractere.");
            }

            if (AreEroare(codEroare, EROARE_MODEL_GOL))
            {
                MarcheazaEroare(lblModelInput, txtModel, tbErrModel, "Modelul este obligatoriu.");
            }
            else if (AreEroare(codEroare, EROARE_MODEL_LUNG))
            {
                MarcheazaEroare(lblModelInput, txtModel, tbErrModel,
                    $"Modelul nu poate avea mai mult de {LUNGIME_MAXIMA_TEXT} caractere.");
            }

            if (AreEroare(codEroare, EROARE_NUMAR_GOL))
            {
                MarcheazaEroare(lblNumarInput, txtNumar, tbErrNumar,
                    "Numărul de înmatriculare este obligatoriu.");
            }
            else if (AreEroare(codEroare, EROARE_NUMAR_LUNG))
            {
                MarcheazaEroare(lblNumarInput, txtNumar, tbErrNumar,
                    $"Numărul nu poate avea mai mult de {LUNGIME_MAXIMA_TEXT} caractere.");
            }

            if (AreEroare(codEroare, EROARE_AN_INVALID))
            {
                MarcheazaEroare(lblAnInput, txtAn, tbErrAn,
                    $"Anul trebuie să fie între {AN_MINIM} și {DateTime.Now.Year + 1}.");
            }

            if (AreEroare(codEroare, EROARE_PRET_INVALID))
            {
                MarcheazaEroare(lblPretInput, txtPret, tbErrPret,
                    "Prețul trebuie să fie un număr mai mare decât 0.");
            }
        }

        private bool AreEroare(int codEroare, int eroare)
        {
            return (codEroare & eroare) != 0;
        }

        private void MarcheazaEroare(Label label, TextBox textBox, TextBlock textBlock, string mesaj)
        {
            label.Foreground = culoareEroare;
            textBox.BorderBrush = culoareEroare;
            textBox.BorderThickness = new Thickness(2);

            textBlock.Text = mesaj;
            textBlock.Visibility = Visibility.Visible;
        }

        private void ReseteazaErori()
        {
            lblMarcaInput.Foreground = culoareNormalaLabel;
            lblModelInput.Foreground = culoareNormalaLabel;
            lblNumarInput.Foreground = culoareNormalaLabel;
            lblAnInput.Foreground = culoareNormalaLabel;
            lblPretInput.Foreground = culoareNormalaLabel;

            ReseteazaTextBox(txtMarca);
            ReseteazaTextBox(txtModel);
            ReseteazaTextBox(txtNumar);
            ReseteazaTextBox(txtAn);
            ReseteazaTextBox(txtPret);

            AscundeEroare(tbErrMarca);
            AscundeEroare(tbErrModel);
            AscundeEroare(tbErrNumar);
            AscundeEroare(tbErrAn);
            AscundeEroare(tbErrPret);
        }

        private void ReseteazaTextBox(TextBox textBox)
        {
            textBox.BorderBrush = culoareBorderNormala;
            textBox.BorderThickness = new Thickness(1);
        }

        private void AscundeEroare(TextBlock textBlock)
        {
            textBlock.Text = string.Empty;
            textBlock.Visibility = Visibility.Collapsed;
        }

        private void AfiseazaToateMasinile()
        {
            List<Masina> masini = adminMasini.GetMasini();

            AfiseazaLista(masini);

            lblTitluLista.Content = "Lista tuturor mașinilor";
            lblStatus.Content = $"Status: {masini.Count} mașini salvate.";
        }

        private void AfiseazaLista(List<Masina> masini)
        {
            dgMasini.ItemsSource = null;

            if (masini.Count == 0)
            {
                lblStatus.Content = "Status: nu există mașini de afișat.";
                return;
            }

            dgMasini.ItemsSource = masini;
        }

        private void CurataCampuri()
        {
            txtMarca.Clear();
            txtModel.Clear();
            txtNumar.Clear();
            txtAn.Clear();
            txtPret.Clear();
        }
    }
}