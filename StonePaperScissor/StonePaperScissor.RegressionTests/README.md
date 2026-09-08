# Motor regression checks

With the .NET 8 SDK, run from the repository root:

```sh
dotnet run --project StonePaperScissor/StonePaperScissor.RegressionTests/StonePaperScissor.RegressionTests.csproj
```

This executable compiles the actual motor sources and uses the ASP.NET Core shared framework. No database, running server, or additional NuGet packages are required. An assertion failure exits with a nonzero code.

Deterministic checks cover all predator/prey pairs, rejecting dead targets, duplicate transformation requests, preserving living elements, and replacing the exact victim when another element has the same type and position. Stress checks cover constant population and liveness across 2,000 randomized crowded rounds. Reflection invokes existing private methods without expanding the motor API. Concurrent start/resume behavior is outside this test scope.
