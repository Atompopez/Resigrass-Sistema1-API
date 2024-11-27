using Newtonsoft.Json; // Para manejar JSON
using ServerResigrassEstesies.Logic;
using System;
using System.Collections.Generic;
using System.Net.Http; // Para HttpClient
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ServerResigrassEstesies
{
    public partial class _headquarters : Page
    {
        // Modelo basado en el JSON proporcionado
        public class ResponseHeadquarters
        {
            public string NameHeadquarter { get; set; } // Nombre de la sede
            public string Address { get; set; } // Dirección de la sede
            public string NumberPhone { get; set; } // Número de teléfono
            public string clientId { get; set; } // Número de teléfono
            public DateTime DateCreationHeadquarter { get; set; } // Fecha de creación de la sede
            public bool Status { get; set; } // Estado (activo o inactivo)
            public string StatusText => Status ? "Activo" : "Inactivo";
        }

        protected async void Page_Load(object sender, EventArgs e)
        {
            // Verifica la autenticación
            if (Session["IsAuthenticated"] == null || (bool)Session["IsAuthenticated"] == false)
            {
                Response.Redirect("Default.aspx", false); // Redirige al login si no está autenticado
                Context.ApplicationInstance.CompleteRequest();
            }

            if (!IsPostBack)
            {
                await CargarSedesDesdeServicio(); // Llama al servicio GET y carga datos en el GridView
            }
        }

        protected async void btnConsultar_Click(object sender, EventArgs e)
        {
            // Filtra las sedes por el nombre ingresado
            string filtro = tbNombre.Text.Trim().ToLower();
            await CargarSedesDesdeServicio(filtro);
        }

        protected async void btnAgregar_Click(object sender, EventArgs e)
        {
            // Nota: Esta funcionalidad necesita lógica adicional para enviar datos al servicio.
            // Por ahora, puedes agregar una sede localmente.
            //lblError.Text = "La funcionalidad de agregar sede aún no está implementada con el servicio.";
        }

        private async Task CargarSedesDesdeServicio(string filtro = "")
        {
            

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Recupera el Bearer Token desde Session
                    if (Session["Token"] == null)
                    {
                        //lblError.Text = "Error: No se encontró el token de autenticación.";
                        return;
                    }
                    string bearerToken = Session["Token"].ToString();

                    // Agrega el token al encabezado de la solicitud
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    // Realiza la solicitud GET
                    HttpResponseMessage response = await client.GetAsync(Globals.url+"/Client/0/0/GetHeadquarters");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = await response.Content.ReadAsStringAsync();

                        // Deserializa la respuesta JSON a una lista de ResponseHeadquarters
                        List<ResponseHeadquarters> sedes = JsonConvert.DeserializeObject<List<ResponseHeadquarters>>(jsonResponse);
                        // Aplica el filtro si existe
                        if (!string.IsNullOrEmpty(filtro))
                        {
                            sedes = sedes.FindAll(s => s.NameHeadquarter.ToLower().Contains(filtro));
                        }

                        // Enlaza los datos al GridView
                        gvSedes.DataSource = sedes;
                        gvSedes.DataBind();
                    }
                    else
                    {
                        //lblError.Text = "Error al obtener los datos del servicio.";
                    }
                }
                catch (Exception ex)
                {
                    //lblError.Text = $"Ocurrió un error: {ex.Message}";
                }
            }
        }
    }
}
