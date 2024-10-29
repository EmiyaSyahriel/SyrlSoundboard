namespace Syrl.Soundboard;
using Gtk;

internal class SysHeaderBar : HeaderBar
{
    private SoundboardWindow _window;
    private Button _addBtn;
    private Button _refBtn;
    private Button _stopBtn;
    private Button _purBtn;

    public SysHeaderBar(SoundboardWindow window) : base()
    {
        _window = window;
        Title = window.Title;
        ShowCloseButton = true;
        Visible = true;

        _addBtn = NewButton("list-add-symbolic", "Add", _window.OnAddClick);
        _refBtn = NewButton("view-refresh-symbolic", "Compact Data", _window.CompactBuffer);
        _stopBtn = NewButton("media-playback-stop-symbolic", "Stop All", _window.StopAll);
        _purBtn = NewButton("user-trash-full-symbolic", "Purge Buffers", _window.PurgeBuffers);
        PackStart(_addBtn);
        PackStart(_refBtn);
        PackStart(_stopBtn);
        PackStart(_purBtn);
    }

    public Button NewButton(string icon, string tooltip, EventHandler click)
    {
        var btn = new Button()
        {
            Visible = true,
            Image = Image.NewFromIconName(icon, IconSize.Menu)
        };
        btn.StyleContext.AddClass("titlebutton");
        btn.TooltipText = tooltip;
        btn.Clicked += click;
        return btn;
    }

}