﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoDuel.Models;
using PhotoDuel.Models.Vk;
using PhotoDuel.Services.Abstract;

namespace PhotoDuel.Services
{
    public class VkService : ISocialService
    {
        private readonly ILogger<VkService> _logger;
        private const string VkApiAddress = "https://api.vk.com";
        private const string VkApiVersion = "5.120";
        private readonly string _vkApiKey;
        private readonly string _vkApiSecret;
        private readonly Regex _imageUrlRegex = new Regex(@"^https:\/\/sun\d{1,3}\-\d{1,3}\.userapi\.com\/.{10,100}\.jpg");

        public VkService(ILogger<VkService> logger)
        {
            _logger = logger;
            var key = Environment.GetEnvironmentVariable("PHOTO_DUEL_VK_TOKEN");
            _vkApiKey = key ?? throw new ArgumentException("No VK Service token!");

            var secret = Environment.GetEnvironmentVariable("PHOTO_DUEL_VK_SECRET");
            _vkApiSecret = secret ?? throw new ArgumentException("No VK Secret key!");
        }

        public void Notify(string[] allUserIds, string message, string hash = "")
        {
            var uids = allUserIds.ToList();
            while (uids.Count > 0)
            {
                var userIds = uids.Shift(100);
                
                MakeVkRequest("notifications.sendMessage", new Dictionary<string, string>
                {
                    {"user_ids", string.Join(",", userIds)},
                    {"fragment", hash},
                    {"message", message}
                });
            }
        }

        public UserMeta GetUser(string userId)
        {
            var userData = MakeVkRequest("users.get", new Dictionary<string, string>
            {
                {"user_ids", userId},
                {"fields", "photo_200"}
            });

            var response = JsonConvert.DeserializeObject<VkUsersResponse>(userData);
            var vkUser = response.Response.First();

            return new UserMeta
            {
                Id = vkUser.Id,
                Name = vkUser.OnlyName,
                Photo = vkUser.Photo
            };
        }

        public bool IsSignValid(string userId, Dictionary<string, string> pars, string sign)
        {
#if DEBUG
            return true;
#endif            
            var parsString = string.Join(
                "&",
                pars.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}").OrderBy(x => x)
            );

            var calculatedSign = Utils.ToBase64(Utils.HashHMAC(_vkApiSecret, parsString));
            return calculatedSign == sign && pars.ContainsKey("vk_user_id") && pars["vk_user_id"] == userId;
        }

        public bool CheckImageUrl(string url)
        {
            return _imageUrlRegex.IsMatch(url);
        }

        private string MakeVkRequest(string method, Dictionary<string, string> data)
        {
            var address = $"{VkApiAddress}/method/{method}?v={VkApiVersion}&access_token={_vkApiKey}";
            return Utils.MakeRequest(address, data, _logger);
        }
    }
}