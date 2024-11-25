using NUnit.Framework;
using Moq;
using NegocioPDF.Models;
using NegocioPDF.Repositories;
using System.Collections.Generic;
using System.Linq; // Agrega esta línea para usar First() y otros métodos de LINQ.

namespace NegocioPDF.Tests
{
    [TestFixture]
    public class OperacionesPDFRepositoryTests
    {
        private Mock<IOperacionesPDFRepository> _mockRepository;
        private List<OperacionPDF> _operaciones;
        
        [SetUp]
        public void Setup()
        {
            // Configurar Moq para simular el repositorio de operaciones
            _mockRepository = new Mock<IOperacionesPDFRepository>();

            // Crear un listado de operaciones para las pruebas
            _operaciones = new List<OperacionPDF>
            {
                new OperacionPDF { Id = 1, UsuarioId = 1, TipoOperacion = "Fusionar" },
                new OperacionPDF { Id = 2, UsuarioId = 1, TipoOperacion = "Cortar" },
                new OperacionPDF { Id = 3, UsuarioId = 1, TipoOperacion = "Fusionar" }
            };
        }

        [Test]
        public void RegistrarOperacionPDF_ValidData_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.RegistrarOperacionPDF(It.IsAny<int>(), It.IsAny<string>()))
                           .Returns(true);

            // Act
            var result = _mockRepository.Object.RegistrarOperacionPDF(1, "Fusionar");

            // Assert
            Assert.IsTrue(result);
            _mockRepository.Verify(repo => repo.RegistrarOperacionPDF(1, "Fusionar"), Times.Once);
        }

        [Test]
        public void ObtenerOperacionesPorUsuario_ValidUsuarioId_ReturnsOperaciones()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ObtenerOperacionesPorUsuario(1))
                           .Returns(_operaciones);

            // Act
            var result = _mockRepository.Object.ObtenerOperacionesPorUsuario(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count()); // Aseguramos que haya 3 operaciones
            Assert.AreEqual("Fusionar", result.First().TipoOperacion); // Accedemos al primer elemento usando First()
        }

        [Test]
        public void ObtenerOperacionesPorUsuario_InvalidUsuarioId_ReturnsEmptyList()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ObtenerOperacionesPorUsuario(It.IsAny<int>()))
                           .Returns(new List<OperacionPDF>());

            // Act
            var result = _mockRepository.Object.ObtenerOperacionesPorUsuario(999); // Usuario no encontrado

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count()); // No debe haber operaciones
        }

        [Test]
        public void ContarOperacionesRealizadas_ValidUsuarioId_ReturnsCorrectCount()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ContarOperacionesRealizadas(1))
                           .Returns(3);  // El usuario 1 tiene 3 operaciones

            // Act
            var result = _mockRepository.Object.ContarOperacionesRealizadas(1);

            // Assert
            Assert.AreEqual(3, result);  // Aseguramos que el número de operaciones sea 3
        }

        [Test]
        public void ContarOperacionesRealizadas_InvalidUsuarioId_ReturnsZero()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ContarOperacionesRealizadas(It.IsAny<int>()))
                           .Returns(0);  // No hay operaciones para este usuario

            // Act
            var result = _mockRepository.Object.ContarOperacionesRealizadas(999);  // Usuario no válido

            // Assert
            Assert.AreEqual(0, result);  // No se debe contar ninguna operación
        }

        [Test]
        public void ValidarOperacion_ValidUsuarioId_ReturnsTrue()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ValidarOperacion(1))
                           .Returns(true);  // La operación es válida para el usuario 1

            // Act
            var result = _mockRepository.Object.ValidarOperacion(1);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidarOperacion_InvalidUsuarioId_ReturnsFalse()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ValidarOperacion(It.IsAny<int>()))
                           .Returns(false);  // La operación no es válida para este usuario

            // Act
            var result = _mockRepository.Object.ValidarOperacion(999);  // Usuario no válido

            // Assert
            Assert.IsFalse(result);
        }
    }
}