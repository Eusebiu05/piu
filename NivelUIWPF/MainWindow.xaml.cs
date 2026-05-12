using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using LibrarieModele;
using NivelStocareDate;

namespace NivelUIWPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly IStocareMasini adminMasini;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Masina> masini = new ObservableCollection<Masina>();

        public ObservableCollection<Masina> Masini
        {
            get => masini;
            set
            {
                masini = value;
                OnPropertyChanged();
            }
        }

        private Masina? masinaSelectata;

        public Masina? MasinaSelectata
        {
            get => masinaSelectata;
            set
            {
                masinaSelectata = value;
                OnPropertyChanged();
            }
        }

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

        private readonly List<string> tipuriCarburant = new List<string>
        {
            "Benzină",
            "Diesel",
            "Hibrid",
            "Electric"
        };

        private readonly List<string> dotariDisponibile = new List<string>
        {
            "AC",
            "Navigație",
            "Bluetooth",
            "Cameră",
            "Scaune încălzite"
        };

        public MainWindow()
        {
            InitializeComponent();

            adminMasini = new AdministrareMasiniFisierText("Masini.txt");

            DataContext = this;

            InitializeazaControaleLab09();
            ReseteazaErori();
            IncarcaMasiniInObservableCollection();
        }

        private void InitializeazaControaleLab09()
        {
            lstTipCarburant.ItemsSource = tipuriCarburant;
            lstTipCarburant.SelectedIndex = 1;

            lstDotariModificare.ItemsSource = dotariDisponibile;

            dpDataInmatriculare.SelectedDate = DateTime.Today;
            dpDataInmatriculareModificare.SelectedDate = DateTime.Today;

            ActualizeazaComboMasini();
        }

        private void IncarcaMasiniInObservableCollection()
        {
            Masini.Clear();

            foreach (Masina masina in adminMasini.GetMasini())
            {
                Masini.Add(masina);
            }

            lblTitluLista.Content = "Lista tuturor mașinilor";
            lblStatus.Content = $"Status: {Masini.Count} mașini salvate.";
        }

        private void AfiseazaToateMasinile()
        {
            IncarcaMasiniInObservableCollection();
        }

        private void AfiseazaLista(List<Masina> listaMasini)
        {
            Masini.Clear();

            foreach (Masina masina in listaMasini)
            {
                Masini.Add(masina);
            }

            if (Masini.Count == 0)
            {
                lblStatus.Content = "Status: nu există mașini de afișat.";
            }
        }

        private void BtnMeniuAdmin_Click(object sender, RoutedEventArgs e)
        {
            panelAdministrare.Visibility = Visibility.Visible;
            panelCautare.Visibility = Visibility.Collapsed;
            panelModificare.Visibility = Visibility.Collapsed;

            lblMesajCautare.Content = string.Empty;
            AfiseazaToateMasinile();
        }

        private void BtnMeniuModifica_Click(object sender, RoutedEventArgs e)
        {
            panelAdministrare.Visibility = Visibility.Collapsed;
            panelCautare.Visibility = Visibility.Collapsed;
            panelModificare.Visibility = Visibility.Visible;

            ActualizeazaComboMasini();

            lblTitluLista.Content = "Modificare mașină";
            lblStatus.Content = "Status: alege o mașină din ComboBox pentru modificare.";
        }

        private void BtnMeniuCauta_Click(object sender, RoutedEventArgs e)
        {
            panelAdministrare.Visibility = Visibility.Collapsed;
            panelCautare.Visibility = Visibility.Visible;
            panelModificare.Visibility = Visibility.Collapsed;

            lblTitluLista.Content = "Rezultate căutare";
            lblStatus.Content = "Status: introdu textul de căutare.";
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
                Disponibila = rbDisponibila.IsChecked == true,
                Dotari = GetDotariSelectate(),
                TipCarburant = GetTipCarburantSelectat(),
                DataInmatriculare = dpDataInmatriculare.SelectedDate ?? DateTime.Today,
                DataActualizare = DateTime.Today
            };

            adminMasini.AddMasina(masina);

            CurataCampuri();
            ReseteazaErori();
            IncarcaMasiniInObservableCollection();
            ActualizeazaComboMasini();

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

        private void BtnCauta_Click(object sender, RoutedEventArgs e)
        {
            string textCautat = txtCautare.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(textCautat))
            {
                lblMesajCautare.Content = "Introdu un text pentru căutare.";
                Masini.Clear();
                lblStatus.Content = "Status: căutare invalidă.";
                return;
            }

            List<Masina> rezultate = adminMasini.GetMasini()
                .Where(m =>
                    m.Marca.ToLower().Contains(textCautat) ||
                    m.Model.ToLower().Contains(textCautat) ||
                    m.NumarInmatriculare.ToLower().Contains(textCautat))
                .ToList();

            AfiseazaLista(rezultate);

            lblTitluLista.Content = "Rezultate căutare";

            if (rezultate.Count == 0)
            {
                lblMesajCautare.Content = "Nu a fost găsită nicio mașină.";
                lblStatus.Content = "Status: 0 rezultate.";
            }
            else
            {
                lblMesajCautare.Content = string.Empty;
                lblStatus.Content = $"Status: {rezultate.Count} rezultate găsite.";
            }
        }

        private void BtnResetCautare_Click(object sender, RoutedEventArgs e)
        {
            txtCautare.Clear();
            lblMesajCautare.Content = string.Empty;
            AfiseazaToateMasinile();
        }

        private void ActualizeazaComboMasini()
        {
            List<Masina> masini = adminMasini.GetMasini();

            cmbMasiniModificare.ItemsSource = null;
            cmbMasiniModificare.ItemsSource = masini;
        }

        private void CmbMasiniModificare_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbMasiniModificare.SelectedItem is not Masina masina)
            {
                return;
            }

            MasinaSelectata = masina;

            rbModDisponibila.IsChecked = masina.Disponibila;
            rbModIndisponibila.IsChecked = !masina.Disponibila;

            lstDotariModificare.SelectedItems.Clear();

            foreach (string dotare in dotariDisponibile)
            {
                if (masina.Dotari.Contains(dotare))
                {
                    lstDotariModificare.SelectedItems.Add(dotare);
                }
            }
        }

        private void BtnActualizeaza_Click(object sender, RoutedEventArgs e)
        {
            if (MasinaSelectata == null)
            {
                lblStatus.Content = "Status: selectează o mașină pentru modificare.";
                return;
            }

            MasinaSelectata.Disponibila = rbModDisponibila.IsChecked == true;
            MasinaSelectata.DataActualizare = DateTime.Today;

            MasinaSelectata.Dotari = lstDotariModificare.SelectedItems
                .Cast<string>()
                .ToList();

            bool modificat = adminMasini.ModificaMasina(MasinaSelectata);

            if (modificat)
            {
                IncarcaMasiniInObservableCollection();
                ActualizeazaComboMasini();
                lblStatus.Content = "Status: mașina a fost actualizată cu succes.";
            }
            else
            {
                lblStatus.Content = "Status: mașina nu a fost găsită în fișier.";
            }
        }

        private List<string> GetDotariSelectate()
        {
            List<string> dotari = new List<string>();

            if (ckbAC.IsChecked == true)
            {
                dotari.Add("AC");
            }

            if (ckbNavigatie.IsChecked == true)
            {
                dotari.Add("Navigație");
            }

            if (ckbBluetooth.IsChecked == true)
            {
                dotari.Add("Bluetooth");
            }

            if (ckbCamera.IsChecked == true)
            {
                dotari.Add("Cameră");
            }

            if (ckbIncalzire.IsChecked == true)
            {
                dotari.Add("Scaune încălzite");
            }

            return dotari;
        }

        private string GetTipCarburantSelectat()
        {
            if (lstTipCarburant.SelectedItem is string tip)
            {
                return tip;
            }

            return "Diesel";
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

            if (!decimal.TryParse(pretText, out decimal pretPeZi) || pretPeZi <= 0)
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

        private void CurataCampuri()
        {
            txtMarca.Clear();
            txtModel.Clear();
            txtNumar.Clear();
            txtAn.Clear();
            txtPret.Clear();

            rbDisponibila.IsChecked = true;
            rbIndisponibila.IsChecked = false;

            ckbAC.IsChecked = false;
            ckbNavigatie.IsChecked = false;
            ckbBluetooth.IsChecked = false;
            ckbCamera.IsChecked = false;
            ckbIncalzire.IsChecked = false;

            lstTipCarburant.SelectedIndex = 1;
            dpDataInmatriculare.SelectedDate = DateTime.Today;
        }
    }
}