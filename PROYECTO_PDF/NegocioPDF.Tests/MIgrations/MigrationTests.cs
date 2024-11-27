using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;

namespace NegocioPDF.Tests.Migrations
{
    [TestFixture]
    public class MigrationTests
    {
        private PDFSolutionsContext _context = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: $"TestDB_{Guid.NewGuid()}")
                .Options;

            _context = new PDFSolutionsContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void ModelConfig_DebePermitirCrearUsuario()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "password123"
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();
            });
        }

        [Test]
        public void ModelConfig_DebePermitirCrearDetalleSuscripcion()
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

            var detalleSuscripcion = new DetalleSuscripcion
            {
                UsuarioId = usuario.Id,
                tipo_suscripcion = "basico",
                operaciones_realizadas = 0,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddMonths(1),
                precio = 0.00m
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _context.DetallesSuscripcion.Add(detalleSuscripcion);
                _context.SaveChanges();
            });
        }

        [Test]
        public void ModelConfig_DebePermitirCrearOperacionPDF()
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

            var operacion = new OperacionPDF
            {
                UsuarioId = usuario.Id,
                TipoOperacion = "Test",
                FechaOperacion = DateTime.Now
            };

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                _context.OperacionesPDF.Add(operacion);
                _context.SaveChanges();
            });
        }

        [Test]
        public void ModelConfig_DebeValidarRelacionesEntreEntidades()
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

            // Act & Assert
            Assert.Multiple(() =>
            {
                Assert.DoesNotThrow(() =>
                {
                    var suscripcion = new DetalleSuscripcion
                    {
                        UsuarioId = usuario.Id,
                        tipo_suscripcion = "basico",
                        operaciones_realizadas = 0
                    };
                    _context.DetallesSuscripcion.Add(suscripcion);
                    _context.SaveChanges();
                });

                var suscripcionGuardada = _context.DetallesSuscripcion
                    .Include(d => d.Usuario)
                    .FirstOrDefault();
                Assert.That(suscripcionGuardada?.Usuario?.Correo, Is.EqualTo("test@test.com"));
            });
        }

        [Test]
        public void ModelConfig_DebeEliminarEnCascada()
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
                TipoOperacion = "Test",
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