namespace MenuSystem;

public class MenuItem
{
    private readonly string _title = default!;
    private readonly string _shortcut = default!;
    
    public Func<string>? MenuItemAction { get; set; }

    public string Title
    {
        get => _title;
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Title cannot be empty");
            }
            _title = value;
        }
    }

    public string Shortcut
    {
        get => _shortcut;
        init
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Shortcut cannot be empty");
            }
            _shortcut = value;
        }
    }

    public override string ToString()
    {
        return Shortcut + ") " + Title;
    }
}