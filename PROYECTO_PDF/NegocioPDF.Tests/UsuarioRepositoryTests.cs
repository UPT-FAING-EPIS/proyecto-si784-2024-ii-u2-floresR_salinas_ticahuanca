using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

[TestFixture]
public class UsuarioRepositoryTests
{
    private PDFSolutionsContext _context;
    private IUsuarioRepository _repository;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
            .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
            .Options;

        _context = new PDFSolutionsContext(options);
        _repository = new UsuarioRepository(_context);

        // Datos de prueba
        var usuario = new Usuario
        {
            Id = 1,
            Nombre = "Test User",
            Correo = "test@test.com",
            Password = "password123"
        };
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public void Login_DebeRetornarUsuario_CuandoCredencialesSonCorrectas()
    {
        // Act
        var resultado = _repository.Login("test@test.com", "password123");

        // Assert
        Assert.That(resultado, Is.Not.Null);
        Assert.That(resultado.Correo, Is.EqualTo("test@test.com"));
    }

    [Test]
    public void Login_DebeRetornarNull_CuandoCredencialesSonIncorrectas()
    {
        // Act
        var resultado = _repository.Login("test@test.com", "wrongpassword");

        // Assert
        Assert.That(resultado, Is.Null);
    }

    [Test]
    public void RegistrarUsuario_DebeLanzarException_CuandoCorreoYaExiste()
    {
        // Arrange
        var nuevoUsuario = new Usuario
        {
            Nombre = "Otro Usuario",
            Correo = "test@test.com", // Correo que ya existe
            Password = "otropassword"
        };

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _repository.RegistrarUsuario(nuevoUsuario));
    }

    [Test]
    public void RegistrarUsuario_DebeCrearSuscripcionBasica()
    {
        // Arrange
        var nuevoUsuario = new Usuario
        {
            Nombre = "Nuevo Usuario",
            Correo = "nuevo@test.com",
            Password = "password123"
        };

        // Act
        _repository.RegistrarUsuario(nuevoUsuario);

        // Assert
        var suscripcion = _context.DetallesSuscripcion
            .FirstOrDefault(d => d.UsuarioId == nuevoUsuario.Id);
        
        Assert.That(suscripcion, Is.Not.Null);
        Assert.That(suscripcion.tipo_suscripcion, Is.EqualTo("basico"));
        Assert.That(suscripcion.operaciones_realizadas, Is.EqualTo(0));
        Assert.That(suscripcion.precio, Is.EqualTo(0.00m));
    }

    [Test]
    public void ObtenerUsuarioPorId_DebeRetornarNull_CuandoUsuarioNoExiste()
    {
        // Act
        var resultado = _repository.ObtenerUsuarioPorId(999);

        // Assert
        Assert.That(resultado, Is.Null);
    }

    [Test]
    public void ObtenerUsuarios_DebeRetornarTodosLosUsuarios()
    {
        // Arrange
        _context.Usuarios.Add(new Usuario
        {
            Nombre = "Otro Usuario",
            Correo = "otro@test.com",
            Password = "password123"
        });
        _context.SaveChanges();

        // Act
        var usuarios = _repository.ObtenerUsuarios();

        // Assert
        Assert.That(usuarios.Count(), Is.EqualTo(2));
    }
}