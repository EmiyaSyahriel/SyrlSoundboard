namespace Syrl.Soundboard;
using System;
using Silk.NET.OpenAL;
using FFMpegCore;
using System.Runtime.CompilerServices;
using FFMpegCore.Pipes;
using FFMpegCore.Enums;

public class SoundManager: IDisposable
{

    private SoundboardWindow window;
    private AL al;
    private ALContext alc;
    private unsafe Device* _device;
    private unsafe Context* _ctx;
    private bool _initialized = false;
    internal List<SoundBuffer> _bufferPool = [];
    internal List<SoundSource> _sourcePool = new();

    public SoundManager(SoundboardWindow window)
    {
        this.window = window;
        al = AL.GetApi(true);
        alc = ALContext.GetApi(true);

        unsafe 
        {
            _device = alc.OpenDevice(null);
            
            _ctx = alc.CreateContext(_device, null);
            if (!alc.MakeContextCurrent(_ctx))
            {
                return;
            }
        }
        AGL();

        PrintInfo(); AGL();
        al.SetListenerProperty(ListenerVector3.Position, 0,0,0); AGL();
        al.SetListenerProperty(ListenerVector3.Velocity, 0,0,0); AGL();
        _initialized = true;
    }

    private void PrintInfo()
    {
        string vnd = al.GetStateProperty(StateString.Vendor);
        string ver = al.GetStateProperty(StateString.Version);
        string rnd = al.GetStateProperty(StateString.Renderer);
        Console.WriteLine($"OpenAL Initialized : {rnd} {ver} ( {vnd} )");

        unsafe 
        {
            int monoCount = 0, stereoCount = 0;
            alc.GetContextProperty(_device, (GetContextInteger)ContextAttributes.MonoSources, 1, &monoCount);
            alc.GetContextProperty(_device, (GetContextInteger)ContextAttributes.StereoSources, 1, &stereoCount);
            Console.WriteLine($"OpenAL Source Count :: Mono({monoCount}), Stereo({stereoCount})");
        }
    }

    private void AGL([CallerFilePath] string path = "", [CallerLineNumber] int line  = 0)
    {
        AudioError err= al.GetError();
        if(err == AudioError.NoError) return;
        Console.WriteLine($"OpenAL Error : {err} at or before {path}:{line}");
    }

    internal void Compact(ConfigModel mdl)
    {
        List<SoundBuffer> purgeable = _bufferPool.FindAll(it => mdl.sounds.FindIndex(e => e.Path == it.path) == -1);
        foreach(var p in purgeable)
        {
            p.Dispose();
        }
        Console.WriteLine($"Purged {purgeable.Count} Unused sound buffers");
    }

    private SoundSource FindSource()
    {
        var p = _sourcePool.FindIndex(it => !it.IsPlaying);
        if(p == -1) // Make new
        {
            var src = new SoundSource(al);
            _sourcePool.Add(src);
            return src;
        }else return _sourcePool[p];
    }

    public void PlayOneShot(string path)
    {
        if(!_initialized) return;
        int sndIdx = _bufferPool.FindIndex(it => it.path == path);
        if(sndIdx == -1) // Not found
        {
            var buffer = new SoundBuffer(al, path);
            buffer.OnComplete += (_) => window.UpdateRAMSize();
            SoundSource source = FindSource();
            _bufferPool.Add(buffer);
            source.SetBuffer(buffer);
            source.Play();
        }
        else
        {
            SoundSource source = FindSource();
            source.SetBuffer(_bufferPool[sndIdx]);
            source.Play();
        }
    }

    public void Dispose()
    {
        foreach(var src in _sourcePool)
        {
            src.Dispose();
        }

        foreach(var buf in _bufferPool)
        {
            buf.Dispose();
        }

        al.Dispose();
    }

    internal void Purge()
    {
        foreach(var p in _sourcePool)
        {
            al.SourceStop(p.sourceId);
            p.SetBuffer(null);
        }

        foreach(var b in _bufferPool)
        {
            b.Dispose();
        }
        _bufferPool.Clear();
    }

    internal void StopAll()
    {
        foreach(var p in _sourcePool)
        {
            if(p.IsPlaying)
            {
                al.SourceStop(p.sourceId);
            }
        }
    }
}