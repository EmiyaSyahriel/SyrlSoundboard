namespace Syrl.Soundboard;
using System.Reflection.Metadata;
using Silk.NET.OpenAL;

internal class SoundSource : IDisposable
{
    public uint sourceId;
    private bool bufferOk;
    private AL _al;
    private bool _playOnComplete;

    public SoundSource(AL al)
    {
        _al = al;
        bufferOk = false;
        sourceId = al.GenSource();
        Console.WriteLine($"[OpenAL] New Source Created : {sourceId}");
    }

    public bool IsPlaying 
    {
        get 
        {
            _al.GetSourceProperty(sourceId, GetSourceInteger.SourceState, out int _state);
            return (SourceState)_state == SourceState.Playing;
        }
    }

    public bool Otw => !bufferOk;

    public void Dispose()
    {
        _al.DeleteSource(sourceId);
    }

    public void SetBuffer(SoundBuffer? buffer)
    {
        bufferOk = false;
        if (buffer == null)
        {
            _al.SetSourceProperty(sourceId, SourceInteger.Buffer, 0);
            return;
        }

        if(buffer.completed)
        {
            AssignBuffer(buffer);
        }
        else
        {
            buffer.OnComplete += AssignBuffer;
        }
    }

    private void AssignBuffer(SoundBuffer buffer)
    {
        bufferOk = true;
        _al.SetSourceProperty(sourceId, SourceInteger.Buffer, buffer.bufferId);
        if(_playOnComplete)
        {
            _al.SourcePlay(sourceId);
        }
    }

    public void Play()
    {
        if(!bufferOk)
        {
            _playOnComplete = true;
        }
        else
        {
            _al.SourcePlay(sourceId);
        }
        
    }
}