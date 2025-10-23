using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PWAs.Helper;
using PWAs.Models.Usuarios;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace PWAs.Services
{
    public class MailService
    {
        private readonly IConfiguration _configuration;
        public MailService()
        {

        }

        public static bool EnviarMail(string email, string asunto, string cuerpoMail)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            bool bBandera;

            try
            {
                mail.From = new MailAddress("norelymovien@gmail.com", "PWAs - Control de Acceso");
                mail.To.Add(email);
                mail.Subject = asunto;
                mail.Body = cuerpoMail;
                mail.IsBodyHtml = true;

                smtp.Credentials = new NetworkCredential("noreplymovien@gmail.com", "eqpc luwk nksk fszz");
                smtp.EnableSsl = true;

                smtp.Send(mail);

                bBandera = true;
            }
            catch
            {
                bBandera = false;
            }
            return bBandera;
        }

        public string GTokenRec()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var token = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return token;
        }

        public bool IsValidMail(string email)
        {
            if (string.IsNullOrEmpty(email)) return false;

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            return Regex.IsMatch(email, pattern);
        }

        public string GTokenOff(string correo)
        {
            var secretKey = "419ad46cd4834e4b740c87d35eac11be5c9dbe3c";
            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, correo));
<<<<<<< HEAD
            claims.AddClaim(new Claim(ClaimTypes.Role, "Lector"));
=======
>>>>>>> edbb295 (Subiendo mi proyecto a Miguel)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddHours(0.5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenT = tokenHandler.CreateToken(tokenDescriptor);

            string bearer_token = tokenHandler.WriteToken(tokenT);

            DateTime hrAct = DateTime.Now;
            DateTime hrExpr = hrAct.AddMinutes(30);

            return bearer_token;
        }
    }
}
