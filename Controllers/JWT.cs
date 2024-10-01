using System.ComponentModel.DataAnnotations;

namespace ResiGrass_API.Controllers
{
    public class JWT
    {
        [Required] public string Key { get; set; }
        [Required] public string Issuer { get; set; }
        [Required] public string Audience { get; set; }
        [Required] public string Subject { get; set; }
    }
}
