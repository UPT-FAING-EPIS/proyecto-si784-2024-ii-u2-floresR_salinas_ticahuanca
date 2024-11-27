using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

[TestFixture]
public class OperacionesPDFRepositoryTests
{
    private PDFSolutionsContext _context;
    private IOperacionesPDFRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
            .UseInMemoryDatabase(databaseName: "PDFSolutions_" + Guid.NewGuid().ToString())
            .Options;

        _context = new PDFSolutionsContext(options);
        _repository = new OperacionesPDFRepository(_context);

        // Datos de prueba base
        var usuario = new Usuario 
        { 
            Id = 1, 
            Nombre = "Test User",
            Correo = "test@test.com",
            Password = "123456"
        };
        _context.Usuarios.Add(usuario);

        var suscripcion = new DetalleSuscripcion
        {
            UsuarioId = 1,
            tipo_suscripcion = "basico",
            operaciones_realizadas = 0
        };
        _context.DetallesSuscripcion.Add(suscripcion);

        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public void RegistrarOperacionPDF_DebeRetornarTrue_CuandoUsuarioPuedeRealizarOperacion()
    {
        // Act
        var resultado = _repository.RegistrarOperacionPDF(1, "Fusionar");

        // Assert
        Assert.That(resultado, Is.True);
        Assert.That(_context.OperacionesPDF.Count(), Is.EqualTo(1));
        
        // Verificar que se incrementó el contador de operaciones
        var suscripcion = _context.DetallesSuscripcion.First(d => d.UsuarioId == 1);
        Assert.That(suscripcion.operaciones_realizadas, Is.EqualTo(1));
    }

    [Test]
    public void RegistrarOperacionPDF_DebeRetornarFalse_CuandoUsuarioAlcanzoLimite()
    {
        // Arrange
        var suscripcion = _context.DetallesSuscripcion.First();
        suscripcion.operaciones_realizadas = 5;
        _context.SaveChanges();

        // Act
        var resultado = _repository.RegistrarOperacionPDF(1, "Fusionar");

        // Assert
        Assert.That(resultado, Is.False);
        Assert.That(_context.OperacionesPDF.Count(), Is.EqualTo(0));
    }

    [Test]
    public void ValidarOperacion_DebeRetornarTrue_CuandoUsuarioPuedeRealizarOperacion()
    {
        // Act
        var resultado = _repository.ValidarOperacion(1);

        // Assert
        Assert.That(resultado, Is.True);
    }

    [Test]
    public void ValidarOperacion_DebeRetornarFalse_CuandoUsuarioAlcanzoLimite()
    {
        // Arrange
        var suscripcion = _context.DetallesSuscripcion.First();
        suscripcion.operaciones_realizadas = 5;
        _context.SaveChanges();

        // Act
        var resultado = _repository.ValidarOperacion(1);

        // Assert
        Assert.That(resultado, Is.False);
    }

    [Test]
    public void ContarOperacionesRealizadas_DebeRetornarNumeroCorrectamente()
    {
        // Arrange
        for (int i = 0; i < 3; i++)
        {
            _context.OperacionesPDF.Add(new OperacionPDF
            {
                UsuarioId = 1,
                TipoOperacion = "Fusionar",
                FechaOperacion = DateTime.Now
            });
        }
        _context.SaveChanges();

        // Act
        var resultado = _repository.ContarOperacionesRealizadas(1);

        // Assert
        Assert.That(resultado, Is.EqualTo(3));
    }
}