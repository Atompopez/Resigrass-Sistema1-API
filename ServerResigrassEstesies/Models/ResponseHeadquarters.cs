using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServerResigrassEstesies.Models
{
    public class ResponseHeadquarters
    {
        public string NameHeadquarter { get; set; } // Nombre de la sede
        public string Address { get; set; } // Dirección de la sede
        public string NumberPhone { get; set; } // Número de teléfono
        public string clientId { get; set; } // Número de teléfono
        public DateTime DateCreationHeadquarter { get; set; } // Fecha de creación de la sede
        public bool Status { get; set; } // Estado (activo o inactivo)
    }
}