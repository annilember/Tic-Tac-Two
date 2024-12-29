using Domain;
using DTO;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    List<SavedGame> GetSavedGames();
    List<string> GetGameNames();
    SavedGame GetSavedGameByName(string name);
    GameState GetSavedGameState(SavedGame savedGame);
    string GetSavedGameJsonByName(string name);
    bool GameExists(string name);
    void SaveGame(SavedGame savedGame);
    void SaveGame(TicTacTwoBrain gameInstance, string name);
    void RenameGame(SavedGame savedGame, string newName);
    void DeleteGame(string name);
    void CreateGame(SavedGame savedGame);
    GameConfiguration GetGameConfiguration(SavedGame savedGame);
}