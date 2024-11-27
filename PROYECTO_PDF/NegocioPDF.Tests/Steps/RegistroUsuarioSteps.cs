using TechTalk.SpecFlow;
using NUnit.Framework;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Steps
{
    [Binding]
    public class RegistroUsuarioSteps
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private Usuario _usuario = null!;
        private string _error = string.Empty;

        public RegistroUsuarioSteps(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            
            // Agregar un usuario inicial para pruebas de duplicados
            var usuarioInicial = new Usuario
            {
                Nombre = "Usuario Inicial",
                Correo = "test@test.com",
                Password = "password123"
            };
            try
            {
                _usuarioRepository.RegistrarUsuario(usuarioInicial);
            }
            catch { }
        }

        [Given(@"un nuevo usuario con nombre ""(.*)"", correo ""(.*)"" y contraseña ""(.*)""")]
        public void GivenUnNuevoUsuarioConNombreCorreoYContrasena(string nombre, string correo, string password)
        {
            _usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Password = password
            };
        }

        [When(@"intento registrar el usuario")]
        public void WhenIntentoRegistrarElUsuario()
        {
            try
            {
                _usuarioRepository.RegistrarUsuario(_usuario);
                _error = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                _error = ex.Message;
            }
            catch (Exception ex)
            {
                _error = ex.Message;
            }
        }

        [Then(@"el usuario debería registrarse correctamente")]
        public void ThenElUsuarioDeberiaRegistrarseCorrectamente()
        {
            Assert.That(_error, Is.Empty);
            var usuarioRegistrado = _usuarioRepository.Login(_usuario.Correo, _usuario.Password);
            Assert.That(usuarioRegistrado, Is.Not.Null);
        }

        [Then(@"debería ver un mensaje de error de registro")]
        public void ThenDeberiaVerUnMensajeDeErrorDeRegistro()
        {
            Assert.That(_error, Is.Not.Empty);
            Console.WriteLine($"Error de registro: {_error}"); // Para debug
        }
    }
}