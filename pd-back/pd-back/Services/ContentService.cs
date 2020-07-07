using System;
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

        private readonly Random _random = new Random();
        
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
            
            foreach (var category in categories)
            {
                foreach (var challenge in category.Challenges)
                {
                    if (GetCategoryByChallenge(challenge.Id) != category.Id)
                    {
                        throw new ArgumentException($"Non valid identifier, cat: {category.Id}; challenge: {challenge.Id}");
                    }
                }
            }
        }

        public Category[] GetCategories()
        {
            return _categories;
        }

        public int[] GetRandomCategoryIds(int count)
        {
            return _categories.Select(x => x.Id).Shuffle().Take(count).ToArray();
        }

        public int GetCategoryByChallenge(int challengeId)
        {
            return (int) Math.Floor((double)challengeId / 1000);
        }

        public int GetRandomChallenge(int categoryId)
        {
            var category = _categories.FirstOrDefault(c => c.Id == categoryId);
            if (category == null) throw new InvalidOperationException($"No category with id: {categoryId}");

            return category.Challenges[_random.Next(category.Challenges.Length)].Id;
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