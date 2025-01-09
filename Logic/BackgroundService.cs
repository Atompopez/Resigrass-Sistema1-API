using Microsoft.EntityFrameworkCore;
using ResiGrass_API.Models;
using System.Net.Mail;
using System.Net;
using System.Text;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Drawing;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;
using WordprocessingParagraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using WordprocessingRun = DocumentFormat.OpenXml.Wordprocessing.Run;
using WordprocessingText = DocumentFormat.OpenXml.Wordprocessing.Text;


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
                        Subject = "Notificación de nuevo certificado de recolección",
                        Body = emailBody,
                        IsBodyHtml = true,
                    };

                  //  record.email = "davidsant2188@gmail.com"; //SOLO PARA PRUEBAS
                    mailMessage.To.Add(record.email);

                    string filePath = GenerateWordDocument(record);
                    if(record.signature_image != null)
                        AddFloatingImageFromBytes(filePath, record.signature_image);

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



        public void AddFloatingImageFromBytes(string filePath, byte[] imageBytes)
        {
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, true))
            {
                MainDocumentPart mainPart = wordDoc.MainDocumentPart;

                // Agregar la imagen como ImagePart a partir de los bytes
                ImagePart imagePart = mainPart.AddImagePart(ImagePartType.Jpeg);

                using (MemoryStream stream = new MemoryStream(imageBytes))
                {
                    imagePart.FeedData(stream);  // Alimentar la imagen desde el MemoryStream
                }

                string imageId = mainPart.GetIdOfPart(imagePart);

                // Crear el elemento Drawing para la imagen flotante
                Drawing element = new Drawing(
                    new DW.Inline(
                        new DW.Extent() { Cx = 950000L, Cy = 780000L }, // Tamaño de la imagen en EMU
                        new DW.EffectExtent()
                        {
                            LeftEdge = 100L,  // Mover 100 EMUs hacia la derecha
                            TopEdge = 200L,   // Mover 200 EMUs hacia abajo
                            RightEdge = 0L,   // No usar desplazamiento desde el borde derecho
                            BottomEdge = 0L
                        },
                        new DW.DocProperties()
                        {
                            Id = (UInt32Value)1U,
                            Name = "Picture 1"
                        },
                        new DW.NonVisualGraphicFrameDrawingProperties(
                            new A.GraphicFrameLocks() { NoChangeAspect = true }),
                        new A.Graphic(
                            new A.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0U,
                                            Name = "Embedded Image"
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()),
                                    new PIC.BlipFill(
                                        new A.Blip()
                                        {
                                            Embed = imageId
                                        },
                                        new A.Stretch(new A.FillRectangle())),
                                    new PIC.ShapeProperties(
                                        new A.Transform2D(
                                            new A.Offset() { X = 0L, Y = 0 },
                                            new A.Extents() { Cx = 950000L, Cy = 780000L }),
                                        new A.PresetGeometry(new A.AdjustValueList())
                                        { Preset = A.ShapeTypeValues.Rectangle }))))
                    )
                    {
                        DistanceFromTop = 0U,
                        DistanceFromBottom = 0U,
                        DistanceFromLeft = 0U,
                        DistanceFromRight = 0U
                    });

                // Insertar la imagen en el documento
                WordprocessingParagraph para = new WordprocessingParagraph(new WordprocessingRun(element));
                Body body = mainPart.Document.Body;
                body.AppendChild(para);

                mainPart.Document.Save();
            }
        }



        #region GenerateWordDocument
        private string GenerateWordDocument(RecolectionModel record)
        {
            try
            {
                string templatePath = @"./Util/plantilla_certificado.docx";
                string outputPath = $@"./Util/Certificado_{record.id}.docx";
                File.Copy(templatePath, outputPath, true);

               

                using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(outputPath, true))
                {
                    var body = wordDoc.MainDocumentPart.Document.Body;

                    ReplaceTextInDocument(body, "serie_recoleccion", record.serial_number ?? "No disponible");
                    ReplaceTextInDocument(body, "name", record.nameClient ?? "No disponible");
                    ReplaceTextInDocument(body, "nit", record.nitCc ?? "No disponible");
                    ReplaceTextInDocument(body, "address", record.address ?? "No disponible");
                    ReplaceTextInDocument(body, "locality", record.nameLocality ?? "No disponible");
                    ReplaceTextInDocument(body, "phone", record.numberPhone ?? "No disponible");
                    ReplaceTextInDocument(body, "weigth", record.netWeight.ToString());
                    ReplaceTextInDocument(body, "date", record.receivedDate.ToShortDateString());
                    ReplaceTextInDocument(body, "finish", record.endDate.ToShortDateString());

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
                foreach (var text in body.Descendants<WordprocessingText>())
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
                sb.AppendLine("<h1>RESIGRASS</h1>");
                sb.AppendLine("<p>Estimado cliente,</p>");
                sb.AppendLine($"<p>Este es su certificado de la recolección número <strong>{record.serial_number}</strong>, realizada el día <strong>{record.receivedDate.ToShortDateString()}</strong>.</p>");
                sb.AppendLine("<ul>");
                sb.AppendLine($"<li>Kilogramos recibidos: {record.netWeight}</li>");
                sb.AppendLine("</ul>");
                sb.AppendLine("<p>Gracias por confiar en nuestros servicios.</p>");

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
