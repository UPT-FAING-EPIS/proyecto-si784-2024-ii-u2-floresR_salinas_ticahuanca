using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Models;
using NegocioPDF.Repositories;

[TestFixture]
public class UsuarioRepositoryExtendedTests
{
    private PDFSolutionsContext _context = null!;
    private IUsuarioRepository _repository = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
            .UseInMemoryDatabase(databaseName: "TestDB_" + Guid.NewGuid().ToString())
            .Options;

        _context = new PDFSolutionsContext(options);
        _repository = new UsuarioRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public void RegistrarUsuario_DebeCrearSuscripcionBasica()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nombre = "Test User",
            Correo = "test@test.com",
            Password = "password123"
        };

        // Act
        _repository.RegistrarUsuario(usuario);

        // Assert
        var suscripcion = _context.DetallesSuscripcion
            .FirstOrDefault(d => d.UsuarioId == usuario.Id);

        Assert.That(suscripcion, Is.Not.Null, "La suscripción no debería ser null");
        
        if (suscripcion != null)
        {
            Assert.Multiple(() =>
            {
                Assert.That(suscripcion.tipo_suscripcion, Is.EqualTo("basico"));
                Assert.That(suscripcion.operaciones_realizadas, Is.EqualTo(0));
                Assert.That(suscripcion.precio, Is.EqualTo(0.00m));
                Assert.That(suscripcion.fecha_inicio.HasValue, Is.True);
                Assert.That(suscripcion.fecha_final.HasValue, Is.True);
                Assert.That(suscripcion.fecha_final!.Value, Is.EqualTo(suscripcion.fecha_inicio!.Value.AddYears(1)).Within(TimeSpan.FromSeconds(1)));
            });
        }
    }

    [Test]
    public void RegistrarUsuario_DebeValidarNombreVacio()
    {
        // Arrange & Act & Assert
        Assert.Multiple(() =>
        {
            // Prueba con string vacío
            var usuario1 = new Usuario
            {
                Nombre = string.Empty,
                Correo = "test@test.com",
                Password = "password123"
            };
            var ex1 = Assert.Throws<InvalidOperationException>(() =>
                _repository.RegistrarUsuario(usuario1)
            );
            Assert.That(ex1.Message, Is.EqualTo("El nombre es requerido."));

            // Prueba con espacios en blanco
            var usuario2 = new Usuario
            {
                Nombre = "   ",
                Correo = "test@test.com",
                Password = "password123"
            };
            var ex2 = Assert.Throws<InvalidOperationException>(() =>
                _repository.RegistrarUsuario(usuario2)
            );
            Assert.That(ex2.Message, Is.EqualTo("El nombre es requerido."));
        });
    }

    [Test]
   public void ObtenerUsuarios_DebeRetornarListaCompleta()
        {
            // Arrange
            var usuarios = new[]
            {
                new Usuario { Nombre = "User B", Correo = "userb@test.com", Password = "pass123" },
                new Usuario { Nombre = "User A", Correo = "usera@test.com", Password = "pass123" },
                new Usuario { Nombre = "User C", Correo = "userc@test.com", Password = "pass123" }
            };

            foreach (var usuario in usuarios)
            {
                _repository.RegistrarUsuario(usuario);
            }

            // Act
            var resultado = _repository.ObtenerUsuarios().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(resultado, Has.Count.EqualTo(3));
                Assert.That(resultado.Select(u => u.Correo),
                    Is.EquivalentTo(usuarios.Select(u => u.Correo)));
            });
        }

    [Test]
    public void Login_DebeRetornarNull_CuandoCredencialesInvalidas()
    {
        // Arrange
        var usuario = new Usuario
        {
            Nombre = "Test User",
            Correo = "test@test.com",
            Password = "password123"
        };
        _repository.RegistrarUsuario(usuario);

        // Act & Assert
        Assert.Multiple(() =>
        {
            Assert.That(_repository.Login("wrong@test.com", "password123"), Is.Null);
            Assert.That(_repository.Login("test@test.com", "wrongpass"), Is.Null);
            Assert.That(_repository.Login("wrong@test.com", "wrongpass"), Is.Null);
        });
    }

    [Test]
    public void ObtenerUsuarioPorId_DebeRetornarNull_CuandoNoExiste()
    {
        // Act
        var resultado = _repository.ObtenerUsuarioPorId(999);

        // Assert
        Assert.That(resultado, Is.Null);
    }

    [Test]
    public void ObtenerUsuarios_DebeRetornarListaVacia_CuandoNoHayUsuarios()
    {
        // Act
        var resultado = _repository.ObtenerUsuarios();

        // Assert
        Assert.That(resultado, Is.Empty);
    }
}