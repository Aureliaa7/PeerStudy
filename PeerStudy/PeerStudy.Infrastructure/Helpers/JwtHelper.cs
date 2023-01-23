using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace PeerStudy.Infrastructure.Helpers
{
    public static class JwtHelper
    {
        public static IList<Claim> GetClaimsFromJWT(string jwt)
        {
            var claims = new List<Claim>();

            if (string.IsNullOrEmpty(jwt))
            {
                return claims;
            }

            var payload = jwt.Split('.')[1];  // take the payload from token
            /*Note. If I don't remove the padding, an error occurs*/
            var payloadWithoutPadding = GetBase64WithoutPadding(payload);
            var payloadBytes = Convert.FromBase64String(payloadWithoutPadding);

            var claimPairs = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadBytes);
            claims.AddRange(claimPairs.Select(x => new Claim(x.Key, x.Value.ToString())));

            return claims;
        }

        private static string GetBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }
            return base64;
        }

        public static string GetClaimValueByName(string jwt, string claimType)
        {
            var claims = GetClaimsFromJWT(jwt);
            var searchedClaim = claims.FirstOrDefault(x => x.Type == claimType);

            return searchedClaim?.Value;
        }

        public static bool IsValidJwt(string jwt)
        {
            var expValue = GetClaimValueByName(jwt, "exp");

            if (long.TryParse(expValue, out long unixTimeSeconds))
            {
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
                var expirationDate = dateTimeOffset.ToUniversalTime();

                return expirationDate > DateTime.UtcNow;
            }

            return false;
        }
    }
}
