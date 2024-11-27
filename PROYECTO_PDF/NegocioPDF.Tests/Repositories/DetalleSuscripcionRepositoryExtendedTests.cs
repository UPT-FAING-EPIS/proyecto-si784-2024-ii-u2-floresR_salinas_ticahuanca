using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Repositories
{
    [TestFixture]
    public class DetalleSuscripcionRepositoryExtendedTests
    {
        private PDFSolutionsContext _context;
        private IDetalleSuscripcionRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new PDFSolutionsContext(options);
            _repository = new DetalleSuscripcionRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public void ActualizarSuscripcion_DebeActualizarOperacionesRealizadas()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "password123"
            };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var suscripcion = new DetalleSuscripcion
            {
                UsuarioId = usuario.Id,
                tipo_suscripcion = "basico",
                operaciones_realizadas = 0,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddMonths(1),
                precio = 0.00m
            };
            _context.DetallesSuscripcion.Add(suscripcion);
            _context.SaveChanges();

            // Act
            suscripcion.operaciones_realizadas = 3;
            _repository.ActualizarSuscripcion(suscripcion);

            // Assert
            var suscripcionActualizada = _repository.ObtenerPorUsuarioId(usuario.Id);
            Assert.That(suscripcionActualizada.operaciones_realizadas, Is.EqualTo(3));
        }

        [Test]
        public void DetalleSuscripcion_DebeRelacionarseConUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "password123"
            };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var suscripcion = new DetalleSuscripcion
            {
                UsuarioId = usuario.Id,
                tipo_suscripcion = "basico",
                operaciones_realizadas = 0,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddMonths(1),
                precio = 0.00m,
                Usuario = usuario
            };
            _context.DetallesSuscripcion.Add(suscripcion);
            _context.SaveChanges();

            // Act
            var suscripcionConUsuario = _context.DetallesSuscripcion
                .Include(d => d.Usuario)
                .FirstOrDefault(d => d.UsuarioId == usuario.Id);

            // Assert
            Assert.That(suscripcionConUsuario, Is.Not.Null);
            Assert.That(suscripcionConUsuario.Usuario, Is.Not.Null);
            Assert.That(suscripcionConUsuario.Usuario.Id, Is.EqualTo(usuario.Id));
            Assert.That(suscripcionConUsuario.Usuario.Correo, Is.EqualTo(usuario.Correo));
        }
    }
}