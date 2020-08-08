using System;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components
{
    public abstract class PageBase : ComponentBase
    {
        protected RenderFragment AddComponent(string component, object model) => builder =>
        {
            const string theme = "Classic";

            var type = Type.GetType($"Atlas.Client.Themes.{theme}.{component}, {typeof(Program).Assembly.FullName}");

            builder.OpenComponent(0, type);
            builder.AddAttribute(1, "Model", model);
            builder.CloseComponent();
        };
    }
}