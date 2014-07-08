using System.IO;
using System.Reflection;

namespace WeatherAggegator.Helpers
{
    public static class AssemblyExtensions
    {
        public static string GetResource(this Assembly assembly, string name)
        {
            using (Stream stream = assembly.GetManifestResourceStream(name))
            using (StreamReader reader = new StreamReader(stream))
            {
               return reader.ReadToEnd();
            }
        }
    }
}
