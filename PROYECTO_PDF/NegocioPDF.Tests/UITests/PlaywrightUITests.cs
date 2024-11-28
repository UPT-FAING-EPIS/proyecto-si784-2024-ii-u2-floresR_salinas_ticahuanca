using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using MSTest = Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NegocioPDF.Tests.UITests;

[MSTest.TestClass]
public class SubscriptionTests : PageTest
{
    private string baseUrl = "http://localhost:5260";
    
    

    [MSTest.TestMethod]
    public async Task LoginFailure_ShouldShowError()
    {
        var browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            SlowMo = 1000
        });

        var context = await browser.NewContextAsync(new()
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new() { Width = 1024, Height = 768 }
        });

        var page = await context.NewPageAsync();

        try
        {
            Console.WriteLine("Iniciando login fallido...");
            await page.GotoAsync($"{baseUrl}/Auth/Login");

            // Llenar formulario con credenciales inválidas
            await page.FillAsync("input[name='correo']", "wrong@email.com");
            await page.FillAsync("input[name='password']", "WrongPassword123!");
            
            // Tomar screenshot antes de enviar
            await page.ScreenshotAsync(new() { Path = "before-login-failure.png" });

            // Click en login
            await page.GetByRole(AriaRole.Button, new() { Name = "Iniciar Sesión" }).ClickAsync();
            await Task.Delay(2000);

            // Verificar mensaje de error
            MSTest.Assert.IsTrue(await page.Locator(".alert-danger").IsVisibleAsync());

            await page.ScreenshotAsync(new() { Path = "after-login-failure.png" });
        }
        finally
        {
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }

    [MSTest.TestMethod]
    public async Task Register_NewUser()
    {
        var browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            SlowMo = 1000
        });

        var context = await browser.NewContextAsync(new()
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new() { Width = 1024, Height = 768 }
        });

        var page = await context.NewPageAsync();

        try
        {
            Console.WriteLine("Iniciando registro de usuario...");
            await page.GotoAsync($"{baseUrl}/Registration/Registrarse");

            // Llenar formulario de registro
            await page.FillAsync("input[name='Nombre']", "Usuario Test");
            await Task.Delay(1000);
            
            await page.FillAsync("input[name='Correo']", "test@example.com");
            await Task.Delay(1000);
            
            await page.FillAsync("input[name='Password']", "Password123!");
            await Task.Delay(1000);

            // Screenshot antes de enviar
            await page.ScreenshotAsync(new() { Path = "before-registration.png" });

            // Click en registrarse
            await page.GetByRole(AriaRole.Button, new() { Name = "Registrarse" }).ClickAsync();
            await Task.Delay(2000);

            // Verificar mensaje de éxito
            MSTest.Assert.IsTrue(await page.Locator(".alert-success").IsVisibleAsync());

            await page.ScreenshotAsync(new() { Path = "after-registration.png" });
        }
        finally
        {
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }

    [MSTest.TestMethod]
    public async Task TestPremiumSubscriptionFlow()
    {
        var browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            SlowMo = 1000
        });

        var context = await browser.NewContextAsync(new()
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new() { Width = 1024, Height = 768 }
        });

        var page = await context.NewPageAsync();

        try
        {
            // 1. Login
            Console.WriteLine("Iniciando login...");
            await page.GotoAsync($"{baseUrl}/Auth/Login");
            await page.FillAsync("input[name='correo']", "test@example.com");
            await page.FillAsync("input[name='password']", "Password123!");
            await page.GetByRole(AriaRole.Button, new() { Name = "Iniciar Sesión" }).ClickAsync();
            await Task.Delay(2000);

            // 2. Ir a comprar premium desde menú principal
            Console.WriteLine("Navegando a comprar premium...");
            await page.GetByRole(AriaRole.Link, new() { Name = "Comprar Premium" }).ClickAsync();
            await Task.Delay(2000);

            // 3. Verificar detalles de suscripción premium
            MSTest.Assert.IsTrue(await page.Locator("text=Detalles de la Suscripción").IsVisibleAsync());
            MSTest.Assert.IsTrue(await page.Locator("text=Fecha de Inicio").IsVisibleAsync());
            MSTest.Assert.IsTrue(await page.Locator("text=Fecha Final").IsVisibleAsync());
            MSTest.Assert.IsTrue(await page.Locator("text=Precio").IsVisibleAsync());

            // 4. Confirmar compra premium
            await page.GetByRole(AriaRole.Button, new() { Name = "Comprar Suscripción Premium" }).ClickAsync();
            await Task.Delay(2000);

            // 5. Verificar que estamos en estado premium
            MSTest.Assert.IsTrue(await page.Locator("text=Operaciones Realizadas: Ilimitadas").IsVisibleAsync());

            await page.ScreenshotAsync(new() { Path = "premium-subscription.png" });
        }
        finally
        {
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }
 [MSTest.TestMethod]
    public async Task LoginSuccess_ShouldShowOperationsButton()
    {
        var browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = true,
            SlowMo = 1000
        });

        var context = await browser.NewContextAsync(new()
        {
            RecordVideoDir = "videos",
            RecordVideoSize = new() { Width = 1024, Height = 768 }
        });

        var page = await context.NewPageAsync();

        try
        {
            Console.WriteLine("Iniciando login exitoso...");
            await page.GotoAsync($"{baseUrl}/Auth/Login");

            // Llenar formulario
            await page.FillAsync("input[name='correo']", "test@example.com");
            await page.FillAsync("input[name='password']", "Password123!");
            
            // Tomar screenshot antes de enviar
            await page.ScreenshotAsync(new() { Path = "before-login.png" });

            // Click en login
            await page.GetByRole(AriaRole.Button, new() { Name = "Iniciar Sesión" }).ClickAsync();
            await Task.Delay(2000);

            // Verificar elementos post-login
            MSTest.Assert.IsTrue(await page.GetByRole(AriaRole.Link, new() { Name = "Realizar Operaciones" }).IsVisibleAsync());
            MSTest.Assert.IsTrue(await page.GetByRole(AriaRole.Link, new() { Name = "Cerrar Sesión" }).IsVisibleAsync());

            await page.ScreenshotAsync(new() { Path = "after-login-success.png" });
        }
        finally
        {
            await context.CloseAsync();
            await browser.CloseAsync();
        }
    }
    
}