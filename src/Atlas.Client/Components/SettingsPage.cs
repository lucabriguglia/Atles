using System.Threading.Tasks;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components.Forms;

namespace Atlas.Client.Components
{
    public abstract class SettingsPage : PageBase
    {
        protected SettingsPageModel Model { get; set; }

        protected EditContext EditContext { get; set; }
        protected ValidationMessageStore ValidationMessageStore { get; set; }
        protected string CurrentDisplayName { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Model = await ApiService.GetFromJsonAsync<SettingsPageModel>("api/public/settings/edit");

            EditContext = new EditContext(Model.Member);
            EditContext.OnFieldChanged += HandleFieldChanged;
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            CurrentDisplayName = Model.Member.DisplayName;
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            ValidationMessageStore.Clear(e.FieldIdentifier);
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
        }
    }
}