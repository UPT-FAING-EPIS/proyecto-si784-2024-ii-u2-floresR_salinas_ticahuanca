using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;

namespace NegocioPDF.Tests.Repositories
{
    [TestFixture]
    public class MigrationsTests
    {
        private PDFSolutionsContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new PDFSolutionsContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public void DbContext_DebePermitirOperacionesBasicas()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "password123"
            };

            // Act
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var suscripcion = new DetalleSuscripcion
            {
                UsuarioId = usuario.Id,
                tipo_suscripcion = "basico",
                operaciones_realizadas = 0,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddYears(1),
                precio = 0.00m
            };

            _context.DetallesSuscripcion.Add(suscripcion);
            _context.SaveChanges();

            var operacion = new OperacionPDF
            {
                UsuarioId = usuario.Id,
                TipoOperacion = "Fusionar",
                FechaOperacion = DateTime.Now
            };

            _context.OperacionesPDF.Add(operacion);
            _context.SaveChanges();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_context.Usuarios.Count(), Is.EqualTo(1));
                Assert.That(_context.DetallesSuscripcion.Count(), Is.EqualTo(1));
                Assert.That(_context.OperacionesPDF.Count(), Is.EqualTo(1));

                var usuarioGuardado = _context.Usuarios.First();
                Assert.That(usuarioGuardado.Correo, Is.EqualTo("test@test.com"));

                var suscripcionGuardada = _context.DetallesSuscripcion.First();
                Assert.That(suscripcionGuardada.tipo_suscripcion, Is.EqualTo("basico"));

                var operacionGuardada = _context.OperacionesPDF.First();
                Assert.That(operacionGuardada.TipoOperacion, Is.EqualTo("Fusionar"));
            });
        }

        [Test]
        public void DbContext_DebeEliminarEnCascada()
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
                operaciones_realizadas = 0
            };
            _context.DetallesSuscripcion.Add(suscripcion);

            var operacion = new OperacionPDF
            {
                UsuarioId = usuario.Id,
                TipoOperacion = "Fusionar",
                FechaOperacion = DateTime.Now
            };
            _context.OperacionesPDF.Add(operacion);
            _context.SaveChanges();

            // Act
            _context.Usuarios.Remove(usuario);
            _context.SaveChanges();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(_context.Usuarios.Count(), Is.EqualTo(0));
                Assert.That(_context.DetallesSuscripcion.Count(), Is.EqualTo(0));
                Assert.That(_context.OperacionesPDF.Count(), Is.EqualTo(0));
            });
        }
    }
}