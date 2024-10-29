namespace Syrl.Soundboard;
using Gtk;
using System;

public class Program()
{
    public static int Main(string[] args)
    {
        Application.Init();
        Syrl.Soundboard.SoundboardWindow _ = new();
        _.Show();
        Application.Run();
        return 0;
    }
}