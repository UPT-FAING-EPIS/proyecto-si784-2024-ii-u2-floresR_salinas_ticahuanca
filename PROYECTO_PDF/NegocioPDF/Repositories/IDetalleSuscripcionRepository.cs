using NegocioPDF.Models;
using NegocioPDF.Repositories;  // Añade esta línea
namespace NegocioPDF.Repositories
{
    public interface IDetalleSuscripcionRepository
    {
        DetalleSuscripcion ObtenerPorUsuarioId(int usuarioId);
    void ActualizarSuscripcion(DetalleSuscripcion suscripcion);
    }
}