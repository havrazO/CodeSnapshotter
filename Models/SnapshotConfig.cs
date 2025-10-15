using System;
using System.Collections.Generic;

namespace CodeSnapshotter.Models
{
    public class SnapshotConfig
    {
        public List<string> IncludeExtensions { get; set; } = new();
        public List<string> ExcludeDirectories { get; set; } = new();
        public List<string> ExcludeFiles { get; set; } = new();
        public bool IsMarkdown { get; set; }
        public Action<string> StatusUpdateCallback { get; set; }
    }
}