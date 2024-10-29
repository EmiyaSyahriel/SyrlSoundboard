# SyrlSoundboard
A simple soundboard app made using .NET 8.0, GtkSharp and Silk.NET.OpenAL

This app is just a collection of sound, each with it's own play button. To maximize it's usage, you can use [PipeWire Helvum](https://gitlab.freedesktop.org/pipewire/helvum) or any other PipeWire routing utility to reroute the the app's audio output to other app, Discord for example.

So far only tested on Linux ( Ubuntu 24.04 ) as it is using Gtk 3.0, but since Gtk also supports Windows, it might be compatible there.

## Usage
### Adding sound
Press the `(+)` button at the top left of window. Select desired audio file, paste an Emoji ( or you can just use letter) and set the title, and then press add.

Supported format : Any format your local ffmpeg supports. but the file chooser would only filter formats that is under the mime type `audio/*`

> [!WARNING]
> All of the audio wave is stored raw on RAM, the longer the sound, the 
> bigger RAM space it will occupy, including the silent part, so cut your sound accordingly.

### Removing sound
Middle click the button.

### Playing sound
Click the sound button.

## Runtime Requirements
- GTK3
- .NET 8.0 Runtime
- OpenAL Driver (OpenAL-Soft is prefered)
- FFMpeg
- Ubuntu 24.04 ( might compatible with older version, might also run on Windows)

## Building
### Requirements
- .NET 8.0 ( Runtime and SDK )

### Compiling
- Clone the repo and open the folder
```bash
git clone https://github.com/EmiyaSyahriel/SyrlSoundboard
cd SyrlSoundboard/SyrlSoundboard
```
- Compile the app
```bash
dotnet publish
```
  > The binary will be generated at `./bin/Release/net8.0/(Your OS)/publish/SyrlSoundboard`
  - Or, if you want to run the app right away
```bash
dotnet run
```

### Used Dependencies
- [GtkSharp](https://github.com/GtkSharp/GtkSharp)
- [Tommy](https://github.com/dezhidki/Tommy)
- [Silk.NET.OpenAL](https://github.com/dotnet/Silk.NET)
- [FFMpegCore](https://github.com/rosenbjerg/FFMpegCore)

## License
MIT License ( see [LICENSE](LICENSE.MD) )