using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NegocioPDF.Repositories
{
    public class OperacionesPDFRepository : IOperacionesPDFRepository
    {
        private readonly PDFSolutionsContext _context;

        public OperacionesPDFRepository(PDFSolutionsContext context)
        {
            _context = context;
        }

        public bool RegistrarOperacionPDF(int usuarioId, string tipoOperacion)
        {
            try
            {
                var suscripcion = _context.DetallesSuscripcion
                    .FirstOrDefault(d => d.UsuarioId == usuarioId);

                if (suscripcion == null)
                {
                    return false;
                }

                if (suscripcion.tipo_suscripcion == "basico" && suscripcion.operaciones_realizadas >= 5)
                {
                    return false;
                }

                var operacion = new OperacionPDF
                {
                    UsuarioId = usuarioId,
                    TipoOperacion = tipoOperacion,
                    FechaOperacion = DateTime.Now
                };

                _context.OperacionesPDF.Add(operacion);

                if (suscripcion.tipo_suscripcion == "basico")
                {
                    suscripcion.operaciones_realizadas++;
                    _context.DetallesSuscripcion.Update(suscripcion);
                }

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<OperacionPDF> ObtenerOperacionesPorUsuario(int usuarioId)
        {
            return _context.OperacionesPDF
                .Where(o => o.UsuarioId == usuarioId)
                .OrderByDescending(o => o.FechaOperacion)
                .ToList();
        }

        public int ContarOperacionesRealizadas(int usuarioId)
        {
            return _context.OperacionesPDF.Count(o => o.UsuarioId == usuarioId);
        }

        public bool ValidarOperacion(int usuarioId)
        {
            var suscripcion = _context.DetallesSuscripcion
                .FirstOrDefault(d => d.UsuarioId == usuarioId);

            if (suscripcion == null)
            {
                return false;
            }

            return !(suscripcion.tipo_suscripcion == "basico" && suscripcion.operaciones_realizadas >= 5);
        }
    }
}