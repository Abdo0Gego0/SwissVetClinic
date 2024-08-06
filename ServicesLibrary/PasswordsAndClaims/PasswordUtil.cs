using CmsDataAccess;
using CmsDataAccess.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLibrary.PasswordsAndClaims
{
    public class PasswordUtil
    {
        public static IConfiguration _config;
        public static UserManager<IdentityUser> _userManager; // Add UserManager

        public PasswordUtil()
        {

        }
        public PasswordUtil(
            UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string GetRolesAsync(IdentityUser user)
        {
            var roles = _userManager.GetRolesAsync(user).Result;
            return string.Join(",", roles);
        }

        public string CreateTokenForUser(Person pat)
        {
            var roles = GetRolesAsync(pat.User);



            PetOwner centerAdmin = new ApplicationDbContext().PetOwner.Find(pat.Id);

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, pat.User.UserName),
                new Claim(ClaimTypes.SerialNumber, pat.Id.ToString()),
                new Claim(ClaimTypes.Name, pat.FullName ),
                new Claim(ClaimTypes.Email, pat.User.Email),
                new Claim(ClaimTypes.MobilePhone, pat.User.PhoneNumber),
                new Claim(ClaimTypes.Role, roles),
                new Claim("Image", pat.ImageFullPath),
                new Claim("CenterId", centerAdmin.MedicalCenterId.ToString()),

            };




            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                getConfigValue("jwt", "Key")
                ));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                issuer: getConfigValue("jwt", "Issuer"),
                audience: getConfigValue("jwt", "Audience"),
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }


        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedPasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                return computedPasswordHash.SequenceEqual(passwordHash);
            }
        }
        public string? CreateRandomToken(string UserName)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,UserName),
            };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(getConfigValue("AppSettings", "Token")));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                issuer: getConfigValue("jwt", "Issuer"),
                audience: getConfigValue("jwt", "Issuer"),
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds

                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        public string CreateRandomTokenN(int? N=8)
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes((int)N));
        }
        public string getConfigValue(string sec, string subSec)
        {
            var configurationBuilder = new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configurationBuilder.AddJsonFile(path, false);
            var root = configurationBuilder.Build();
            var domain = root.GetSection(sec);
            var StripeApiKey = domain.GetSection(subSec).Value;
            return StripeApiKey;
        }
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
            "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            "abcdefghijkmnopqrstuvwxyz",    // lowercase
            "0123456789",                   // digits
            "!@$?_-"                        // non-alphanumeric
        };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

    }
}
