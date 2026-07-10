using System.Windows;
using BookTracker.Data;

namespace BookTracker;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // EnsureCreated statt Migrations: für dieses Projekt bewusst einfach gehalten.
        // Bei einem wachsenden Projekt würde ich auf EF Core Migrations wechseln,
        // um Schemaänderungen nachvollziehbar zu versionieren.
        using var context = new BookTrackerContext();
        context.Database.EnsureCreated();
    }
}
