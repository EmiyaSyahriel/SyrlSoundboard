namespace Syrl.Soundboard;
using Gdk;
using Gtk;

public class SoundButton : EventBox
{
    private SoundboardWindow _window;
    private Label emoTxt;
    private Label titTxt;
    internal ConfigModel.SoundItem data;

    public SoundButton(SoundboardWindow window, ConfigModel.SoundItem item) : base()
    {
        _window = window;
        var btn = new Button()
        {
            Visible = true
        };
        Child =btn;
        this.data = item;
        var box = new Box(Orientation.Vertical, 0)
        {
            Visible = true
        };
        emoTxt = new Label(item.Emoji)
        {
            Visible = true
        };
        titTxt = new Label(item.Name)
        {
            Visible = true
        };
        
        emoTxt.Layout.Alignment = Pango.Alignment.Center;

        var fd = emoTxt.PangoContext.FontDescription;
        fd.Size = 32;
        emoTxt.PangoContext.FontDescription = fd;

        box.PackStart(emoTxt, true, true, 0);
        box.PackEnd(titTxt, false, true, 10);
        this.SetSizeRequest(110, 110);
        btn.Add(box);
        btn.Clicked += OnBtnClick;
        Visible = true;
        UpdateData();
    }

    private void OnBtnClick(object? sender, EventArgs e)
    {
        _window._manager.PlayOneShot(data.Path);
    }

    public void UpdateData()
    {
        emoTxt.Markup = $"<span size=\'28pt\'>{data.Emoji}</span>";
        titTxt.Text = data.Name;
    }

    protected override bool OnButtonPressEvent(EventButton evnt)
    {
        if(evnt.Button == 2)
        {
            _window.RemoveSoundBtn(this);
            return true;
        }
        return base.OnButtonPressEvent(evnt);
    }

}
