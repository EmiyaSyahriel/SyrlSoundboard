namespace Syrl.Soundboard;
using Tommy;
public class ConfigModel
{
    public class SoundItem
    {
        public string Name = "unknown";
        public string Path = "file://";
        public string Emoji = "🤯";

        public static SoundItem FromTomlTable(TomlTable table)
        {
            return new SoundItem()
            {
                Name = table[nameof(Name)].AsString,
                Path = table[nameof(Path)].AsString,
                Emoji = table[nameof(Emoji)].AsString,
            };
        }

        public TomlTable ToTomlTable()
        {
            return new TomlTable()
            {
                [nameof(Name)] = Name,
                [nameof(Path)] = Path,
                [nameof(Emoji)] = Emoji
            };
        }
    }

    public List<SoundItem> sounds = new();

    public void ApplyTomlTable(TomlTable table)
    {
        sounds.Clear();
        if(table.HasKey(nameof(sounds)))
        {
            foreach(var arr in table[nameof(sounds)].AsArray)
            {
                if(arr is TomlTable t)
                {
                    sounds.Add(SoundItem.FromTomlTable(t));
                }
            }
        }
    }

    public TomlTable ToTomlTable()
    {
        List<TomlTable> _sounds = new();
        foreach(var sound in sounds)
        {
            _sounds.Add(sound.ToTomlTable());
        }
        return new TomlTable()
        {
            [nameof(sounds)] = _sounds.ToArray()
        };
    }
}
