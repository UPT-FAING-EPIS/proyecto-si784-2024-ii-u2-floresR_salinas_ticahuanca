using NUnit.Framework;
using NegocioPDF.Models;
using System;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;

namespace NegocioPDF.Tests.Models
{
    [TestFixture]
    public class ModelsDetailedTests
    {
        [Test]
        public void DetalleSuscripcion_PropiedadesDebenFuncionarCorrectamente()
        {
            // Arrange
            var fechaInicio = DateTime.Now;
            var fechaFinal = fechaInicio.AddYears(1);
            var detalleSuscripcion = new DetalleSuscripcion
            {
                tipo_suscripcion = "premium",
                fecha_inicio = fechaInicio,
                fecha_final = fechaFinal,
                precio = 99.99m,
                operaciones_realizadas = 5,
                UsuarioId = 1,
                Usuario = new Usuario 
                { 
                    Id = 1, 
                    Nombre = "Test",
                    Correo = "test@test.com",
                    Password = "test123"
                }
            };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(detalleSuscripcion.tipo_suscripcion, Is.EqualTo("premium"));
                Assert.That(detalleSuscripcion.fecha_inicio, Is.EqualTo(fechaInicio));
                Assert.That(detalleSuscripcion.fecha_final, Is.EqualTo(fechaFinal));
                Assert.That(detalleSuscripcion.precio, Is.EqualTo(99.99m));
                Assert.That(detalleSuscripcion.operaciones_realizadas, Is.EqualTo(5));
                Assert.That(detalleSuscripcion.UsuarioId, Is.EqualTo(1));
                Assert.That(detalleSuscripcion.Usuario, Is.Not.Null);
            });
        }

        [Test]
        public void OperacionPDF_PropiedadesDebenFuncionarCorrectamente()
        {
            // Arrange
            var fechaOperacion = DateTime.Now;
            var operacion = new OperacionPDF
            {
                UsuarioId = 1,
                TipoOperacion = "Fusionar",
                FechaOperacion = fechaOperacion
            };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(operacion.UsuarioId, Is.EqualTo(1));
                Assert.That(operacion.TipoOperacion, Is.EqualTo("Fusionar"));
                Assert.That(operacion.FechaOperacion, Is.EqualTo(fechaOperacion));
            });
        }

        [Test]
        public void Usuario_PropiedadesDebenFuncionarCorrectamente()
        {
            // Arrange
            var usuario = new Usuario
            {
                Id = 1,
                Nombre = "Test User",
                Correo = "test@test.com",
                Password = "password123"
            };

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(usuario.Id, Is.EqualTo(1));
                Assert.That(usuario.Nombre, Is.EqualTo("Test User"));
                Assert.That(usuario.Correo, Is.EqualTo("test@test.com"));
                Assert.That(usuario.Password, Is.EqualTo("password123"));
            });
        }
    }
}