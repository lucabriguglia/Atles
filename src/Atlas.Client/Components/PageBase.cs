using System;
using System.Collections.Generic;
using System.Linq;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components
{
    public abstract class PageBase : ComponentBase
    {
        [CascadingParameter] 
        protected CurrentSiteModel Site { get; set; }

        protected RenderFragment AddComponent(string component, Dictionary<string, object> models = null) => builder =>
        {
            var type = Type.GetType($"Atlas.Client.Themes.{Site.Theme}.{component}, {typeof(Program).Assembly.FullName}");

            builder.OpenComponent(0, type);

            if (models != null)
            {
                for (int i = 0; i < models.Count; i++)
                {
                    var item = models.ElementAt(i);
                    builder.AddAttribute(i + 1, item.Key, item.Value);
                }
            }
            
            builder.CloseComponent();
        };
    }
}