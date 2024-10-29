namespace Syrl.Soundboard;
using Gtk;
using IOPath = System.IO.Path;
using System.IO;
using Tommy;

public class SoundboardWindow : Window
{
    ScrolledWindow _scroll;
    Grid _mainGrid;
    internal SoundManager _manager;
    internal ConfigModel cfg = new();
    private List<SoundButton> btns = new();
    private SysHeaderBar header;
    public SoundboardWindow() : base("SyrlSoundboard")
    {
        header = new SysHeaderBar(this);
        Titlebar = header;
        var root = new Box(Orientation.Vertical, 10)
        {
            Visible = true,
            Expand = true,
            Valign = Align.Fill,
            Halign = Align.Fill
        };
        SetDefaultSize(610, 500); 
        _scroll = new ScrolledWindow()
        {
            Visible = true,
            Hexpand = false,
            Vexpand = true,
            Valign = Align.Fill,
            Halign = Align.Fill
        };

        _scroll.Add(_mainGrid = new Grid()
        {
            Visible = true,
            RowSpacing= 10,
            ColumnSpacing = 10,
        });
        _mainGrid.Margin = 10;

        root.PackStart(_scroll, true, true, 10);

        _manager = new SoundManager(this);

        Add(root);
        UpdateWindowSubtitle(0, 0UL);

        LoadCatalog();
        DeleteEvent += Quit;
    }
    
    private int _lastColCount = 0;

    private void ReadjustGrid(bool force)
    {
        this.GetSize(out int w, out int h);
        int colCount = (w - 10) / 120;

        if(_lastColCount == colCount && !force) // Same, Do not relayout
        {
            return;
        }

        foreach (var e in btns)
        {
            _mainGrid.Remove(e);
        }

        for (int i = 0; i < btns.Count; i++)
        {
            int x = i % colCount;
            int y = i / colCount;
            _mainGrid.Attach(btns[i], x, y, 1, 1);
        }

    }

    protected override void OnResizeChecked()
    {
        ReadjustGrid(false);
        base.OnResizeChecked();
    }

    private string CfgRootPath 
    {
        get {
            string userData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
            string path = IOPath.Combine(userData, "Soundboard");
            if(!Directory.Exists(path)) Directory.CreateDirectory(path);

            return path;
        }
    }

    private string CfgPath => IOPath.Combine(CfgRootPath, "sndcfg.toml");

    private void LoadCatalog()
    {
        if(!File.Exists(CfgPath)) return;

        try 
        {
            using(StreamReader r = File.OpenText(CfgPath))
            {
                cfg.ApplyTomlTable(TOML.Parse(r));
            }
        }
        catch(Exception _e)
        {
            var msgDlg = new MessageDialog(this, DialogFlags.Modal, MessageType.Error, ButtonsType.Close, $"Failed to parse config file : \n\n{_e.Message}");
            msgDlg.Run();
            msgDlg.Destroy();
            cfg = new();
        }

        foreach(var snd in cfg.sounds)
        {
            AddSound(snd.Emoji, snd.Name, snd.Path, false);
        }
    }

    public void WriteCatalog()
    {
        using var cfgstr = File.OpenWrite(CfgPath);
        cfgstr.SetLength(0);
        using var writer = new StreamWriter(cfgstr);
        cfg.ToTomlTable().WriteTo(writer);
    }

    internal void OnAddClick(object? sender, EventArgs e)
    {
        var dlg = new FileChooserDialog("Add Sound", this, FileChooserAction.Open, [
            "Cancel",
            ResponseType.Cancel,
            "Accept",
            ResponseType.Accept
        ]);

        var filter = new FileFilter();
        filter.AddMimeType("audio/*");
        dlg.AddFilter(filter);
        
        ResponseType r = (ResponseType)dlg.Run();
        Console.WriteLine(r);
        if(r == ResponseType.Accept)
        {
            string fName = dlg.Filename;
            Console.WriteLine(fName);
            var _addDlg = new AddSoundDialog(this, fName)
            {
                SourceFileName = fName
            };
            _addDlg.Show();
        }
        dlg.Destroy();
    }

    private void Quit(object o, DeleteEventArgs args)
    {
        Application.Quit();
    }

    internal void AddSound(string emoji, string title, string fileName, bool writeToConfig)
    {
        var mdl = new ConfigModel.SoundItem()
        {
            Name = title,
            Emoji = emoji,
            Path = fileName
        };
        // New Button
        if (writeToConfig)
        {
            cfg.sounds.Add(mdl);
            WriteCatalog();
        }

        var btn = new SoundButton(this, mdl);
        btns.Add(btn);
        ReadjustGrid(true);
    }

    protected override void OnShown()
    {
        base.OnShown();
    }

    internal void RemoveSoundBtn(SoundButton soundButton)
    {
        btns.RemoveAll(it => it.data.Name == soundButton.data.Name);
        _mainGrid.Remove(soundButton);
        cfg.sounds.RemoveAll(it => it.Name == soundButton.data.Name);
        _manager.Compact(cfg);
        WriteCatalog();
        ReadjustGrid(true);
    }

    internal void CompactBuffer(object? sender, EventArgs e)
    {
        _manager.Compact(cfg);
    }

    internal void UpdateWindowSubtitle(int bufferCount, ulong byteSize)
    {
        header.HasSubtitle = true;
        string sz = byteSize.ToNearestByteSize("0.0", false);
        header.Subtitle = $"{bufferCount} buffers, {sz} bytes";
    }

    internal void UpdateRAMSize()
    {
        ulong total = 0;
        foreach(var snd in _manager._bufferPool)
        {
            total+= snd.ramData;
        }
        UpdateWindowSubtitle(_manager._bufferPool.Count, total);
    }

    internal void StopAll(object? sender, EventArgs e)
    {
        _manager.StopAll();
    }

    internal void PurgeBuffers(object? sender, EventArgs e)
    {
        StopAll(sender, e);
        _manager.Purge();
        UpdateRAMSize();
    }
}