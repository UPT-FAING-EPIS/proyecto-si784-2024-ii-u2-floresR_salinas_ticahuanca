// NegocioPDF/Models/OperacionPDF.cs
using System;

namespace NegocioPDF.Models
{
    public class OperacionPDF
    {
            public int Id { get; set; }
        public int UsuarioId { get; set; }
        public required string TipoOperacion { get; set; }
        public DateTime FechaOperacion { get; set; }
    }
}
