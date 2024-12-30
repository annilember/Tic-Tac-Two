using Domain;
using GameBrain;

namespace DAL;

public interface IGameRepository
{
    List<SavedGame> GetSavedGames();
    List<string> GetGameNames();
    SavedGame GetSavedGameByName(string name);
    bool GameExists(string name);
    void SaveGame(TicTacTwoBrain gameInstance);
    void RenameGame(SavedGame savedGame, string newName);
    void DeleteGame(string name);
    SavedGame CreateGame(GameConfiguration config, EGameMode gameMode, string playerXName, string playerOName);

    SavedGame CreateGame(
        GameConfiguration config,
        EGameMode gameMode,
        string gameName,
        string playerXName,
        string playerOName,
        string playerXPassword,
        string playerOPassword);
    
    GameConfiguration GetGameConfiguration(SavedGame savedGame);
}