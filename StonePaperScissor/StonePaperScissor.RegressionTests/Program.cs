using System.Reflection;
using StonePaperScissor.Service.Simulation;
using StonePaperScissor.Service.Simulation.Items;
using StonePaperScissor.Service.Simulation.SimulationServices;
using StonePaperScissor.View;

var checks = 0;
void Check(bool condition, string message)
{
    checks++;
    if (!condition) throw new Exception(message);
}
var cases = new (Func<Position, Item> Attacker, Func<Position, Item> Victim, ItemType Winner)[]
{
    (p => new Stone("O", p), p => new Scissor("S", p), ItemType.Stone),
    (p => new Paper("P", p), p => new Stone("O", p), ItemType.Paper),
    (p => new Scissor("S", p), p => new Paper("P", p), ItemType.Scissor)
};
var transform = typeof(Simulator).GetMethod("MakeOneTransform", BindingFlags.NonPublic | BindingFlags.Instance)!;
foreach (var test in cases)
{
    var victim = test.Victim(new Position(1, 1));
    var first = test.Attacker(new Position(0, 0));
    var second = test.Attacker(new Position(0, 1));
    var items = new List<Item> { victim, first, second };
    Check(ReferenceEquals(first.Move(3, 3, items), victim), "Live prey must be hit.");
    Check(!victim.Alive, "Hit must defeat prey.");
    Check(second.Move(3, 3, items) is null, "Dead prey must not be hit again.");
    var simulator = new Simulator(items, 3, 3, new SilentVisualiser(), new DotStatistic(), null!);
    transform.Invoke(simulator, new object[] { victim });
    transform.Invoke(simulator, new object[] { victim });
    Check(items.Count == 3, "Duplicate transformations must preserve population.");
    Check(items.All(i => i.Alive && i.Type == test.Winner), "Replacement must have winner type and be alive.");
    Check(items.Count(i => i.Position.Equals(new Position(1, 1))) >= 1, "Replacement must retain position.");
    transform.Invoke(simulator, new object[] { first });
    Check(items.Contains(first) && items.Count == 3, "Living items must not transform.");
}

foreach (var test in cases)
{
    var position = new Position(1, 1);
    var survivor = test.Victim(position);
    var victim = test.Victim(position);
    victim.Alive = false;
    var items = new List<Item> { survivor, victim };
    var simulator = new Simulator(items, 3, 3, new SilentVisualiser(), new DotStatistic(), null!);
    transform.Invoke(simulator, new object[] { victim });
    Check(items.Any(i => ReferenceEquals(i, survivor)), "Equal living neighbour must survive.");
    Check(!items.Any(i => ReferenceEquals(i, victim)), "Exact defeated object must be removed.");
    Check(items.Count == 2 && items.All(i => i.Alive), "Identity replacement must preserve population and liveness.");
    var replacement = items.Single(i => !ReferenceEquals(i, survivor));
    Check(replacement.Type == test.Winner && replacement.Position.Equals(position), "Replacement must retain position and winner type.");
    transform.Invoke(simulator, new object[] { victim });
    Check(items.Count == 2 && items.Any(i => ReferenceEquals(i, survivor)), "Duplicate hit must not remove equal neighbour.");
}

var round = typeof(Simulator).GetMethod("PlayOneRound", BindingFlags.NonPublic | BindingFlags.Instance)!;
for (var game = 0; game < 20; game++)
{
    var items = new List<Item>();
    foreach (var test in cases)
        for (var i = 0; i < 10; i++)
            items.Add(test.Attacker(new Position(1, 1)));
    var simulator = new Simulator(items, 4, 4, new SilentVisualiser(), new DotStatistic(), null!);
    for (var turn = 0; turn < 100; turn++)
    {
        round.Invoke(simulator, null);
        Check(items.Count == 30, "Round changed population.");
        Check(items.All(i => i.Alive), "Dead item remained after round.");
    }
}
Console.WriteLine($"PASS: {checks} assertions, including 2,000 crowded simulation rounds.");

sealed class SilentVisualiser : IVisualiser
{
    public void SimulationVisualisation(List<Item> items, int rows, int columns) { }
}
