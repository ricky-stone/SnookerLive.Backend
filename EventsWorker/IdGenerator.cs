using System.Text;

namespace EventsWorker;

public static class IdGenerator
{
    public static string Generate(params object[] args)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < args.Length; i++)
        {
            sb.Append(args[i]);
            if (i < args.Length - 1)
                sb.Append("-");
        }
        return sb.ToString();
    }
}