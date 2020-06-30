﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using PhotoDuel.Models;
using PhotoDuel.Models.Vk;
using PhotoDuel.Services.Abstract;

namespace PhotoDuel.Services
{
    public class VkService : ISocialService
    {
        private const string VkApiAddress = "https://api.vk.com";
        private const string VkApiVersion = "5.120";
        private readonly string _vkApiKey;

        public VkService()
        {
            var key = Environment.GetEnvironmentVariable("PHOTO_DUEL_VK_TOKEN");
            _vkApiKey = key ?? throw new ArgumentException("No VK Service token!");
        }

        public void Notify(string[] userIds, string message, string hash = "")
        {
            MakeVkRequest("notifications.sendMessage", new Dictionary<string, string>
            {
                {"user_ids", string.Join(",", userIds)},
                {"fragment", hash},
                {"message", message}
            });
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
                Name = vkUser.FullName,
                Photo = vkUser.Photo
            };
        }

        private string MakeVkRequest(string method, Dictionary<string, string> data)
        {
            var address = $"{VkApiAddress}/method/{method}?v={VkApiVersion}&access_token={_vkApiKey}";
            return Utils.MakeRequest(address, data);
        }
    }
}