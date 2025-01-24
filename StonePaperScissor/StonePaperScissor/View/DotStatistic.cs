using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.View;

public class DotStatistic : IGameStatistic
{
    public void ShowStatistic(List<Item> items)
    {
        Dictionary<ItemType, int> actualStand = CountItemsByType(items);
        Console.WriteLine(string.Join(", ", actualStand.Select(item => $"{item.Key}: {item.Value}")));
    }

    private Dictionary<ItemType, int> CountItemsByType(List<Item> items)
    {
        return items.GroupBy(item => item.Type).ToDictionary(g => g.Key,
            g => g.Count());
    }
}