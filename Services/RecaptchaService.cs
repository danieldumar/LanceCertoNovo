using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LanceCerto.WebApp.Services
{
    /// <summary>
    /// Serviço para validação de tokens reCAPTCHA v2/v3 via API do Google.
    /// </summary>
    public class RecaptchaService
    {
        private const string VerifyUrl = "https://www.google.com/recaptcha/api/siteverify";
        private readonly RecaptchaSettings _settings;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<RecaptchaService> _logger;

        public RecaptchaService(
            IOptions<RecaptchaSettings> settings,
            IHttpClientFactory clientFactory,
            ILogger<RecaptchaService> logger)
        {
            _settings = settings?.Value
                ?? throw new ArgumentNullException(nameof(settings));
            _clientFactory = clientFactory
                ?? throw new ArgumentNullException(nameof(clientFactory));
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Verifica o token do reCAPTCHA junto ao Google.
        /// </summary>
        /// <param name="recaptchaToken">Token obtido no cliente.</param>
        /// <returns>True se a validação for bem-sucedida.</returns>
        public async Task<bool> VerifyAsync(string recaptchaToken)
        {
            if (string.IsNullOrWhiteSpace(recaptchaToken))
            {
                _logger.LogWarning("reCAPTCHA token está vazio ou nulo.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(_settings.SecretKey))
            {
                _logger.LogError("Chave secreta do reCAPTCHA não configurada.");
                return false;
            }

            var parameters = new Dictionary<string, string>
            {
                ["secret"] = _settings.SecretKey,
                ["response"] = recaptchaToken
            };

            try
            {
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));

                using var content = new FormUrlEncodedContent(parameters);
                using var response = await client.PostAsync(VerifyUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning(
                        "Falha na chamada ao reCAPTCHA: {StatusCode}",
                        response.StatusCode);
                    return false;
                }

                await using var stream = await response.Content.ReadAsStreamAsync();
                var result = await JsonSerializer.DeserializeAsync<RecaptchaResponse>(
                    stream,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result == null)
                {
                    _logger.LogError("Resposta do reCAPTCHA desserializada como nula.");
                    return false;
                }

                if (!result.Success)
                {
                    var errors = result.ErrorCodes != null && result.ErrorCodes.Length > 0
                        ? string.Join(", ", result.ErrorCodes)
                        : "nenhum código";
                    _logger.LogWarning(
                        "reCAPTCHA falhou. Erros: {Errors}",
                        errors);
                }

                return result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção ao verificar o reCAPTCHA.");
                return false;
            }
        }

        private class RecaptchaResponse
        {
            public bool Success { get; init; }
            public DateTime? ChallengeTs { get; init; }
            public string? Hostname { get; init; }
            public string[]? ErrorCodes { get; init; }
        }
    }
}