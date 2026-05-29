using UnityEngine;

namespace LizardCards.Network
{
    public static class AccessTokenController
    {
        private static string GetAccessToken()
        {
            if (PlayerPrefs.HasKey("access_token")) return PlayerPrefs.GetString("access_token");
            var newToken = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString("access_token", newToken);
            PlayerPrefs.Save();
            return newToken;
        }

        public static string AccessToken => GetAccessToken();
    }
}