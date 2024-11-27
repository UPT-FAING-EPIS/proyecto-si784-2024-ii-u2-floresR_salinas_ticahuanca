using NUnit.Framework;
using NegocioPDF.Models;
using System;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Models
{
    [TestFixture]
    public class ModelValidationTests
    {
        private PDFSolutionsContext _context;
        private IUsuarioRepository _usuarioRepository;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
                .Options;

            _context = new PDFSolutionsContext(options);
            _usuarioRepository = new UsuarioRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public void Usuario_NoDebePermitirDuplicados()
        {
            // Arrange
            var usuario1 = new Usuario
            {
                Nombre = "Test User 1",
                Correo = "test@test.com",
                Password = "test123"
            };

            var usuario2 = new Usuario
            {
                Nombre = "Test User 2",
                Correo = "test@test.com", // Mismo correo
                Password = "test456"
            };

            // Act & Assert
            _usuarioRepository.RegistrarUsuario(usuario1);
            
            var ex = Assert.Throws<InvalidOperationException>(() => 
                _usuarioRepository.RegistrarUsuario(usuario2)
            );
            Assert.That(ex.Message, Is.EqualTo("El correo electrónico ya está registrado."));
        }

        [Test]
        public void Usuario_NoDebePermitirDatosVacios()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "",
                Correo = "",
                Password = ""
            };

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => 
                _usuarioRepository.RegistrarUsuario(usuario)
            );
            Assert.That(ex.Message, Is.EqualTo("El nombre es requerido."));
        }

        [Test]
        public void Usuario_DebePermitirDatosValidos()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "test123"
            };

            // Act & Assert
            Assert.DoesNotThrow(() => _usuarioRepository.RegistrarUsuario(usuario));
            
            var usuarioGuardado = _context.Usuarios.FirstOrDefault();
            Assert.That(usuarioGuardado, Is.Not.Null);
            Assert.That(usuarioGuardado.Correo, Is.EqualTo("test@test.com"));
        }


        [Test]
        public void DetalleSuscripcion_DebeGuardarDatosValidos()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "test123"
            };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var detalleSuscripcion = new DetalleSuscripcion
            {
                tipo_suscripcion = "basico",
                operaciones_realizadas = 0,
                precio = 0.00m,
                UsuarioId = usuario.Id,
                fecha_inicio = DateTime.Now,
                fecha_final = DateTime.Now.AddYears(1)
            };

            // Act
            _context.DetallesSuscripcion.Add(detalleSuscripcion);
            _context.SaveChanges();

            // Assert
            var suscripcionGuardada = _context.DetallesSuscripcion.Include(d => d.Usuario).FirstOrDefault();
            Assert.That(suscripcionGuardada, Is.Not.Null);
            Assert.That(suscripcionGuardada.tipo_suscripcion, Is.EqualTo("basico"));
            Assert.That(suscripcionGuardada.Usuario, Is.Not.Null);
        }

        [Test]
        public void OperacionPDF_DebeGuardarOperacionValida()
        {
            // Arrange
            var usuario = new Usuario
            {
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "test123"
            };
            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            var operacion = new OperacionPDF
            {
                UsuarioId = usuario.Id,
                TipoOperacion = "Fusionar",
                FechaOperacion = DateTime.Now
            };

            // Act
            _context.OperacionesPDF.Add(operacion);
            _context.SaveChanges();

            // Assert
            var operacionGuardada = _context.OperacionesPDF.FirstOrDefault();
            Assert.That(operacionGuardada, Is.Not.Null);
            Assert.That(operacionGuardada.TipoOperacion, Is.EqualTo("Fusionar"));
        }
    }
}