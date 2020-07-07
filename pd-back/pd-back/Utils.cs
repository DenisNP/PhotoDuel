using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PhotoDuel
{
    public static class Utils
    {
        public static readonly JsonSerializerSettings ConverterSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };
        
        public static long Now()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public static long ToUnixTimeMs(this DateTime dateTime)
        {
            var span = dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (long)span.TotalMilliseconds;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)  
        {
            var rng = new Random();
            var buffer = list.ToList();
            for (var i = 0; i < buffer.Count; i++)
            {
                var j = rng.Next(i, buffer.Count);
                yield return buffer[j];
                buffer[j] = buffer[i];
            }
        }
        
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        public static string MakeRequest(string url, Dictionary<string, string> data)
        {
            var kvList = new List<KeyValuePair<string, string>>();
            foreach (var (key, value) in data)
            {
                kvList.Add(new KeyValuePair<string, string>(key, value));
            }
            var formContent = new FormUrlEncodedContent(kvList);
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU");
            var response = client.PostAsync(url, formContent).Result;
            var bytes = response.Content.ReadAsByteArrayAsync().Result;

            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }
        
        public static byte[] HashHMAC(string key, string message)
        {
            var encoding = new UTF8Encoding();
            var hash = new HMACSHA256(encoding.GetBytes(key));
            var result = hash.ComputeHash(encoding.GetBytes(message));
            return result;
        }
        
        public static string ToBase64(byte[] hash)
        {
            return Convert.ToBase64String(hash)
                .TrimEnd(new []{'='})
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static IEnumerable<T> Shift<T>(this List<T> list, int count)
        {
            var items = list.Take(count);
            list.RemoveRange(0, Math.Min(count, list.Count));
            return items;
        }
    }
}