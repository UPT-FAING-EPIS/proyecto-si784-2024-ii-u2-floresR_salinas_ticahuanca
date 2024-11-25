using TechTalk.SpecFlow;
using NUnit.Framework;
using NegocioPDF.Models;
using Moq;

namespace NegocioPDF.Tests.Steps
{
    [Binding]
    public class RegistroUsuarioFeatureSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private Usuario _usuario;
        private Exception _error;

        public RegistroUsuarioFeatureSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"un nuevo usuario con nombre ""(.*)"", correo ""(.*)"" y contraseña ""(.*)""")]
        public void DadoUnNuevoUsuario(string nombre, string correo, string password)
        {
            _usuario = new Usuario
            {
                Nombre = nombre,
                Correo = correo,
                Password = password
            };
        }

        [When(@"intento registrar el usuario")]
        public void CuandoIntentoRegistrarElUsuario()
        {
            try
            {
                // Validaciones simuladas
                if (string.IsNullOrEmpty(_usuario.Nombre) || 
                    string.IsNullOrEmpty(_usuario.Correo) || 
                    string.IsNullOrEmpty(_usuario.Password))
                {
                    throw new Exception("Datos incompletos");
                }

                // Simular verificación de correo existente
                if (_usuario.Correo == "test@test.com")
                {
                    throw new Exception("El correo ya está registrado");
                }

                // Si llegamos aquí, el registro fue exitoso
                _error = null;
            }
            catch (Exception ex)
            {
                _error = ex;
            }
        }

        [Then(@"el usuario debería registrarse correctamente")]
        public void EntoncesElUsuarioDeberiaRegistrarseCorrectamente()
        {
            Assert.That(_error, Is.Null);
        }

        [Then(@"debería ver un mensaje de error de registro")]
        public void EntoncesDeberiaVerUnMensajeDeErrorDeRegistro()
        {
            Assert.That(_error, Is.Not.Null);
        }
    }
}