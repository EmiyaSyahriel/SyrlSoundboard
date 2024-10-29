namespace Syrl.Soundboard;
using Gtk;

public class AddSoundDialog : Dialog
{
    internal string SourceFileName;
    private SoundboardWindow _window;
    private Entry _emojiEntry;
    private Entry _titleEntry;
    private Label _sourceFileName;
    private Button _previewBtn;


    public AddSoundDialog(SoundboardWindow window, string sourceFileName) : base("Add Sound", window, DialogFlags.Modal)
    {
        _window = window;
        SourceFileName = sourceFileName;
        Title = "Add Sound";
        var root = ContentArea;
        
        SetDefaultSize(400,200);

        var btnBox = new ButtonBox(Orientation.Horizontal)
        {
            Visible = true
        };
        var closeBtn = new Button("close", IconSize.Dialog)
        {
            Label = "Cancel",
            Visible = true
        };
        var addBtn = new Button("list-add", IconSize.Dialog)
        {
            Label = "Add",
            Visible = true
        };

        closeBtn.Clicked += HideDlg;
        addBtn.Clicked += AddToCatalog;
        btnBox.PackEnd(addBtn, true, true, 0);
        btnBox.PackEnd(closeBtn, true, true, 0);

        var table = new Grid()
        {
            Visible = true,
            RowSpacing = 10
        };

        table.Attach(new Label("Emoji")
        {
            Visible = true
        }, 0, 0, 1,1);

        table.Attach(_emojiEntry = new Entry()
        {
            Visible = true,
            MaxLength = 1,
            MaxWidthChars = 1
        }, 1, 0, 5, 1);

        table.Attach(new Label("Title")
        {
            Visible = true
        }, 0,1,1,1);

        table.Attach(_titleEntry = new Entry()
        {
            Visible = true,
        }, 1,1,5,1);

        table.Attach(new Label("Path")
        {
            Visible = true
        }, 0, 2, 1, 1);
        table.Attach(_previewBtn = new Button("media-playback-start", IconSize.Dialog)
        {
            Visible = true,
            Label = "Preview",
            MarginEnd = 5,
            MarginStart = 5
        }, 1, 2, 1, 1);

        _previewBtn.Clicked += Preview;
        table.Attach(_sourceFileName = new Label(sourceFileName)
        {
            Visible = true
        }, 2,2,4,1);
        root.SetSizeRequest(400, 250);
        root.PackStart(table, true, true,0);
        root.PackEnd(btnBox, false, false, 0);    
    }

    private void Preview(object? sender, EventArgs e)
    {
        _window._manager.PlayOneShot(SourceFileName);
    }

    private void HideDlg(object? sender, EventArgs e)
    {
        Destroy();
    }

    private void AddToCatalog(object? sender, EventArgs e)
    {
        _window.AddSound(_emojiEntry.Text, _titleEntry.Text, SourceFileName, true);
        Destroy();
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        _window._manager.Compact(_window.cfg);
    }
}