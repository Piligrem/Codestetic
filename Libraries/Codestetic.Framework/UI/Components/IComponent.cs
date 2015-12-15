using System;

namespace Codestetic.Web.Framework.UI
{
    public interface IComponent : IHtmlAttributesContainer
    {
        string Id { get; }

        string Name { get; }

        bool NameIsRequired { get; }
    }
}
