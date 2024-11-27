using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using ServerResigrassEstesies.Models;
using ServerResigrassEstesies.Logic;


namespace ServerResigrassEstesies
{
    public partial class _Default : Page
    {
        public ResponseLogin response = new ResponseLogin();

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        public void btnLogin_Click(object sender, EventArgs e)
        {
           

            var datos = new
            {
                password = txtPassword.Text,
                user = txtUsername.Text
            };

            string jsonBody = JsonConvert.SerializeObject(datos);

            // Crear una instancia de HttpClient
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Define el contenido de la solicitud como JSON
                    StringContent contenido = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    // Envía la solicitud POST
                    HttpResponseMessage respuesta = client.PostAsync(Globals.url + "/UserAdmin/ControllerLoginUserAdmin", contenido).Result;

                    // Verifica si la solicitud fue exitosa
                    if (respuesta.IsSuccessStatusCode)
                    {
                        // Lee el contenido de la respuesta y deserialízalo
                        string respuestaContenido = respuesta.Content.ReadAsStringAsync().Result;

                        // Deserializar el contenido para obtener solo el token
                         response = JsonConvert.DeserializeObject<ResponseLogin>(respuestaContenido);

                        // Guarda solo el token en la sesión
                        if (response != null)
                        {
                            Session["IsAuthenticated"] = true;
                            Session["Token"] = response.token;
                        }

                        // Redirige al menú
                        Response.Redirect("Menu.aspx");
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        if (respuesta != null)
                        {
                            response.message = respuesta.Content.ReadAsStringAsync().Result;
                            Console.WriteLine(response.message);
                            lblMessage.Visible = true;
                            lblMessage.Text = response.message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error: {ex.Message}");
                }
            }
        }

    }

}