using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace RadioButtons
{
    public class CurrencyConverter
    {
        private readonly HttpClient _httpClient;

        public CurrencyRates ratesUSD;
        public string USDtoEUR;
        public string USDtoBRL;


        public CurrencyRates ratesBRL;
        public string BRLtoUSD;
        public string BRLtoEUR;


        public CurrencyRates ratesEUR;
        public string EURtoBRL;
        public string EURtoUSD;


        public CurrencyConverter()
        {
            _httpClient = new HttpClient();

            GetAllCurrencyRates();
        }

        public async Task<CurrencyRates> GetRatesAsync(string baseCurrency)
        {
            try
            {
                var url = $"https://open.er-api.com/v6/latest/{baseCurrency}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonString = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<CurrencyRates>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha ao tentar desserializar!");
            }
        }

        private async void GetAllCurrencyRates()
        {
            ratesUSD = await GetRatesAsync("USD");
            ratesBRL = await GetRatesAsync("BRL");
            ratesEUR = await GetRatesAsync("EUR");
        }

        public decimal ConvertValue(decimal amount, string from, string to)
        {
            if (from == "USD" && to == "BRL") 
                return amount * ratesUSD.rates["BRL"];
            else if (from == "USD" && to == "EUR") 
                return amount * ratesUSD.rates["EUR"];
            else if (from == "BRL" && to == "EUR")
                return amount * ratesUSD.rates["EUR"];
            else if (from == "BRL" && to == "USD")
                return amount * ratesUSD.rates["EUR"];
            else if (from == "EUR" && to == "USD")
                return amount * ratesUSD.rates["EUR"];
            else if (from == "EUR" && to == "BRL")
                return amount * ratesUSD.rates["EUR"];
            else 
                return amount;

        }


        // Criar um método para salvar e outro para ler
        // todos os dados das cotações em  
        // arquivos no dispositivo no formato da string JSON


    }
}
