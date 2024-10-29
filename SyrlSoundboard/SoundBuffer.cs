namespace Syrl.Soundboard;
using FFMpegCore;
using FFMpegCore.Pipes;
using Gtk;
using Silk.NET.OpenAL;

internal class SoundBuffer : IDisposable
{
    public uint bufferId;
    private AL _al;
    public bool completed = false;
    public bool valid = false;

    public event Action<SoundBuffer> OnComplete = (_) => {};
    public string path = "";
    public ulong ramData = 0;

    public SoundBuffer(AL al, string path)
    {
        this.path = path;
        this._al = al;
        bufferId = _al.GenBuffer();
        Console.WriteLine($"[OpenAL] - New Buffer Generated {bufferId}");
        Load();
    }

    private void Load()
    {
        completed = false;
        valid = false;
        Task.Run(async () =>
        {
            MemoryStream dStream = new();
            var ok = await FFMpegArguments
                .FromFileInput(path, true)
                .OutputToPipe(new StreamPipeSink(dStream), opt =>
                    opt.WithCustomArgument("-c:a pcm_s16le -f s16le -ar 44100 -ac 2")
                )
                .ProcessAsynchronously();
            completed = true;
            valid = ok;

            if (ok)
                Console.WriteLine($"[OpenAL] Decode OK : \"{path}\"");
            else
                Console.WriteLine($"[OpenAL] Failed to decode : \"{path}\"");

            GLib.Idle.Add(0, () =>
            {
                if (valid)
                {
                    byte[] bytes = dStream.ToArray();
                    ramData = (ulong)bytes.Length;
                    _al.BufferData(bufferId, BufferFormat.Stereo16, bytes, 44100);
                    OnComplete.Invoke(this);
                }
                return false;
            });
        });
    }

    public void Dispose()
    {
        _al.DeleteBuffer(bufferId);
    }
}