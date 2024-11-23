using NUnit.Framework;
using Moq;
using NegocioPDF.Models;
using NegocioPDF.Repositories;
using System;

namespace NegocioPDF.Tests
{
    [TestFixture]
    public class DetalleSuscripcionRepositoryTests
    {
        private Mock<IDetalleSuscripcionRepository> _mockRepository;
        private DetalleSuscripcion _detalleSuscripcion;

        [SetUp]
        public void Setup()
        {
            // Configurar Moq para simular un repositorio
            _mockRepository = new Mock<IDetalleSuscripcionRepository>();
            
            // Crear un objeto DetalleSuscripcion para las pruebas
            _detalleSuscripcion = new DetalleSuscripcion
            {
                Id = 1,
                tipo_suscripcion = "Premium",
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddMonths(1),
                precio = 29.99m,
                operaciones_realizadas = 10,
                UsuarioId = 1
            };
        }

        [Test]
        public void ObtenerPorUsuarioId_ValidUsuarioId_ReturnsDetalleSuscripcion()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ObtenerPorUsuarioId(1))
                           .Returns(_detalleSuscripcion);

            // Act
            var result = _mockRepository.Object.ObtenerPorUsuarioId(1);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Premium", result.tipo_suscripcion);
            Assert.AreEqual(29.99m, result.precio);
        }

        [Test]
        public void ObtenerPorUsuarioId_InvalidUsuarioId_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ObtenerPorUsuarioId(It.IsAny<int>()))
                           .Returns((DetalleSuscripcion)null);

            // Act
            var result = _mockRepository.Object.ObtenerPorUsuarioId(999); // ID no válido

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void ActualizarSuscripcion_ValidData_UpdatesSuscripcion()
        {
            // Arrange
            var updatedDetalle = new DetalleSuscripcion
            {
                Id = 1,
                tipo_suscripcion = "Basic",
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddMonths(1),
                precio = 19.99m,
                operaciones_realizadas = 5,
                UsuarioId = 1
            };

            _mockRepository.Setup(repo => repo.ActualizarSuscripcion(updatedDetalle));

            // Act & Assert
            TestDelegate testDelegate = () => _mockRepository.Object.ActualizarSuscripcion(updatedDetalle);
            Assert.DoesNotThrow(testDelegate);

            // Verify that the method was called once with the expected parameter
            _mockRepository.Verify(repo => repo.ActualizarSuscripcion(updatedDetalle), Times.Once);
        }

        [Test]
        public void ActualizarSuscripcion_InvalidData_ThrowsException()
        {
            // Arrange
            var invalidDetalle = new DetalleSuscripcion
            {
                Id = 0,  // ID no válido
                tipo_suscripcion = "Basic",
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddMonths(1),
                precio = null,  // Precio no válido
                operaciones_realizadas = 5,
                UsuarioId = 1
            };

            _mockRepository.Setup(repo => repo.ActualizarSuscripcion(invalidDetalle))
                           .Throws<ArgumentException>();

            // Act & Assert
            TestDelegate testDelegate = () => _mockRepository.Object.ActualizarSuscripcion(invalidDetalle);
            Assert.Throws<ArgumentException>(testDelegate);
        }
    }
}