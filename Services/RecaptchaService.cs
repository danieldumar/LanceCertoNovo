using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace LanceCerto.WebApp.Services
{
    public class RecaptchaService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<RecaptchaService> _logger;

        public RecaptchaService(
            IConfiguration config,
            IHttpClientFactory clientFactory,
            ILogger<RecaptchaService> logger)
        {
            _config = config;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Valida o token do reCAPTCHA v2/v3 com o Google.
        /// </summary>
        public async Task<bool> VerifyAsync(string recaptchaToken)
        {
            if (string.IsNullOrWhiteSpace(recaptchaToken))
            {
                _logger.LogWarning("reCAPTCHA token está vazio.");
                return false;
            }

            var secretKey = _config["GoogleReCaptcha:SecretKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                _logger.LogError("Chave secreta do reCAPTCHA não configurada.");
                return false;
            }

            var parameters = new Dictionary<string, string>
            {
                { "secret", secretKey },
                { "response", recaptchaToken }
            };

            try
            {
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", new FormUrlEncodedContent(parameters));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Falha na chamada ao reCAPTCHA: {StatusCode}", response.StatusCode);
                    return false;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<RecaptchaResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (result == null)
                {
                    _logger.LogError("Não foi possível desserializar a resposta do reCAPTCHA.");
                    return false;
                }

                if (!result.Success)
                {
                    _logger.LogWarning("reCAPTCHA falhou. Erros: {Errors}", string.Join(", ", result.ErrorCodes ?? Array.Empty<string>()));
                }

                return result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar o reCAPTCHA.");
                return false;
            }
        }

        private class RecaptchaResponse
        {
            public bool Success { get; set; }
            public DateTime Challenge_ts { get; set; }
            public string Hostname { get; set; }
            public string[] ErrorCodes { get; set; }
        }
    }
}