using System.Collections.Generic;
using PhotoDuel.Models;

namespace PhotoDuel.Services.Abstract
{
    public interface ISocialService
    {
        void Notify(string[] userIds, string message, string hash = "");

        UserMeta GetUser(string userId);

        bool IsSignValid(string userId, Dictionary<string, string> pars, string sign);
    }
}