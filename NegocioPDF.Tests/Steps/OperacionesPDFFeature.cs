using TechTalk.SpecFlow;
using NUnit.Framework;
using NegocioPDF.Models;
using NegocioPDF.Repositories;
using Moq;

namespace NegocioPDF.Tests.Steps
{
    [Binding]
    public class OperacionesPDFFeatureSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private bool _resultado;
        private int _operacionesRealizadas;
        private string _tipoSuscripcion;

        public OperacionesPDFFeatureSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _operacionesRealizadas = 0;
        }

        [Given(@"un usuario con id (.*) y suscripción ""(.*)""")]
        public void DadoUnUsuarioConSuscripcion(int usuarioId, string tipoSuscripcion)
        {
            _scenarioContext["usuarioId"] = usuarioId;
            _tipoSuscripcion = tipoSuscripcion;
        }

        [When(@"intento realizar una operación ""(.*)""")]
        public void CuandoIntentoRealizarUnaOperacion(string tipoOperacion)
        {
            var usuarioId = _scenarioContext.Get<int>("usuarioId");
            
            // Simular lógica de validación de operaciones
            if (_tipoSuscripcion == "basico" && _operacionesRealizadas >= 5)
            {
                _resultado = false;
            }
            else
            {
                _resultado = true;
                _operacionesRealizadas++;
            }
        }

        [Then(@"la operación debería realizarse correctamente")]
        public void EntoncesLaOperacionDeberiaRealizarseCorrectamente()
        {
            Assert.That(_resultado, Is.True);
        }

        [Then(@"la operación debería ser rechazada")]
        public void EntoncesLaOperacionDeberiaSerRechazada()
        {
            Assert.That(_resultado, Is.False);
        }
    }
}