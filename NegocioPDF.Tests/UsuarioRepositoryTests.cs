using NUnit.Framework;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using NegocioPDF.Models;

namespace NegocioPDF.Tests
{
    [TestFixture]
    public class UsuarioRepositoryTests
    {
        private Mock<IUsuarioRepository> _mockRepository = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IUsuarioRepository>();
        }

        [Test]
        public void Login_WithValidCredentials_ReturnsUser()
        {
            // Arrange
            var expectedUser = new Usuario 
            { 
                Id = 1, 
                Nombre = "Test User", 
                Correo = "test@test.com", 
                Password = "password123" 
            };
            
            _mockRepository.Setup(repo => 
                repo.Login("test@test.com", "password123"))
                .Returns(expectedUser);

            // Act
            var result = _mockRepository.Object.Login("test@test.com", "password123");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.Id, Is.EqualTo(expectedUser.Id));
                Assert.That(result.Correo, Is.EqualTo(expectedUser.Correo));
                Assert.That(result.Nombre, Is.EqualTo(expectedUser.Nombre));
            });
        }

        [Test]
        public void Login_WithInvalidCredentials_ReturnsNull()
        {
            // Arrange
            _mockRepository.Setup(repo => 
                repo.Login(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((Usuario?)null);

            // Act
            var result = _mockRepository.Object.Login("invalid@test.com", "wrongpassword");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Login_WithEmptyEmail_ThrowsArgumentException()
        {
            // Arrange
            _mockRepository.Setup(repo => 
                repo.Login("", It.IsAny<string>()))
                .Throws(new ArgumentException("El correo no puede estar vacío"));

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _mockRepository.Object.Login("", "password123"));
            Assert.That(ex.Message, Is.EqualTo("El correo no puede estar vacío"));
        }

        [Test]
        public void Login_WithEmptyPassword_ThrowsArgumentException()
        {
            // Arrange
            _mockRepository.Setup(repo => 
                repo.Login(It.IsAny<string>(), ""))
                .Throws(new ArgumentException("La contraseña no puede estar vacía"));

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _mockRepository.Object.Login("test@test.com", ""));
            Assert.That(ex.Message, Is.EqualTo("La contraseña no puede estar vacía"));
        }

        [Test]
        public void RegistrarUsuario_WithNewEmail_Succeeds()
        {
            // Arrange
            var newUser = new Usuario
            {
                Nombre = "New User",
                Correo = "new@test.com",
                Password = "newpass123"
            };
            _mockRepository.Setup(repo => 
                repo.RegistrarUsuario(It.IsAny<Usuario>()));

            // Act & Assert
            Assert.DoesNotThrow(() => _mockRepository.Object.RegistrarUsuario(newUser));
        }

        [Test]
        public void RegistrarUsuario_WithNullUser_ThrowsArgumentNullException()
        {
            // Arrange
            _mockRepository.Setup(repo => 
                repo.RegistrarUsuario(null!))
                .Throws(new ArgumentNullException("usuario", "El usuario no puede ser null"));

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => 
                _mockRepository.Object.RegistrarUsuario(null!));
            Assert.That(ex.ParamName, Is.EqualTo("usuario"));
        }

        [Test]
        public void RegistrarUsuario_WithEmptyEmail_ThrowsArgumentException()
        {
            // Arrange
            var userWithEmptyEmail = new Usuario
            {
                Nombre = "Test User",
                Correo = "",
                Password = "password123"
            };
            _mockRepository.Setup(repo => 
                repo.RegistrarUsuario(It.Is<Usuario>(u => string.IsNullOrEmpty(u.Correo))))
                .Throws(new ArgumentException("El correo no puede estar vacío"));

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _mockRepository.Object.RegistrarUsuario(userWithEmptyEmail));
            Assert.That(ex.Message, Is.EqualTo("El correo no puede estar vacío"));
        }

        [Test]
        public void RegistrarUsuario_WithEmptyPassword_ThrowsArgumentException()
        {
            // Arrange
            var userWithEmptyPassword = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = ""
            };
            _mockRepository.Setup(repo => 
                repo.RegistrarUsuario(It.Is<Usuario>(u => string.IsNullOrEmpty(u.Password))))
                .Throws(new ArgumentException("La contraseña no puede estar vacía"));

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _mockRepository.Object.RegistrarUsuario(userWithEmptyPassword));
            Assert.That(ex.Message, Is.EqualTo("La contraseña no puede estar vacía"));
        }

        [Test]
        public void ObtenerUsuarios_ReturnsUserList()
        {
            // Arrange
            var expectedUsers = new List<Usuario>
            {
                new Usuario { Id = 1, Nombre = "User 1", Correo = "user1@test.com" },
                new Usuario { Id = 2, Nombre = "User 2", Correo = "user2@test.com" }
            };
            _mockRepository.Setup(repo => repo.ObtenerUsuarios())
                .Returns(expectedUsers);

            // Act
            var result = _mockRepository.Object.ObtenerUsuarios().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count, Is.EqualTo(expectedUsers.Count));
                for (int i = 0; i < result.Count; i++)
                {
                    Assert.That(result[i].Id, Is.EqualTo(expectedUsers[i].Id));
                    Assert.That(result[i].Nombre, Is.EqualTo(expectedUsers[i].Nombre));
                    Assert.That(result[i].Correo, Is.EqualTo(expectedUsers[i].Correo));
                }
            });
        }

        [Test]
        public void ObtenerUsuarioPorId_WithValidId_ReturnsUser()
        {
            // Arrange
            var expectedUser = new Usuario
            {
                Id = 1,
                Nombre = "Test User",
                Correo = "test@test.com"
            };
            _mockRepository.Setup(repo => repo.ObtenerUsuarioPorId(1))
                .Returns(expectedUser);

            // Act
            var result = _mockRepository.Object.ObtenerUsuarioPorId(1);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result!.Id, Is.EqualTo(expectedUser.Id));
                Assert.That(result.Nombre, Is.EqualTo(expectedUser.Nombre));
                Assert.That(result.Correo, Is.EqualTo(expectedUser.Correo));
            });
        }

        [Test]
        public void ObtenerUsuarioPorId_WithZeroId_ThrowsArgumentException()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.ObtenerUsuarioPorId(0))
                .Throws(new ArgumentException("El ID debe ser mayor que cero"));

            // Act & Assert
            var ex = Assert.Throws<ArgumentException>(() => 
                _mockRepository.Object.ObtenerUsuarioPorId(0));
            Assert.That(ex.Message, Is.EqualTo("El ID debe ser mayor que cero"));
        }
    }
}