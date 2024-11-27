using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Repositories
{
    [TestFixture]
    public class OperacionesPDFRepositoryExtendedTests
    {
        private PDFSolutionsContext _context;
        private IOperacionesPDFRepository _repository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new PDFSolutionsContext(options);
            _repository = new OperacionesPDFRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public void ValidarOperacion_DebeRetornarFalse_CuandoNoExisteSuscripcion()
        {
            // Act
            var resultado = _repository.ValidarOperacion(999);

            // Assert
            Assert.That(resultado, Is.False);
        }

        [Test]
        public void RegistrarOperacionPDF_DebeRetornarFalse_CuandoUsuarioNoExiste()
        {
            // Act
            var resultado = _repository.RegistrarOperacionPDF(999, "Fusionar");

            // Assert
            Assert.That(resultado, Is.False);
        }

        [Test]
        public void ObtenerOperacionesPorUsuario_DebeRetornarOperacionesOrdenadas()
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

            var fechaBase = DateTime.Now;
            var operaciones = new[]
            {
                new OperacionPDF { UsuarioId = usuario.Id, TipoOperacion = "Fusionar", FechaOperacion = fechaBase.AddDays(-2) },
                new OperacionPDF { UsuarioId = usuario.Id, TipoOperacion = "Dividir", FechaOperacion = fechaBase.AddDays(-1) },
                new OperacionPDF { UsuarioId = usuario.Id, TipoOperacion = "Comprimir", FechaOperacion = fechaBase }
            };

            _context.OperacionesPDF.AddRange(operaciones);
            _context.SaveChanges();

            // Act
            var operacionesObtenidas = _repository.ObtenerOperacionesPorUsuario(usuario.Id).ToList();

            // Assert
            Assert.That(operacionesObtenidas.Count, Is.EqualTo(3));
            Assert.That(operacionesObtenidas[0].FechaOperacion, Is.GreaterThanOrEqualTo(operacionesObtenidas[1].FechaOperacion));
            Assert.That(operacionesObtenidas[1].FechaOperacion, Is.GreaterThanOrEqualTo(operacionesObtenidas[2].FechaOperacion));
        }
    }
}