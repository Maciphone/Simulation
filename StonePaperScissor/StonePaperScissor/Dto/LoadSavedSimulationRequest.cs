namespace StonePaperScissor.Dto;

public class LoadSavedSimulationRequest
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public required string SavedItems { get; set; }
}