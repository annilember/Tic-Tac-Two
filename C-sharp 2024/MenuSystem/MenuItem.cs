namespace MenuSystem;

public class MenuItem
{
    private string _title;
    private string _shortcut;

    public string Title
    {
        get => _title;
        set
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
        set
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