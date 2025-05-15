using System.Net.Mail;
using System.Net;
using ResiGrass_API.Models;
using System.Text;
using System.Text.Json;
using Resend;
using System.Net.Mime;

namespace ResiGrass_API.Logic
{
    public class SendTokenUser : BackgroundService
    {
        private readonly SmtpClient _smtpClient;
        private readonly TimeSpan _interval = TimeSpan.FromDays(7);
        Random random;
        DbQuery _dbQuery;
        IResend _resend;

        public SendTokenUser(DbQuery dbQuery)
        {
            random = new Random();
            _dbQuery = dbQuery;
            _resend = ResendClient.Create("re_4JikCCrp_7aDpuYuga72LrYVUwxHeFjrb");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var users = await _dbQuery.GetUsersToSendToken();
                    if (users != null && users.Any())
                    {
                        await SendTokensAsync(users);
                    }
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, "Error en el servicio de notificación de correo.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }

        public async Task<bool> SendTokensAsync(List<DataUpdateToken> users)
        {
            try
            {
                foreach (var user in users)
                {
                    var token = GenerateToken();

                    if (!await UpdateDB(user.id, token))
                    {
                        return false;
                    }

                    string emailBody = CreateEmailBody(token);

                    var message = new EmailMessage
                    {
                        From = "Resigrass <notificaciones@resigrass.com.co>",
                        To = user.email,
                        Subject = "Notificación de nuevo token",
                        HtmlBody = emailBody
                    };

                    await _resend.EmailSendAsync(message);
                }
                return true;
                await Task.Delay(1000); // Espera 1 segundo entre envíos para evitar problemas de límite de envío
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> UpdateDB(int idUser, string token)
        {
            try
            {
                return await _dbQuery.UpdateTokenUser(idUser, token);
            }
            catch
            {
                return false;
            }
        }

        private string CreateEmailBody(string token)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("<h1>RESIGRASS</h1>");
                sb.AppendLine("<p>Estimado recolector,</p>");
                sb.AppendLine($"<p>Le informamos que su token semanal número <strong>{token}</strong> ha sido generado y está vigente a partir del envío de este correo.</p>");
                sb.AppendLine("<p>Para más información, por favor contacte al sistema administrativo.</p>");
                sb.AppendLine("<p>Gracias por su colaboración.</p>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                // Log del error con mayor información contextual
                //_logger?.LogError(ex, "Error al generar el cuerpo del correo para el token {Token}", token);

                // Mensaje amigable en caso de error
                return "<h1>RESIGRASS</h1>" +
                       "<p>Estimado recolector,</p>" +
                       "<p>Ha ocurrido un error al generar su token semanal. Por favor, contacte al sistema administrativo para recibir asistencia.</p>" +
                       "<p>Gracias por su comprensión.</p>";
            }
        }

        private string GenerateToken()
        {
            int token = random.Next(1000, 10000);
            return token.ToString();
        }
    }
}
