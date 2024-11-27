// NegocioPDF/Repositories/DetalleSuscripcionRepository.cs
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using System;


namespace NegocioPDF.Repositories
{
    public class DetalleSuscripcionRepository : IDetalleSuscripcionRepository
    {
        private readonly PDFSolutionsContext _context;

        public DetalleSuscripcionRepository(PDFSolutionsContext context)
        {
            _context = context;
        }

       public DetalleSuscripcion? ObtenerPorUsuarioId(int usuarioId) // Marca el tipo como nullable
{
    return _context.DetallesSuscripcion
        .Include(d => d.Usuario)
        .Where(d => d.UsuarioId == usuarioId)
        .OrderByDescending(d => d.Id)
        .FirstOrDefault();
}

public void ActualizarSuscripcion(DetalleSuscripcion suscripcion)
{
    var suscripcionExistente = _context.DetallesSuscripcion
        .FirstOrDefault(d => d.UsuarioId == suscripcion.UsuarioId);

    if (suscripcionExistente != null)
    {
          // Actualizar las propiedades específicas de la tabla
        suscripcionExistente.tipo_suscripcion = suscripcion.tipo_suscripcion;
        suscripcionExistente.fecha_inicio = suscripcion.fecha_inicio;
        suscripcionExistente.fecha_final = suscripcion.fecha_final;
        suscripcionExistente.precio = suscripcion.precio;
        suscripcionExistente.operaciones_realizadas = suscripcion.operaciones_realizadas;
        // Guardar los cambios en la base de datos
        _context.SaveChanges();
    }
    else
    {
        throw new Exception("No se encontró una suscripción para actualizar.");
    }
}
    }
}