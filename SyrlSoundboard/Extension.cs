namespace Syrl.Soundboard;
internal static class Extension 
{
    internal static string[] BI_BYTES_SUFFICES = ["B", "kiB", "MiB", "GiB", "TiB", "PiB", "EiB", "ZiB", "YiB", "RiB", "QiB"];
    internal static string[] TH_BYTES_SUFFICES = ["B", "kB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB", "RB", "QB"];
    public static string ToNearestByteSize(this ulong bytes, string fmt, bool useBiBytes)
    {
        double dBytes = bytes;
        double div = useBiBytes ? 1024 : 1000;
        string[] suff = useBiBytes ? BI_BYTES_SUFFICES : TH_BYTES_SUFFICES;
        int i = 0;
        do 
        {
            dBytes /= div;
            i++;
        }while(dBytes > div && i < suff.Length);
        return $"{dBytes.ToString(fmt)}{suff[i]}";
    }
}