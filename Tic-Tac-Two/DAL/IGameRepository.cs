using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    List<string> GetGameNames();
    SavedGame GetSavedGameByName(string name);
    GameState GetSavedGameState(SavedGame savedGame);
    string GetSavedGameJsonByName(string name);
    bool GameExists(string name);
    void SaveGame(TicTacTwoBrain gameInstance, string name);
    void RenameGame(SavedGame savedGame, string newName);
    void DeleteGame(string name);
    GameConfiguration GetGameConfiguration(SavedGame savedGame);
}