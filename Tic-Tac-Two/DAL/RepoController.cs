namespace DAL;

public class RepoController
{
    public IConfigRepository ConfigRepository { get; private set; } = default!;
    public IGameRepository GameRepository { get; private set; } = default!;

    public RepoController(AppDbContext context)
    {
        ConfigRepository = new ConfigRepositoryDb(context);
        GameRepository = new GameRepositoryDb(context);

        // ConfigRepository = new ConfigRepositoryJson();
        // GameRepository = new GameRepositoryJson();
    }
}