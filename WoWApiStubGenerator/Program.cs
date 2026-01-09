using System;
using System.Threading;
using System.Threading.Tasks;

namespace WoWApiStubGenerator;
internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        using var cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };


        var cliCommand = CreateCommandLineCommand();
        var parseResult = cliCommand.Parse(args);
        if (parseResult.Errors.Count > 0)
        {
            foreach (var err in parseResult.Errors) Console.Error.WriteLine(err.Message);
            return 1;
        }

        parseResult.GetValue<string>("BlizzardInterfaceSourceDir");
        parseResult.GetValue<string>("StubOutputDir");

        try
        {
            var source = args[0];
            Console.WriteLine($"Generating stubs from: {source}");
            await GenerateStubsAsync(source, cts.Token).ConfigureAwait(false);
            Console.WriteLine("Stub generation completed.");
            return 0;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation cancelled.");
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static System.CommandLine.Command CreateCommandLineCommand()
    {
        return new System.CommandLine.Command("WoWApiStubGenerator", "Generates WoW API stubs from a given source.")
        {
            new System.CommandLine.Argument<string>("BlizzardInterfaceSourceDir"),
            new System.CommandLine.Argument<string>("StubOutputDir"),
            new System.CommandLine.Option<bool>("--widget-api", "Fetch widget API definitions from the web.")
        };
    }

    // Placeholder for the real generation logic.
    private static Task GenerateStubsAsync(string source, CancellationToken cancellationToken)
    {
        // Replace with real implementation.
        return Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
    }
}
