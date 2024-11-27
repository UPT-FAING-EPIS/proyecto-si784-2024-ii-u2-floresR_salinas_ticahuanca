using TechTalk.SpecFlow;
using NUnit.Framework;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

namespace NegocioPDF.Tests.Steps
{
    [Binding]
    public class OperacionesPDFFeature
    {
        private string _tipoSuscripcion = string.Empty;
        private readonly IOperacionesPDFRepository _operacionesRepository;
        private readonly IDetalleSuscripcionRepository _suscripcionRepository;
        private bool _resultadoOperacion;

        public OperacionesPDFFeature(
            IOperacionesPDFRepository operacionesRepository,
            IDetalleSuscripcionRepository suscripcionRepository)
        {
            _operacionesRepository = operacionesRepository;
            _suscripcionRepository = suscripcionRepository;
        }

        [Given(@"un usuario con suscripción ""(.*)""")]
        public void DadoUnUsuarioConSuscripcion(string tipoSuscripcion)
        {
            _tipoSuscripcion = tipoSuscripcion;
        }

        [When(@"intento realizar una operación PDF")]
        public void CuandoIntentoRealizarUnaOperacionPDF()
        {
            var usuarioId = 1; // Usuario de prueba
            _resultadoOperacion = _operacionesRepository.RegistrarOperacionPDF(usuarioId, "Fusionar");
        }

        [Then(@"la operación debe ser exitosa")]
        public void EntoncesLaOperacionDebeSerExitosa()
        {
            Assert.That(_resultadoOperacion, Is.True);
        }

        [Then(@"la operación debe ser rechazada")]
        public void EntoncesLaOperacionDebeSerRechazada()
        {
            Assert.That(_resultadoOperacion, Is.False);
        }
    }
}