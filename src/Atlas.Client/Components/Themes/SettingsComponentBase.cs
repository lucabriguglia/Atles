using System.Threading.Tasks;
using Atlas.Models.Public;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Atlas.Client.Components.Themes
{
    public class SettingsComponentBase : ThemeComponentBase
    {
        [Parameter] public SettingsPageModel Model { get; set; }

        protected EditContext EditContext { get; set; }
        protected ValidationMessageStore ValidationMessageStore { get; set; }
        protected string CurrentDisplayName { get; set; }

        protected bool Savings { get; set; }
        protected bool Saved { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            EditContext = new EditContext(Model.Member);
            EditContext.OnFieldChanged += HandleFieldChanged;
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            CurrentDisplayName = Model.Member.DisplayName;
        }

        protected async Task OnSubmitAsync()
        {
            Savings = true;
            Saved = false;

            if (EditContext.Validate())
            {
                if (await NameIsUniqueAsync(EditContext))
                {
                    await ApiService.PostAsJsonAsync("api/public/settings/update", Model);
                    Savings = false;
                    Saved = true;
                    await OnInitializedAsync();
                }
                else
                {
                    Savings = false;
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