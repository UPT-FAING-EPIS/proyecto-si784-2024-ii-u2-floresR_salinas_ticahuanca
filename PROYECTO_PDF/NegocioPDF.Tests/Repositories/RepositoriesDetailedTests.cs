using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Repositories
{
    [TestFixture]
    public class RepositoriesDetailedTests
    {
        private PDFSolutionsContext _context = null!;
        private IDetalleSuscripcionRepository _detalleSuscripcionRepo = null!;
        private IOperacionesPDFRepository _operacionesPDFRepo = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new PDFSolutionsContext(options);
            _detalleSuscripcionRepo = new DetalleSuscripcionRepository(_context);
            _operacionesPDFRepo = new OperacionesPDFRepository(_context);

            // Datos base para pruebas
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
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public void DetalleSuscripcion_DebeActualizarOperacionesRealizadas()
        {
            // Arrange
            var suscripcion = _detalleSuscripcionRepo.ObtenerPorUsuarioId(1);
            Assert.That(suscripcion, Is.Not.Null, "La suscripción debe existir");

            // Act
            suscripcion!.operaciones_realizadas = 5;
            _detalleSuscripcionRepo.ActualizarSuscripcion(suscripcion);

            // Assert
            var suscripcionActualizada = _detalleSuscripcionRepo.ObtenerPorUsuarioId(1);
            Assert.That(suscripcionActualizada!.operaciones_realizadas, Is.EqualTo(5));
        }

        [Test]
        public void OperacionesPDF_DebeContarOperacionesCorrectamente()
        {
            // Arrange
            _operacionesPDFRepo.RegistrarOperacionPDF(1, "Fusionar");
            _operacionesPDFRepo.RegistrarOperacionPDF(1, "Dividir");
            _operacionesPDFRepo.RegistrarOperacionPDF(1, "Comprimir");

            // Act
            var cantidad = _operacionesPDFRepo.ContarOperacionesRealizadas(1);

            // Assert
            Assert.That(cantidad, Is.EqualTo(3));
        }

        [Test]
        public void OperacionesPDF_DebeValidarLimiteOperaciones()
        {
            // Arrange
            var suscripcion = _detalleSuscripcionRepo.ObtenerPorUsuarioId(1);
            Assert.That(suscripcion, Is.Not.Null);
            suscripcion!.operaciones_realizadas = 5;
            _detalleSuscripcionRepo.ActualizarSuscripcion(suscripcion);

            // Act
            var resultado = _operacionesPDFRepo.ValidarOperacion(1);

            // Assert
            Assert.That(resultado, Is.False);
        }

        [Test]
        public void OperacionesPDF_DebePermitirOperacionesIlimitadasEnPremium()
        {
            // Arrange
            var suscripcion = _detalleSuscripcionRepo.ObtenerPorUsuarioId(1);
            Assert.That(suscripcion, Is.Not.Null);
            suscripcion!.tipo_suscripcion = "premium";
            _detalleSuscripcionRepo.ActualizarSuscripcion(suscripcion);

            // Act & Assert
            for (int i = 0; i < 10; i++)
            {
                var resultado = _operacionesPDFRepo.RegistrarOperacionPDF(1, $"Operacion{i}");
                Assert.That(resultado, Is.True);
            }
        }

        [Test]
        public void DetalleSuscripcion_DebeValidarFechasCorrectas()
        {
            // Arrange
            var suscripcion = _detalleSuscripcionRepo.ObtenerPorUsuarioId(1);
            Assert.That(suscripcion, Is.Not.Null);

            // Act
            var nuevaFechaInicio = DateTime.Now.AddDays(-30);
            var nuevaFechaFinal = nuevaFechaInicio.AddYears(1);
            suscripcion!.fecha_inicio = nuevaFechaInicio;
            suscripcion.fecha_final = nuevaFechaFinal;
            _detalleSuscripcionRepo.ActualizarSuscripcion(suscripcion);

            // Assert
            var suscripcionActualizada = _detalleSuscripcionRepo.ObtenerPorUsuarioId(1);
            Assert.Multiple(() =>
            {
                Assert.That(suscripcionActualizada!.fecha_inicio!.Value.Date, Is.EqualTo(nuevaFechaInicio.Date));
                Assert.That(suscripcionActualizada.fecha_final!.Value.Date, Is.EqualTo(nuevaFechaFinal.Date));
            });
        }
    }
}