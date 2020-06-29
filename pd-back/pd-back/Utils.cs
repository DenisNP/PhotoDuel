using System;
using System.Linq;

namespace PhotoDuel
{
    public static class Utils
    {
        public static long Now()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
        
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}