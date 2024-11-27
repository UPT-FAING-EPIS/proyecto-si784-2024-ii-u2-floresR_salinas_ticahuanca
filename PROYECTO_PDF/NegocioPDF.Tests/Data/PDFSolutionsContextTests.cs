using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;

namespace NegocioPDF.Tests.Data
{
    [TestFixture]
    public class PDFSolutionsContextTests
    {
        private PDFSolutionsContext _context = null!;

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
        public void Context_DebeConfigurarRelacionesCorrectamente()
        {
            // Arrange & Act
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
                fecha_final = DateTime.Now.AddYears(1),
                precio = 0.00m
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

            // Assert
            var usuarioGuardado = _context.Usuarios.Find(usuario.Id);
            var suscripcionGuardada = _context.DetallesSuscripcion.FirstOrDefault(d => d.UsuarioId == usuario.Id);
            var operacionGuardada = _context.OperacionesPDF.FirstOrDefault(o => o.UsuarioId == usuario.Id);

            Assert.Multiple(() =>
            {
                Assert.That(usuarioGuardado, Is.Not.Null);
                Assert.That(suscripcionGuardada, Is.Not.Null);
                Assert.That(operacionGuardada, Is.Not.Null);
                Assert.That(suscripcionGuardada?.UsuarioId, Is.EqualTo(usuario.Id));
                Assert.That(operacionGuardada?.UsuarioId, Is.EqualTo(usuario.Id));
            });
        }

        [Test]
        public void Context_DebeEliminarEnCascada()
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
                Assert.That(_context.DetallesSuscripcion.Count(), Is.EqualTo(0));
                Assert.That(_context.OperacionesPDF.Count(), Is.EqualTo(0));
            });
        }
    }
}