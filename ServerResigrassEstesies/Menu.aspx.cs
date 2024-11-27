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


namespace ServerResigrassEstesies
{
    public partial class _Menu : Page
    {
        public ResponseLogin response = new ResponseLogin();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["IsAuthenticated"] == null || (bool)Session["IsAuthenticated"] == false)
            {
                Response.Redirect("Default.aspx", false);  // Redirige al login si no está autenticado
                Context.ApplicationInstance.CompleteRequest();
            }

        }
       
    }

}