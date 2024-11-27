using TechTalk.SpecFlow;
using NUnit.Framework;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Steps
{
    [Binding]
    public class OperacionesPDFSteps
    {
        private readonly IOperacionesPDFRepository _operacionesRepository;
        private readonly IDetalleSuscripcionRepository _suscripcionRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private int _usuarioId;
        private string _tipoOperacion = string.Empty;
        private bool _resultadoOperacion;

        public OperacionesPDFSteps(
            IOperacionesPDFRepository operacionesRepository,
            IDetalleSuscripcionRepository suscripcionRepository,
            IUsuarioRepository usuarioRepository)
        {
            _operacionesRepository = operacionesRepository;
            _suscripcionRepository = suscripcionRepository;
            _usuarioRepository = usuarioRepository;
        }

        [Given(@"un usuario con id (.*) y suscripción ""(.*)""")]
        public void GivenUnUsuarioConIdYSuscripcion(int usuarioId, string tipoSuscripcion)
        {
            _usuarioId = usuarioId;

            // Crear usuario si no existe
            var usuario = _usuarioRepository.ObtenerUsuarioPorId(usuarioId);
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Id = usuarioId,
                    Nombre = $"Test User {usuarioId}",
                    Correo = $"test{usuarioId}@test.com",
                    Password = "password123"
                };
                _usuarioRepository.RegistrarUsuario(usuario);
            }

            // Actualizar suscripción
            var suscripcion = _suscripcionRepository.ObtenerPorUsuarioId(usuarioId);
            if (suscripcion != null)
            {
                suscripcion.tipo_suscripcion = tipoSuscripcion;
                _suscripcionRepository.ActualizarSuscripcion(suscripcion);
            }
        }

        [When(@"intento realizar una operación ""(.*)""")]
        public void WhenIntentoRealizarUnaOperacion(string tipoOperacion)
        {
            _tipoOperacion = tipoOperacion;
            _resultadoOperacion = _operacionesRepository.RegistrarOperacionPDF(_usuarioId, tipoOperacion);
        }

        [Then(@"la operación debería ser rechazada")]
        public void ThenLaOperacionDeberiaSerRechazada()
        {
            Assert.That(_resultadoOperacion, Is.False);
        }

        [Then(@"la operación debería realizarse correctamente")]
        public void ThenLaOperacionDeberiaRealizarseCorrectamente()
        {
            Assert.That(_resultadoOperacion, Is.True);
        }
    }
}