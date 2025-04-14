using ResiGrass_API.Models;
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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Resend;
using System.Net.Mail;
using System.Net.Mime;

namespace ResiGrass_API.Logic
{
    public class EmailNotificationService : BackgroundService
    {
        private readonly DbQuery _dbQuery;
        private readonly ILogger<EmailNotificationService> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromHours(48);
        private readonly IResend _resend;

        public EmailNotificationService(DbQuery dbQuery, ILogger<EmailNotificationService> logger)
        {
            _dbQuery = dbQuery;
            _logger = logger;
            _resend = ResendClient.Create("re_4JikCCrp_7aDpuYuga72LrYVUwxHeFjrb");
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
            foreach (var record in records)
            {
                string filePath = string.Empty;

                try
                {
                    string emailBody = CreateEmailBody(record);

                    // Generar archivo Word con imagen (si aplica)
                    filePath = GenerateWordDocument(record);
                    if (record.signature_image != null)
                        AddFloatingImageFromBytes(filePath, record.signature_image);

                    if (!File.Exists(filePath))
                    {
                        _logger.LogWarning($"No se pudo generar el documento para el registro con ID {record.id}.");
                        continue;
                    }

                    byte[] fileBytes = File.ReadAllBytes(filePath);

                    var message = new EmailMessage
                    {
                        From = "Resigrass <notificaciones@resigrass.com.co>",
                        To = record.email,
                        Subject = $"{DateTime.Now:dd MMM yyyy} Certificado de Recolección Aceite Vegetal Usado",
                        HtmlBody = emailBody,
                        Attachments = new List<EmailAttachment>()
                        {
                            new EmailAttachment
                            {
                                Filename = "Certificado.docx",
                                Content = fileBytes,
                                ContentType = MediaTypeNames.Application.Octet
                            }
                        }
                    };

                    var response = await _resend.EmailSendAsync(message);
                    _logger.LogInformation($"Correo enviado a {record.email}. ID: {response.Content}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error al enviar el correo a {record.email}: {ex.Message}");
                }
                finally
                {
                    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                        File.Delete(filePath);
                }
            }
        }
        #endregion

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
                                        new A.Stretch(new A.FillRectangle())),
                                    new PIC.ShapeProperties(
                                        new A.Transform2D(
                                            new A.Offset() { X = 0L, Y = 0L },
                                            new A.Extents() { Cx = 990000L, Cy = 600000L }),
                                        new A.PresetGeometry(new A.AdjustValueList())
                                        { Preset = A.ShapeTypeValues.Rectangle }))))
                        )
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

                mainPart.Document.Save();
            }
        }

        public byte[] QuitarFondoBlanco(byte[] imagenBytes)
        {
            using (var ms = new MemoryStream(imagenBytes))
            {
                // Cargar la imagen
                using (var image = SixLabors.ImageSharp.Image.Load<Rgba32>(ms))
                {
                    // Definir el color blanco (que se eliminará)
                    var blanco = new Rgba32(255, 255, 255);

                    // Recorrer todos los píxeles y hacer transparentes los píxeles blancos
                    for (int y = 0; y < image.Height; y++)
                    {
                        for (int x = 0; x < image.Width; x++)
                        {
                            var color = image[x, y];
                            if (color.Equals(blanco))
                            {
                                image[x, y] = new Rgba32(0, 0, 0, 0); // Hacerlo transparente
                            }
                        }
                    }

                    // Guardar la imagen modificada en un MemoryStream
                    using (var msSalida = new MemoryStream())
                    {
                        image.SaveAsPng(msSalida); // Guardar en formato PNG
                        return msSalida.ToArray();
                    }
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
                sb.AppendLine("<p><strong>NOTA:</strong> Imprimir Documento, Anexar carpeta para visita de sanidad.</p>");
                sb.AppendLine("<h2>Aceite Vegetal Usado (AVU)</h2>");
                sb.AppendLine("<h3>Resigrass S.A.S</h3>");
                sb.AppendLine("<p><strong>Correo electrónico:</strong> resigrass@hotmail.com</p>");
                sb.AppendLine("<p><strong>Teléfonos:</strong> 3208482407 - 3106747173</p>");

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
