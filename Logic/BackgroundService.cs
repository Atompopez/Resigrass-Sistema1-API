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
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;


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
            string filePath = string.Empty;
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

                    filePath = GenerateWordDocument(record);
                    if(record.signature_image != null)
                        AddFloatingImageFromBytes(filePath, record.signature_image);

                    if (filePath != null)
                    {
                        // Añadir el archivo adjunto
                        using (var attachment = new Attachment(filePath))
                        {
                            mailMessage.Attachments.Add(attachment);
                            await smtpClient.SendMailAsync(mailMessage);
                            return;
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"No se pudo generar el documento para el registro con ID {record.id}.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al enviar el correo: {ex.Message}");
            }
            finally
            {
                bool fileDeleted = false;
                int attemptCount = 0;
                DeleteFile(filePath);
                while (!fileDeleted && attemptCount < 5)  // Intentamos 5 veces
                {
                    try
                    {
                        if (!IsFileLocked(filePath)) // Verificar si el archivo está bloqueado
                        {
                            DeleteFile(filePath);  // Si no está bloqueado, lo eliminamos
                            fileDeleted = true;
                        }
                    }
                    catch (IOException)
                    {
                        _logger.LogWarning($"El archivo {filePath} está bloqueado. Intentando nuevamente.");
                    }

                    await Task.Delay(1000);  // Esperamos 1 segundo antes de intentar de nuevo
                    attemptCount++;
                }
            }
        }
        #endregion

        public bool IsFileLocked(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException)
            {
                return true;
            }
        }


        public void DeleteFile(string filePath)
        {
            try
            {
                // Verificar si el archivo existe antes de intentar borrarlo
                if (File.Exists(filePath))
                {
                    File.Delete(filePath); // Elimina el archivo
                    Console.WriteLine($"Archivo {filePath} eliminado con éxito.");
                }
                else
                {
                    Console.WriteLine($"El archivo {filePath} no existe.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al intentar eliminar el archivo: {ex.Message}");
            }
        }

        public void AddFloatingImageFromBytes(string filePath, byte[] imageBytes)
        {
            imageBytes = QuitarFondoBlanco(imageBytes);

            try
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
                        new DW.Anchor(
                            new DW.HorizontalPosition(
                                new DW.HorizontalAlignment("right"))
                            { RelativeFrom = DW.HorizontalRelativePositionValues.RightMargin },
                            new DW.VerticalPosition(
                                new DW.VerticalAlignment("bottom"))
                            { RelativeFrom = DW.VerticalRelativePositionValues.BottomMargin },
                            new DW.Extent() { Cx = 2600000L, Cy = 2600000L }, // Tamaño de la imagen en EMU
                            new DW.EffectExtent()
                            {
                                LeftEdge = 0L,
                                TopEdge = 0L,
                                RightEdge = 0L,
                                BottomEdge = 0L
                            },
                            new DW.WrapNone(), // Configura que no haya ajuste de texto
                            new DW.DocProperties()
                            {
                                Id = (UInt32Value)1U,
                                Name = "FloatingImage"
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
                                                Name = "Image"
                                            },
                                            new PIC.NonVisualPictureDrawingProperties()),
                                        new PIC.BlipFill(
                                            new A.Blip()
                                            {
                                                Embed = imageId,
                                                CompressionState = A.BlipCompressionValues.Print
                                            },
                                            new A.Stretch(new A.FillRectangle()))),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(
                                        new A.Offset() { X = 0L, Y = 0L },
                                        new A.Extents() { Cx = 990000L, Cy = 600000L }),
                                    new A.PresetGeometry(new A.AdjustValueList())
                                    { Preset = A.ShapeTypeValues.Rectangle }))))
                        {
                            SimplePos = false,
                            BehindDoc = true, // Colocar detrás del texto
                            Locked = false,
                            LayoutInCell = true,
                            AllowOverlap = true // Permitir superposición con el texto
                        });

                    // Insertar la imagen en el documento
                    WordprocessingParagraph para = new WordprocessingParagraph(new WordprocessingRun(element));
                    Body body = mainPart.Document.Body;
                    body.AppendChild(para);

                    // Asegurarse de que los cambios se guarden correctamente
                    mainPart.Document.Save();
                }

                // Forzar a que el archivo se libere y que los recursos de la imagen sean liberados
                Task.Delay(500).Wait();  // Retraso para asegurar que el archivo se libere
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al agregar la imagen flotante: {ex.Message}");
            }
        }

        public byte[] QuitarFondoBlanco(byte[] imagenBytes)
        {
            using (var ms = new MemoryStream(imagenBytes))
            {
                Bitmap bitmap = new Bitmap(ms);

                System.Drawing.Color blanco = System.Drawing.Color.FromArgb(255, 255, 255);
                bitmap.MakeTransparent(blanco);

                using (var msSalida = new MemoryStream())
                {
                    bitmap.Save(msSalida, ImageFormat.Png);
                    return msSalida.ToArray();
                }
            }
        }

        #region GenerateWordDocument
        private string GenerateWordDocument(RecolectionModel record)
        {
            try
            {
                string templatePath = @"./Util/plantilla_certificado.docx";
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff");
                string outputPath = $@"./Util/Certificado_{timestamp}.docx";

                // Copiar el archivo de plantilla al destino
                File.Copy(templatePath, outputPath, true);

                // Abrir el archivo para manipulación
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

                    // Guardar los cambios en el documento
                    wordDoc.MainDocumentPart.Document.Save();
                }

                // Añadir un pequeño retraso para asegurar que el archivo esté completamente liberado
                Task.Delay(500).Wait();  // Espera de 500 ms

                // El archivo se cierra automáticamente al salir del bloque using
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
