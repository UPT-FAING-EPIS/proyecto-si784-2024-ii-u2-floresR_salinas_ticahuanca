using BoDi;
using Microsoft.EntityFrameworkCore;
using NegocioPDF.Data;
using NegocioPDF.Repositories;
using TechTalk.SpecFlow;

namespace NegocioPDF.Tests
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _container;
        private PDFSolutionsContext? _context;

        public Hooks(IObjectContainer container)
        {
            _container = container;
        }

        [BeforeScenario]
        public void RegisterDependencies()
        {
            var options = new DbContextOptionsBuilder<PDFSolutionsContext>()
                .UseInMemoryDatabase(databaseName: $"TestDB_{Guid.NewGuid()}")
                .Options;

            _context = new PDFSolutionsContext(options);
            
            _container.RegisterInstanceAs<PDFSolutionsContext>(_context);
            _container.RegisterInstanceAs<IUsuarioRepository>(new UsuarioRepository(_context));
            _container.RegisterInstanceAs<IOperacionesPDFRepository>(new OperacionesPDFRepository(_context));
            _container.RegisterInstanceAs<IDetalleSuscripcionRepository>(new DetalleSuscripcionRepository(_context));
        }

        [AfterScenario]
        public void CleanupDependencies()
        {
            _context?.Dispose();
        }
    }
}