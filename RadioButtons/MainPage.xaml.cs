namespace RadioButtons
{
    public partial class MainPage : ContentPage
    {
        CurrencyConverter currencyConverter;

        public MainPage()
        {
            InitializeComponent();

            currencyConverter = new CurrencyConverter();
        }

        private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton.Content.Equals("Dolar"))
            {
                txValorConvertido.Text = String.Format("{0:C2}", Convert.ToSingle(txValor.Text) / 5.01f);
            }

            if (radioButton.Content.Equals("Euro"))
            {
                txValorConvertido.Text = String.Format("{0:C2}", Convert.ToSingle(txValor.Text) / 5.61f);
            }

            if (radioButton.Content.Equals("Pesos"))
            {
                txValorConvertido.Text = String.Format("{0:C2}", Convert.ToSingle(txValor.Text) * 305.12f);
            }
        }

        private void OnPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;
        }

        private void btConverterPicker_Clicked(object sender, EventArgs e)
        {
            if (!currencyConverter.RatesLoaded)
            {
                lbResultado.Text = "Cotações indisponíveis. Abra o app com internet ao menos uma vez.";
                return;
            }

            string fromCurrency = pickFrom.SelectedItem as string;
            string toCurrency = pickTo.SelectedItem as string;
            
            decimal amount = Convert.ToDecimal(txValor.Text);

            decimal exchangeRate = currencyConverter.ConvertValue(
                amount, fromCurrency, toCurrency);

            lbResultado.Text = String.Format("{0:C2}",exchangeRate);
        }
    }
}
