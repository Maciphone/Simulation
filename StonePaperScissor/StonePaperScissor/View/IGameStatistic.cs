using StonePaperScissor.Service.Simulation;

namespace StonePaperScissor.View;

public interface IGameStatistic
{
    void ShowStatistic(List<Item> items);

    Dictionary<ItemType, int> SendStatistic(List<Item> items);
}