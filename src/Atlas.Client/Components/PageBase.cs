using System;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components
{
    public abstract class PageBase : ComponentBase
    {
        [CascadingParameter] 
        protected CurrentSiteModel Site { get; set; }

        protected RenderFragment AddComponent(string component, object model) => builder =>
        {
            var type = Type.GetType($"Atlas.Client.Themes.{Site.Theme}.{component}, {typeof(Program).Assembly.FullName}");

            builder.OpenComponent(0, type);
            builder.AddAttribute(1, "Model", model);
            builder.CloseComponent();
        };
    }
}