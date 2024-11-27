// NegocioPDF/Models/Usuario.cs

namespace NegocioPDF.Models
{
    public class Usuario
    {
        public int Id { get; set; }
    public required string Nombre { get; set; }
    public required string Correo { get; set; }
    public required string Password { get; set; }
    }
}
