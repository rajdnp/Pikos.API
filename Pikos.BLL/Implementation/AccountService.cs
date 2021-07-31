using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pikos.BLL.Interfaces;
using Pikos.DAL;
using Pikos.DAL.Contracts;
using Pikos.Helpers;
using Pikos.Models.Entities;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using static Pikos.Models.DTOs.SignInDtos;
using static Pikos.Models.DTOs.SignUpDtos;

namespace Pikos.BLL.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository userRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly AppSettings appSettings;

        public AccountService(IUserRepository userRepository, IUnitOfWork unitOfWork,IOptions<AppSettings> appSettings)
        {
            this.userRepository = userRepository;
            this.unitOfWork = unitOfWork;
            this.appSettings = appSettings.Value;
        }
        public async Task<SignInResponse> SignIn(SignInRequest model, string ipAdress)
        {
            var users = await userRepository.GetAll();
            var user = users.SingleOrDefault(x => (x.Email == model.UserName || x.UserName == model.UserName) && (x.Password == model.Password));
            

            if (user == null) return null;


            var jwtToken = GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken(ipAdress);
            user.RefreshTokens.Add(refreshToken);
            await unitOfWork.Complete();

            return new SignInResponse
            {
                UserId = user.Id.ToString(),
                Token = jwtToken,
                UserName = user.UserName ?? user.Email
            };

        }

        public async Task<bool> SignUp(SignUpRequest model)
        {
            var user = new User { Email = model.Email, Name = model.FirstName + model.LastName, Password = model.Password, UserName = model.UserName };
            userRepository.Add(user);
            var result = await unitOfWork.Complete();
            if(result > 0)
            {
                return true;
            }

            return false;
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}
