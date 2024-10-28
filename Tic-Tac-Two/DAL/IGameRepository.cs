using GameBrain;

namespace DAL;

public interface IGameRepository
{
    List<string> GetGameNames();
    GameState? GetGameStateByName(string name);
    string GetGameStateJsonByName(string name);
    bool GameExists(string name);
    void SaveGame(string jsonStateString, string gameConfigName, bool addDateTime);
    void DeleteGame(string name);
}