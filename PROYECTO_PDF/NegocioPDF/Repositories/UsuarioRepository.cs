// NegocioPDF/Repositories/UsuarioRepository.cs
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;

namespace NegocioPDF.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly PDFSolutionsContext _context;

        public UsuarioRepository(PDFSolutionsContext context)
        {
            _context = context;
        }

        public Usuario? Login(string correo, string password)
        {
            return _context.Usuarios
                .FirstOrDefault(u => u.Correo == correo && u.Password == password);
        }

        public void RegistrarUsuario(Usuario usuario)
        {
            // Validar datos requeridos
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
            {
                throw new InvalidOperationException("El nombre es requerido.");
            }
            
            if (string.IsNullOrWhiteSpace(usuario.Correo))
            {
                throw new InvalidOperationException("El correo es requerido.");
            }
            
            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                throw new InvalidOperationException("La contraseña es requerida.");
            }

            // Verificar si el correo ya existe
            if (_context.Usuarios.Any(u => u.Correo == usuario.Correo))
            {
                throw new InvalidOperationException("El correo electrónico ya está registrado.");
            }

            if (_context.Database.IsInMemory())
            {
                // Omite transacciones si estás usando In-Memory
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                // Crear suscripción básica para el usuario
                CrearSuscripcionBasica(usuario);
                return;
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                // Crear suscripción básica para el usuario
                CrearSuscripcionBasica(usuario);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private void CrearSuscripcionBasica(Usuario usuario)
        {
            var suscripcion = new DetalleSuscripcion
            {
                UsuarioId = usuario.Id,
                tipo_suscripcion = "basico",
                precio = 0.00m,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddYears(1),
                operaciones_realizadas = 0
            };

            _context.DetallesSuscripcion.Add(suscripcion);
            _context.SaveChanges();
        }

        public IEnumerable<Usuario> ObtenerUsuarios()
        {
            return _context.Usuarios.ToList();
        }

        public Usuario? ObtenerUsuarioPorId(int idUsuario)
        {
            return _context.Usuarios.Find(idUsuario);
        }
    }
}