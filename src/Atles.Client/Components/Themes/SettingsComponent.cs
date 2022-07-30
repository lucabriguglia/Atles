using System.Threading.Tasks;
using Atles.Models.Public;
using Microsoft.AspNetCore.Components.Forms;

namespace Atles.Client.Components.Themes
{
    public abstract class SettingsComponent : ThemeComponentBase
    {
        protected SettingsPageModel Model { get; set; }
        protected EditContext EditContext { get; set; }
        protected ValidationMessageStore ValidationMessageStore { get; set; }
        protected string CurrentDisplayName { get; set; }

        protected bool Saved { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            Model = await ApiService.GetFromJsonAsync<SettingsPageModel>("api/public/settings/edit");

            EditContext = new EditContext(Model.User);
            EditContext.OnFieldChanged += HandleFieldChanged;
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            CurrentDisplayName = Model.User.DisplayName;
        }

        protected async Task UpdateAsync()
        {
            SavingData = true;
            Saved = false;

            if (EditContext.Validate())
            {
                if (await NameIsUniqueAsync(EditContext))
                {
                    await ApiService.PostAsJsonAsync("api/public/settings/update", Model);
                    SavingData = false;
                    Saved = true;
                    await OnInitializedAsync();
                }
                else
                {
                    SavingData = false;
                    var fieldIdentifier = new FieldIdentifier(EditContext.Model, "DisplayName");
                    ValidationMessageStore.Clear(fieldIdentifier);
                    ValidationMessageStore.Add(fieldIdentifier, Loc["Display name already taken."]);
                    EditContext.NotifyValidationStateChanged();
                }
            }
        }

        protected async Task<bool> NameIsUniqueAsync(EditContext editContext)
        {
            var displayNameProp = editContext.Model.GetType().GetProperty("DisplayName");
            var displayNameVal = displayNameProp.GetValue(editContext.Model).ToString();

            var isDisplayNameUnique = displayNameVal == CurrentDisplayName || await ApiService.GetFromJsonAsync<bool>($"api/public/settings/is-display-name-unique/{displayNameVal}");

            return isDisplayNameUnique;
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