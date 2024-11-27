using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;
using System.Transactions;

namespace NegocioPDF.Tests.Repositories
{
    [TestFixture]
    public class OperacionesPDFRepositoryDetailedTests
    {
        private PDFSolutionsContext _context = null!;
        private IOperacionesPDFRepository _operacionesRepo = null!;
        private IDetalleSuscripcionRepository _suscripcionRepo = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new PDFSolutionsContext(options);
            _operacionesRepo = new OperacionesPDFRepository(_context);
            _suscripcionRepo = new DetalleSuscripcionRepository(_context);

            // Configurar datos de prueba
            ConfigurarDatosPrueba();
        }

        private void ConfigurarDatosPrueba()
        {
            // Usuario básico
            var usuarioBasico = new Usuario
            {
                Id = 1,
                Nombre = "Usuario Básico",
                Correo = "basico@test.com",
                Password = "password123"
            };
            _context.Usuarios.Add(usuarioBasico);

            // Usuario premium
            var usuarioPremium = new Usuario
            {
                Id = 2,
                Nombre = "Usuario Premium",
                Correo = "premium@test.com",
                Password = "password123"
            };
            _context.Usuarios.Add(usuarioPremium);
            _context.SaveChanges();

            // Suscripción básica
            var suscripcionBasica = new DetalleSuscripcion
            {
                UsuarioId = 1,
                tipo_suscripcion = "basico",
                operaciones_realizadas = 0,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddYears(1)
            };
            _context.DetallesSuscripcion.Add(suscripcionBasica);

            // Suscripción premium
            var suscripcionPremium = new DetalleSuscripcion
            {
                UsuarioId = 2,
                tipo_suscripcion = "premium",
                operaciones_realizadas = 0,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddYears(1)
            };
            _context.DetallesSuscripcion.Add(suscripcionPremium);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public void RegistrarOperacionPDF_DebeActualizarContador_ParaUsuarioBasico()
        {
            // Act
            var resultado = _operacionesRepo.RegistrarOperacionPDF(1, "Fusionar");
            var suscripcion = _suscripcionRepo.ObtenerPorUsuarioId(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True);
                Assert.That(suscripcion?.operaciones_realizadas, Is.EqualTo(1));
            });
        }

        [Test]
        public void RegistrarOperacionPDF_NoDebeActualizarContador_ParaUsuarioPremium()
        {
            // Act
            var resultado = _operacionesRepo.RegistrarOperacionPDF(2, "Fusionar");
            var suscripcion = _suscripcionRepo.ObtenerPorUsuarioId(2);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Is.True);
                Assert.That(suscripcion?.operaciones_realizadas, Is.EqualTo(0));
            });
        }

        [Test]
        public void ValidarOperacion_DebeRetornarTrue_ParaUsuarioPremium()
        {
            // Arrange
            for (int i = 0; i < 10; i++)
            {
                _operacionesRepo.RegistrarOperacionPDF(2, "Operacion");
            }

            // Act
            var resultado = _operacionesRepo.ValidarOperacion(2);

            // Assert
            Assert.That(resultado, Is.True);
        }

        [Test]
        public void ObtenerOperacionesPorUsuario_DebeRetornarListaOrdenada()
        {
            // Arrange
            var fechaBase = DateTime.Now;
            for (int i = 0; i < 3; i++)
            {
                _context.OperacionesPDF.Add(new OperacionPDF
                {
                    UsuarioId = 1,
                    TipoOperacion = $"Operacion{i}",
                    FechaOperacion = fechaBase.AddDays(-i)
                });
            }
            _context.SaveChanges();

            // Act
            var operaciones = _operacionesRepo.ObtenerOperacionesPorUsuario(1).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(operaciones, Has.Count.EqualTo(3));
                Assert.That(operaciones, Is.Ordered.By("FechaOperacion").Descending);
            });
        }

        [Test]
        public void ContarOperacionesRealizadas_DebeRetornarTotalCorrecto()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                _operacionesRepo.RegistrarOperacionPDF(1, $"Operacion{i}");
            }

            // Act
            var total = _operacionesRepo.ContarOperacionesRealizadas(1);
            var operaciones = _operacionesRepo.ObtenerOperacionesPorUsuario(1).ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(total, Is.EqualTo(5));
                Assert.That(operaciones, Has.Count.EqualTo(5));
            });
        }

        [Test]
        public void RegistrarOperacionPDF_DebeRetornarFalse_CuandoUsuarioNoExiste()
        {
            // Act
            var resultado = _operacionesRepo.RegistrarOperacionPDF(999, "Fusionar");

            // Assert
            Assert.That(resultado, Is.False);
        }

        [Test]
        public void ValidarOperacion_DebeRetornarFalse_CuandoSuscripcionNoExiste()
        {
            // Act
            var resultado = _operacionesRepo.ValidarOperacion(999);

            // Assert
            Assert.That(resultado, Is.False);
        }
    }
}