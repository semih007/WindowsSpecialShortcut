using System.Collections.Generic;

namespace SmartShortcutManager
{
    public class ServiceExePair
    {
        public string Name { get; set; } = string.Empty;
        public List<string> ServiceNames { get; set; } = new();
        public string ExePath { get; set; } = string.Empty;
    }

    public class Config
    {
        public List<ServiceExePair> Pairs { get; set; } = new();
    }
}
