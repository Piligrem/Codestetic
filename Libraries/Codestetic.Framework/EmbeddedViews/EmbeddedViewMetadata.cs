using System;

namespace Codestetic.Web.Framework.EmbeddedViews
{
    [Serializable]
    public class EmbeddedViewMetadata
    {
        public string Name { get; set; }
        public string AssemblyFullName { get; set; }
    }
}