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
            _httpClient.Timeout = TimeSpan.FromSeconds(10);

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
            try
            {
                // ONLINE: busca as cotações na API e salva uma cópia no dispositivo
                ratesUSD = await GetRatesAsync("USD");
                ratesBRL = await GetRatesAsync("BRL");
                ratesEUR = await GetRatesAsync("EUR");

                SaveRates(ratesUSD);
                SaveRates(ratesBRL);
                SaveRates(ratesEUR);
            }
            catch (Exception)
            {
                // OFFLINE: usa as cotações salvas na última vez em que o app esteve online
                ratesUSD = ReadRates("USD");
                ratesBRL = ReadRates("BRL");
                ratesEUR = ReadRates("EUR");
            }
        }

        public decimal ConvertValue(decimal amount, string from, string to)
        {
            if (from == "USD" && to == "BRL") 
                return amount * ratesUSD.rates["BRL"];
            else if (from == "USD" && to == "EUR") 
                return amount * ratesUSD.rates["EUR"];
            else if (from == "BRL" && to == "EUR")
                return amount * ratesBRL.rates["EUR"];
            else if (from == "BRL" && to == "USD")
                return amount * ratesBRL.rates["USD"];
            else if (from == "EUR" && to == "USD")
                return amount * ratesEUR.rates["USD"];
            else if (from == "EUR" && to == "BRL")
                return amount * ratesEUR.rates["BRL"];
            else 
                return amount;

        }

        // Indica se as cotações já foram carregadas (da internet ou dos arquivos)
        public bool RatesLoaded => ratesUSD != null && ratesBRL != null && ratesEUR != null;

        // Caminho do arquivo JSON de uma moeda, na pasta de dados do app
        private string GetFilePath(string baseCurrency)
        {
            return Path.Combine(FileSystem.AppDataDirectory, $"rates_{baseCurrency}.json");
        }

        // Salva todos os dados de uma cotação em um arquivo no dispositivo, como string JSON
        public void SaveRates(CurrencyRates rates)
        {
            string jsonString = JsonSerializer.Serialize(rates);

            File.WriteAllText(GetFilePath(rates.base_code), jsonString);
        }

        // Lê todos os dados de uma cotação do arquivo JSON salvo no dispositivo
        // Retorna null se ainda não existir arquivo salvo para a moeda
        public CurrencyRates ReadRates(string baseCurrency)
        {
            string filePath = GetFilePath(baseCurrency);

            if (!File.Exists(filePath))
                return null;

            string jsonString = File.ReadAllText(filePath);

            return JsonSerializer.Deserialize<CurrencyRates>(jsonString);
        }
    }
}
