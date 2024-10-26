using GameBrain;

namespace DAL;

public interface IGameRepository
{
    List<string> GetGameNames();
    GameState? GetGameStateByName(string name);
    public void SaveGame(string jsonStateString, string gameConfigName);
}