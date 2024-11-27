using NegocioPDF.Models;
using System.Collections.Generic;

namespace NegocioPDF.Repositories
{
    public interface IOperacionesPDFRepository
    {
        bool RegistrarOperacionPDF(int usuarioId, string tipoOperacion);
        IEnumerable<OperacionPDF> ObtenerOperacionesPorUsuario(int usuarioId);
        int ContarOperacionesRealizadas(int usuarioId);
        bool ValidarOperacion(int usuarioId);
    }
}