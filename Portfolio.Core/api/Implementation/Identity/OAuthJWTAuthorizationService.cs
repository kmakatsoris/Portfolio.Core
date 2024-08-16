using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Core.Interfaces.Context.Users;
using Portfolio.Core.Interfaces.Identity;
using Portfolio.Core.Types.Context;
using Portfolio.Core.Types.Models.Users;
using ILogger = NLog.ILogger;

namespace Portfolio.Core.Implementation.Identity
{
    public class OAuthJWTAuthorizationService : IOAuthAuthorizationService
    {
        // Declare Class Variables,
        private readonly IOptions<AppSettings> _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;


        #region Constructors
        public OAuthJWTAuthorizationService
        (
            IOptions<AppSettings> appSettings,
            IUserRepository userRepository,
            ILogger logger
        )
        {
            _logger = logger;
            _appSettings = appSettings;
            _userRepository = userRepository;
        }
        #endregion

        #region Public Methods
        public async Task<string> GenerateJwtToken(User user)
        {
            if (user == null || string.IsNullOrEmpty(user?.Email)) return "";
            if (string.IsNullOrEmpty(user?.Role))
            {
                IEnumerable<User> userRoles = await _userRepository?.GetUserByEmailAsync(user?.Email, ["Id", "Password", "ConfirmPassword", "Token", "Salt"]);
                user.Role = userRoles.Select(ur => ur.Role)?.FirstOrDefault();
            }
            return await GenerateJwtTokenByRoles(user?.Email, [user?.Role]);
        }

        public async Task<bool> ValidateUserJwtToken(string email, string token)
        {
            try
            {
                IEnumerable<User> users = await _userRepository?.GetUserByEmailAsync(email, ["Id", "Password", "ConfirmPassword", "Salt"]);
                if (users == null || users.Count() != 1 || users.Any(u => u == null)) return false;
                User user = users.SingleOrDefault();

                if (user?.Token?.ToLower()?.Equals(token?.ToLower()) != true) return false;

                JwtSecurityToken jwtClaims = ValidateJwtToken(token);
                string userEmailClaim = jwtClaims.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

                return userEmailClaim?.ToLower().Equals(email?.ToLower()) == true;
            }
            catch (Exception ex)
            {
                // Token validation failed
                _logger.Error(ex, $"Token validation failed:\n\n{ex?.Message}. @ValidateUserJwtToken");
                return false;
            }
        }

        public string GetEmailFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            var emailClaim = decodedToken.Claims.FirstOrDefault(claim => claim.Type == "email");

            return emailClaim?.Value;
        }
        #endregion

        #region Private Methods

        private async Task<string> GenerateJwtTokenByRoles(string email, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings?.Value?.JwtConfig?.IssuerSigningKey ?? "");

            // Add Basic Information For Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
                // new Claim(ClaimTypes.Email, user.Email)
                // new Claim("username", "admin"));
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(double.TryParse(_appSettings?.Value?.JwtConfig?.JwtTime, out var result) ? result : 0),
                Issuer = _appSettings?.Value?.JwtConfig?.ValidIssuer ?? "",
                Audience = _appSettings?.Value?.JwtConfig?.ValidAudience ?? "",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenValue = tokenHandler.WriteToken(token);

            if (!await _userRepository?.UpdateUserTokenAsync(new User { Email = email, Token = tokenValue })) throw new Exception("Error while updating the token.");

            return tokenValue;
        }

        private JwtSecurityToken ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings?.Value?.JwtConfig?.IssuerSigningKey ?? "");

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _appSettings?.Value?.JwtConfig?.ValidIssuer ?? "",
                    ValidAudience = _appSettings?.Value?.JwtConfig?.ValidAudience ?? "",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                // Extract the claims from the validated token
                var jwtToken = (JwtSecurityToken)validatedToken;
                // var userEmailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                // Compare the email claim from the token with the email of the user
                // return userEmailClaim == user.Email;
                return jwtToken;
            }
            catch (Exception ex)
            {
                // Token validation failed
                _logger.Error(ex, $"Token validation failed:\n\n{ex?.Message}. @ValidateJwtToken");
                return null;
            }
        }
        #endregion
    }
}