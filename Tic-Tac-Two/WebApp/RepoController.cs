using DAL;

namespace WebApp;

public class RepoController
{
    private readonly AppDbContext _context;

    public IConfigRepository ConfigRepository { get; private set; } = default!;
    public IGameRepository GameRepository { get; private set; } = default!;

    public RepoController(AppDbContext context)
    {
        _context = context;
        ConfigRepository = new ConfigRepositoryDb(context);
        GameRepository = new GameRepositoryDb(context);

        // ConfigRepository = new ConfigRepositoryJson();
        // GameRepository = new GameRepositoryJson();
    }
}