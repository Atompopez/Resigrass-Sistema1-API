using Microsoft.EntityFrameworkCore;
using ResiGrass_API.Models;
using System.Net.Mail;
using System.Net;
using System.Text;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ResiGrass_API.Logic
{
    public class EmailNotificationService : BackgroundService
    {
        private readonly DbQuery _dbQuery;
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(48);

        public EmailNotificationService(DbQuery dbQuery, ILogger<EmailNotificationService> logger)
        {
            _dbQuery = dbQuery;
            _logger = logger;
        }

        #region ExecuteAsync
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SendNotificationsAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el servicio de notificación de correo.");
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
        #endregion

        #region SendNotificationsAsync
        public async Task SendNotificationsAsync()
        {
            var recordsToNotify = _dbQuery.GetRecordsDueInTwoDays();

            if (recordsToNotify.Any())
            {
                await SendEmailAsync(recordsToNotify);
            }
        }
        #endregion

        #region SendEmailAsync
        public async Task SendEmailAsync(List<RecolectionModel> records)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("resigrass0@gmail.com", "xnzs bpwv mlhk fmxi"),
                    EnableSsl = true,
                };

                foreach (var record in records)
                {
                    string emailBody = CreateEmailBody(record);

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("resigrass0@gmail.com"),
                        Subject = "Notificación de registros a dos días",
                        Body = emailBody,
                        IsBodyHtml = true,
                    };

                  //  record.email = "davidsant2188@gmail.com"; //SOLO PARA PRUEBAS
                    mailMessage.To.Add(record.email);

                    string filePath = GenerateWordDocument(record);
                    if (filePath != null)
                    {
                        Attachment attachment = new Attachment(filePath);
                        mailMessage.Attachments.Add(attachment);
                    }
                    else
                    {
                        _logger.LogWarning($"No se pudo generar el documento para el registro con ID {record.id}.");
                    }

                    await smtpClient.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar el correo: {ex.Message}");
            }
        }
        #endregion

        #region GenerateWordDocument
        private string GenerateWordDocument(RecolectionModel record)
        {
            try
            {
                string templatePath = @"./Util/WordTemplate.docx";
                string outputPath = $@"./Util/Certificado_{record.id}.docx";
                File.Copy(templatePath, outputPath, true);

               

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(outputPath, true))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;

                    ReplaceTextInDocument(body, "Nombre_Generador", record.nameClient ?? "No disponible");
                    ReplaceTextInDocument(body, "Punto_Venta", record.nameHeadquarter ?? "No disponible");
                    ReplaceTextInDocument(body, "Nitt", record.nitCc ?? "No disponible");
                    ReplaceTextInDocument(body, "Direccionn", record.address + ',' + record.nameHeadquarter ?? "No disponible");
                    ReplaceTextInDocument(body, "Telefonoo", record.numberPhone ?? "No disponible");
                    ReplaceTextInDocument(body, "Tipo_Negocio", record.businessType ?? "No disponible");
                    ReplaceTextInDocument(body, "KG_Recibido", record.netWeight.ToString());
                    ReplaceTextInDocument(body, "Fecha_Recoleccion", record.receivedDate.ToShortDateString());
                    ReplaceTextInDocument(body, "Fecha_Hoy_Cliente", record.receivedDate.ToShortDateString());

                    // Calcula la fecha límite como 30 días después de la fecha de recolección
                    DateTime fechaLimiteCliente = record.receivedDate.AddDays(30);
                    ReplaceTextInDocument(body, "Fecha_Limite_Cliente", fechaLimiteCliente.ToShortDateString());

                    // Sustituye Fecha_Hoy por el número de serie
                    ReplaceTextInDocument(body, "Fecha_Hoy", record.seria_number ?? "No disponible");

                    wordDoc.MainDocumentPart.Document.Save();
                }

                return outputPath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al generar el documento Word: {ex.Message}");
                return null;
            }
        }



        private void ReplaceTextInDocument(Body body, string placeholder, string newValue)
        {
            try
            {
                foreach (var text in body.Descendants<Text>())
                {
                    if (text.Text.Contains(placeholder))
                    {
                        text.Text = text.Text.Replace(placeholder, newValue);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al remplazar el documento: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region CreateEmailBody
        private string CreateEmailBody(RecolectionModel record)
        {
            try
            {
                var sb = new StringBuilder();
                sb.Append("<h1>RESIGRASS</h1>");
                sb.Append("<p>Estimado cliente,</p>");
                sb.Append("<p>Le notificamos que en dos días se cumplirá la fecha para la recolección del aceite.</p>");
                sb.Append("<ul>");
                sb.AppendFormat("<li>Kilogramos recibidos: {0}</li>", record.netWeight);
                sb.AppendFormat("<li>Date de recolección: {0}</li>", record.receivedDate.ToShortDateString());
                sb.Append("</ul>");
                sb.Append("<p>Gracias por su atención.</p>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear el body del mail: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}
