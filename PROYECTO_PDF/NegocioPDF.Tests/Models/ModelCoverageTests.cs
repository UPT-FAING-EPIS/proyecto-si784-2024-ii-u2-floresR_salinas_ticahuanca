using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Models
{
    [TestFixture]
    public class ModelCoverageTests
    {
        private PDFSolutionsContext _context;
        private IDetalleSuscripcionRepository _detalleSuscripcionRepo;
        private IUsuarioRepository _usuarioRepo;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new PDFSolutionsContext(options);
            _detalleSuscripcionRepo = new DetalleSuscripcionRepository(_context);
            _usuarioRepo = new UsuarioRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public void Usuario_PropiedadesDebenFuncionar()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "password123"
            };

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(usuario.Nombre, Is.EqualTo("Test User"));
                Assert.That(usuario.Correo, Is.EqualTo("test@test.com"));
                Assert.That(usuario.Password, Is.EqualTo("password123"));
            });
        }

        [Test]
        public void DetalleSuscripcion_PropiedadesDebenFuncionar()
        {
            // Arrange
            var fechaInicio = DateTime.Now;
            var fechaFinal = fechaInicio.AddMonths(1);
            var detalleSuscripcion = new DetalleSuscripcion
            {
                tipo_suscripcion = "premium",
                fecha_inicio = fechaInicio,
                fecha_final = fechaFinal,
                precio = 99.99m,
                operaciones_realizadas = 5,
                UsuarioId = 1
            };

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(detalleSuscripcion.tipo_suscripcion, Is.EqualTo("premium"));
                Assert.That(detalleSuscripcion.fecha_inicio, Is.EqualTo(fechaInicio));
                Assert.That(detalleSuscripcion.fecha_final, Is.EqualTo(fechaFinal));
                Assert.That(detalleSuscripcion.precio, Is.EqualTo(99.99m));
                Assert.That(detalleSuscripcion.operaciones_realizadas, Is.EqualTo(5));
                Assert.That(detalleSuscripcion.UsuarioId, Is.EqualTo(1));
            });
        }

        [Test]
        public void OperacionPDF_PropiedadesDebenFuncionar()
        {
            // Arrange
            var fechaOperacion = DateTime.Now;
            var operacionPDF = new OperacionPDF
            {
                UsuarioId = 1,
                TipoOperacion = "Fusionar",
                FechaOperacion = fechaOperacion
            };

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.That(operacionPDF.UsuarioId, Is.EqualTo(1));
                Assert.That(operacionPDF.TipoOperacion, Is.EqualTo("Fusionar"));
                Assert.That(operacionPDF.FechaOperacion, Is.EqualTo(fechaOperacion));
            });
        }
    }
}