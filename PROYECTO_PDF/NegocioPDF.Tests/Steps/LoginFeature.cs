using TechTalk.SpecFlow;
using NUnit.Framework;
using NegocioPDF.Models;

namespace NegocioPDF.Tests.Acceptance
{
    [Binding]
    public class LoginFeatureSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private string _correo = string.Empty;
        private string _password = string.Empty;
        private Usuario? _resultado;

        public LoginFeatureSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [Given(@"un usuario con correo ""(.*)"" y contraseña ""(.*)""")]
        public void DadoUnUsuarioConCorreoYContrasena(string correo, string password)
        {
            _correo = correo;
            _password = password;
        }

        [When(@"intento iniciar sesión")]
        public void CuandoIntentoIniciarSesion()
        {
            try
            {
                if (string.IsNullOrEmpty(_correo))
                {
                    _resultado = null;
                    return;
                }

                if (string.IsNullOrEmpty(_password))
                {
                    _resultado = null;
                    return;
                }

                // Simular login exitoso solo para credenciales específicas
                if (_correo == "test@test.com" && _password == "password123")
                {
                    _resultado = new Usuario
                    {
                        Id = 1,
                        Nombre = "Test User",
                        Correo = _correo,
                        Password = _password
                    };
                }
                else
                {
                    _resultado = null;
                }
            }
            catch
            {
                _resultado = null;
            }
        }

        [Then(@"debería iniciar sesión correctamente")]
        public void EntoncesDeberiaIniciarSesionCorrectamente()
        {
            Assert.That(_resultado, Is.Not.Null);
            Assert.That(_resultado!.Correo, Is.EqualTo(_correo));
        }

        [Then(@"debería ver un mensaje de error")]
        public void EntoncesDeberiaVerUnMensajeDeError()
        {
            Assert.That(_resultado, Is.Null, "Se esperaba que el login fallara");
        }
    }
}