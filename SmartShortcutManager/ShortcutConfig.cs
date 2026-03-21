using System.Collections.Generic;

namespace SmartShortcutManager
{
    public class ServiceExePair
    {
        public string Name { get; set; } = string.Empty;
        public List<string> ServiceNames { get; set; } = new();
        public List<string> ExePaths { get; set; } = new();

        // Backward compatibility for old configs with single ExePath
        public string ExePath
        {
            get => ExePaths.FirstOrDefault() ?? string.Empty;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    ExePaths = new List<string> { value };
                }
            }
        }
    }

    public class Config
    {
        public List<ServiceExePair> Pairs { get; set; } = new();
    }
}
