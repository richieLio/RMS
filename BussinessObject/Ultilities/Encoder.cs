using DataAccess.Entities;
using Data.Models.EncodeModel;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Business.Ultilities
{
    public class Encoder
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private static string Key = "ThisIsASecretKey123!@#ABCDE12321";
        private static string Issuser = "MyAwesomeApp";



        public Encoder()
        {

            _tokenHandler = new JwtSecurityTokenHandler();

        }

        static string GenerateSalt()
        {
            int SaltLength = 16;

            byte[] Salt = new byte[SaltLength];

            using (var Rng = new RNGCryptoServiceProvider())
            {
                Rng.GetBytes(Salt);
            }

            return BitConverter.ToString(Salt).Replace("-", "");
        }

        public static CreateHashPasswordModel CreateHashPassword(string Password)
        {
            string SaltString = GenerateSalt();
            byte[] Salt = Encoding.UTF8.GetBytes(SaltString);
            byte[] PasswordByte = Encoding.UTF8.GetBytes(Password);
            byte[] CombinedBytes = CombineBytes(PasswordByte, Salt);
            byte[] HashedPassword = HashingPassword(CombinedBytes);
            return new CreateHashPasswordModel()
            {
                Salt = Encoding.UTF8.GetBytes(SaltString),
                HashedPassword = HashedPassword
            };
        }
        public static CreateHashPasswordModel CreateHash2ndPassword(string Password)
        {
            byte[] PasswordByte = Encoding.UTF8.GetBytes(Password);
            byte[] HashedPassword = HashingPassword(PasswordByte);
            return new CreateHashPasswordModel()
            {
                Salt = null, // No salt is used
                HashedPassword = HashedPassword
            };
        }


        public static bool VerifyPasswordHashed(string Password, byte[] Salt, byte[] PasswordStored)
        {
            byte[] PasswordByte = Encoding.UTF8.GetBytes(Password);
            byte[] CombinedBytes = CombineBytes(PasswordByte, Salt);
            byte[] NewHash = HashingPassword(CombinedBytes);
            return PasswordStored.SequenceEqual(NewHash);
        }

        public static bool Verify2ndPasswordHashed(string Password, byte[] PasswordStored)
        {
            byte[] PasswordByte = Encoding.UTF8.GetBytes(Password);
            byte[] NewHash = HashingPassword(PasswordByte);
            return PasswordStored.SequenceEqual(NewHash);
        }

        static byte[] HashingPassword(byte[] PasswordCombined)
        {
            using (SHA256 SHA256 = SHA256.Create())
            {
                byte[] HashBytes = SHA256.ComputeHash(PasswordCombined);
                return HashBytes;
            }
        }

        static byte[] CombineBytes(byte[] First, byte[] Second)
        {
            byte[] Combined = new byte[First.Length + Second.Length];
            Buffer.BlockCopy(First, 0, Combined, 0, First.Length);
            Buffer.BlockCopy(Second, 0, Combined, First.Length, Second.Length);
            return Combined;
        }

        public static string GenerateJWT(User User)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var Credential = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> Claims = new()
            {
                new Claim(ClaimsIdentity.DefaultRoleClaimType, User.Role),
                new Claim("userid", User.Id.ToString()),
                new Claim("email", User.Email),
            };

            var Token = new JwtSecurityToken(
                issuer: Issuser,
                audience: Issuser,
                claims: Claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: Credential
                );
            var Encodetoken = new JwtSecurityTokenHandler().WriteToken(Token);
            return Encodetoken;
        }
        public static string GenerateHouseJWT(House house)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var Credential = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> Claims = new()
            {
                new Claim("houseid", house.Id.ToString()),
                new Claim("houseaccount", house.HouseAccount),
            };

            var Token = new JwtSecurityToken(
                issuer: Issuser,
                audience: Issuser,
                claims: Claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: Credential
                );
            var Encodetoken = new JwtSecurityTokenHandler().WriteToken(Token);
            return Encodetoken;
        }
        public static string GenerateRandomPassword()
        {
            int length = 12;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            byte[] data = new byte[length];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                byte[] buffer = new byte[sizeof(int)];
                System.Text.StringBuilder result = new System.Text.StringBuilder(length);
                for (int i = 0; i < length; i++)
                {
                    crypto.GetBytes(buffer);
                    int randomNumber = BitConverter.ToInt32(buffer, 0);
                    randomNumber = Math.Abs(randomNumber);
                    int index = randomNumber % chars.Length;
                    result.Append(chars[index]);
                }
                return result.ToString();
            }
        }
        public string DecodeToken(string jwtToken, string nameClaim)
        {
            Claim? claim = _tokenHandler.ReadJwtToken(jwtToken).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
            return claim != null ? claim.Value : "Error!!!";
        }
    }
}
