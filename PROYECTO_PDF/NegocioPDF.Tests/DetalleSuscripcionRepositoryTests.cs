using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;
namespace NegocioPDF.Tests
{
[TestFixture]
public class DetalleSuscripcionRepositoryTests
{
    private PDFSolutionsContext _context;
    private IDetalleSuscripcionRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
            .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
            .Options;

        _context = new PDFSolutionsContext(options);
        _repository = new DetalleSuscripcionRepository(_context);

        // Datos de prueba
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Test User",
            Correo = "test@test.com",
            Password = "password123"
        };
        _context.Usuarios.Add(usuario);

        var suscripcion = new DetalleSuscripcion
        {
            UsuarioId = 1,
            tipo_suscripcion = "basico",
            operaciones_realizadas = 0,
            fecha_inicio = DateTime.Now,
            fecha_final = DateTime.Now.AddMonths(1),
            precio = 0.00m,
            Usuario = usuario
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
    public void ObtenerPorUsuarioId_DebeRetornarNull_CuandoUsuarioNoExiste()
    {
        // Act
        var resultado = _repository.ObtenerPorUsuarioId(999);

        // Assert
        Assert.That(resultado, Is.Null);
    }

    [Test]
    public void ObtenerPorUsuarioId_DebeRetornarSuscripcion_CuandoExisteUsuario()
    {
        // Act
        var result = _repository.ObtenerPorUsuarioId(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.tipo_suscripcion, Is.EqualTo("basico"));
        Assert.That(result.Usuario, Is.Not.Null);
        Assert.That(result.Usuario.Correo, Is.EqualTo("test@test.com"));
    }

    [Test]
    public void ActualizarSuscripcion_DebeLanzarException_CuandoSuscripcionNoExiste()
    {
        // Arrange
        var suscripcionInexistente = new DetalleSuscripcion
        {
            UsuarioId = 999,
            tipo_suscripcion = "premium"
        };

        // Act & Assert
        Assert.Throws<Exception>(() => _repository.ActualizarSuscripcion(suscripcionInexistente));
    }

    [Test]
    public void ActualizarSuscripcion_DebeActualizarTodosLosCampos()
    {
        // Arrange
        var suscripcion = _repository.ObtenerPorUsuarioId(1);
        var nuevaFecha = DateTime.Now.AddYears(1);
        
        suscripcion.tipo_suscripcion = "premium";
        suscripcion.operaciones_realizadas = 10;
        suscripcion.fecha_final = nuevaFecha;
        suscripcion.precio = 99.99m;

        // Act
        _repository.ActualizarSuscripcion(suscripcion);

        // Assert
        var suscripcionActualizada = _repository.ObtenerPorUsuarioId(1);
        Assert.That(suscripcionActualizada.tipo_suscripcion, Is.EqualTo("premium"));
        Assert.That(suscripcionActualizada.operaciones_realizadas, Is.EqualTo(10));
        Assert.That(suscripcionActualizada.fecha_final, Is.EqualTo(nuevaFecha));
        Assert.That(suscripcionActualizada.precio, Is.EqualTo(99.99m));
    }
}
}