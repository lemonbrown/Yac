using Microsoft.Playwright;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataScraper.Helpers {
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpHelper {
        private readonly HttpClient _httpClient;
        private readonly SemaphoreSlim _throttleSemaphore = new(1, 1);
        private readonly int _throttleDelayMs;
        private readonly string _cacheDir;

        public HttpHelper(int throttleMilliseconds = 6000, string cacheDirectory = "cache") {
            _httpClient = new HttpClient();
            _throttleDelayMs = throttleMilliseconds;
            _cacheDir = cacheDirectory;

            if (!Directory.Exists(_cacheDir))
                Directory.CreateDirectory(_cacheDir);
        }

        public async Task<string> GetHtmlAsync(string url) {
            var cachePath = GetCacheFilePath(url);

            // Load from disk if it exists
            if (File.Exists(cachePath)) {
                Console.WriteLine($"🧠 Loaded from cache: {url}");
                return await File.ReadAllTextAsync(cachePath);
            }

            // Throttle
            await _throttleSemaphore.WaitAsync();
            try {
                await Task.Delay(_throttleDelayMs);

                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) {
                    Console.WriteLine($"⚠️ Failed: {response.StatusCode} - {url}");
                    return null;
                }

                var html = await response.Content.ReadAsStringAsync();

                // Save to disk
                await File.WriteAllTextAsync(cachePath, html);
                Console.WriteLine($"💾 Cached: {url}");

                return html;
            } finally {
                _throttleSemaphore.Release();
            }
        }

        private string GetCacheFilePath(string url) {
            // Hash URL to generate a unique file name
            using var sha = SHA256.Create();
            var hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(url)));
            return Path.Combine(_cacheDir, $"{hash}.html");
        }
    }

}
