using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Repositories
{
    [TestFixture]
    public class DetalleSuscripcionRepositoryDetailedTests
    {
        private PDFSolutionsContext _context = null!;
        private IDetalleSuscripcionRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new PDFSolutionsContext(options);
            _repository = new DetalleSuscripcionRepository(_context);

            ConfigurarDatosPrueba();
        }

        private void ConfigurarDatosPrueba()
        {
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "password123"
            };
            _context.Usuarios.Add(usuario);
            
            var suscripcion = new DetalleSuscripcion
            {
                UsuarioId = 1,
                tipo_suscripcion = "basico",
                operaciones_realizadas = 0,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddMonths(1),
                precio = 0.00m
            };
            _context.DetallesSuscripcion.Add(suscripcion);
            
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void ActualizarSuscripcion_DebeActualizarTodosLosCampos()
        {
            // Arrange
            var suscripcion = _repository.ObtenerPorUsuarioId(1);
            Assert.That(suscripcion, Is.Not.Null);

            var nuevaFechaFinal = DateTime.Now.AddYears(1);
            
            // Act
            suscripcion!.tipo_suscripcion = "premium";
            suscripcion.operaciones_realizadas = 10;
            suscripcion.fecha_final = nuevaFechaFinal;
            suscripcion.precio = 99.99m;
            
            _repository.ActualizarSuscripcion(suscripcion);

            // Assert
            var suscripcionActualizada = _repository.ObtenerPorUsuarioId(1);
            Assert.Multiple(() =>
            {
                Assert.That(suscripcionActualizada!.tipo_suscripcion, Is.EqualTo("premium"));
                Assert.That(suscripcionActualizada.operaciones_realizadas, Is.EqualTo(10));
                Assert.That(suscripcionActualizada.fecha_final!.Value.Date, Is.EqualTo(nuevaFechaFinal.Date));
                Assert.That(suscripcionActualizada.precio, Is.EqualTo(99.99m));
            });
        }

        [Test]
        public void ActualizarSuscripcion_DebeLanzarException_CuandoNoExiste()
        {
            // Arrange
            var suscripcionInexistente = new DetalleSuscripcion
            {
                UsuarioId = 999,
                tipo_suscripcion = "premium",
                operaciones_realizadas = 0
            };

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => 
                _repository.ActualizarSuscripcion(suscripcionInexistente)
            );
            Assert.That(ex.Message, Is.EqualTo("No se encontró una suscripción para actualizar."));
        }

        [Test]
        public void ObtenerPorUsuarioId_DebeIncluirUsuario()
        {
            // Act
            var suscripcion = _repository.ObtenerPorUsuarioId(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(suscripcion, Is.Not.Null);
                Assert.That(suscripcion!.Usuario, Is.Not.Null);
                Assert.That(suscripcion.Usuario!.Correo, Is.EqualTo("test@test.com"));
            });
        }
    }
}