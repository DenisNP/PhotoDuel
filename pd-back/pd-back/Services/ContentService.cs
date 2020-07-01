﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhotoDuel.Models;

namespace PhotoDuel.Services
{
    public class ContentService
    {
        private readonly ILogger<ContentService> _logger;
        private Category[] _categories;
        private Dictionary<int, string> _challengeNames;

        public ContentService(ILogger<ContentService> logger)
        {
            _logger = logger;
        }
        
        public void Init()
        {
            // read categories file
            var categoriesFile = File.ReadAllText("categories.json");
            var categories = JsonConvert.DeserializeObject<Category[]>(categoriesFile);
            _categories = categories;

            // count challenges
            var challengesCount = categories.Sum(x => x.Challenges.Length);
            _logger.LogInformation(
                $"Categories loaded: {categories.Length}, total challenges: {challengesCount}"
            );

            _challengeNames = _categories.SelectMany(c => c.Challenges).ToDictionary(c => c.Id, c => c.Name);

            // check if valid
            if (_challengeNames.Keys.Count != challengesCount)
            {
                throw new ArgumentException("Non unique challenge identifiers");
            }

            if (_categories.Select(c => c.Id).ToHashSet().Count != _categories.Length)
            {
                throw new ArgumentException("Non unique category identifiers");
            }
        }

        public Category[] GetCategories()
        {
            return _categories;
        }

        public bool HasChallengeId(int challengeId)
        {
            return _challengeNames.ContainsKey(challengeId);
        }

        public string GetChallengeName(int challengeId)
        {
            return _challengeNames.ContainsKey(challengeId) ? _challengeNames[challengeId] : "";
        }
    }
}