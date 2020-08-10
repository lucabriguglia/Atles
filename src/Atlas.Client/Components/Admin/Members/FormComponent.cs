using System.Threading.Tasks;
using Atlas.Models.Admin.Members;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Atlas.Client.Components.Admin.Members
{
    public abstract class FormComponent : AdminComponentBase
    {
        [Parameter] public EditPageModel Model { get; set; }
        [Parameter] public string Button { get; set; }
        [Parameter] public EventCallback OnValidSubmit { get; set; }

        protected EditContext EditContext;
        protected ValidationMessageStore ValidationMessageStore;
        protected string CurrentDisplayName;

        protected override void OnInitialized()
        {
            EditContext = new EditContext(Model.Member);
            EditContext.OnFieldChanged += HandleFieldChanged;
            ValidationMessageStore = new ValidationMessageStore(EditContext);
            CurrentDisplayName = Model.Member.DisplayName;
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            ValidationMessageStore.Clear(e.FieldIdentifier);
        }

        protected async Task OnSubmitAsync()
        {
            if (EditContext.Validate())
            {
                if (await NameIsUniqueAsync(EditContext))
                {
                    await OnValidSubmit.InvokeAsync(null);
                }
                else
                {
                    var fieldIdentifier = new FieldIdentifier(EditContext.Model, "DisplayName");
                    ValidationMessageStore.Clear(fieldIdentifier);
                    ValidationMessageStore.Add(fieldIdentifier, Loc["A member with the same display name already exists."]);
                    EditContext.NotifyValidationStateChanged();
                }
            }
        }

        private async Task<bool> NameIsUniqueAsync(EditContext editContext)
        {
            var displayNameProp = editContext.Model.GetType().GetProperty("DisplayName");
            var displayNameVal = displayNameProp.GetValue(editContext.Model).ToString();

            var isDisplayNameUnique = displayNameVal == CurrentDisplayName || await ApiService.GetFromJsonAsync<bool>($"api/admin/members/is-display-name-unique/{displayNameVal}");

            return isDisplayNameUnique;
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
        }

        protected void Cancel()
        {
            NavigationManager.NavigateTo("/admin/members");
        }
    }
}