using System.Threading.Tasks;
using Atles.Models.Admin.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Atlas.Client.Components.Admin.Users
{
    public abstract class FormComponent : AdminComponentBase
    {
        [Parameter] public EditPageModel Model { get; set; }
        [Parameter] public string Button { get; set; }
        [Parameter] public EventCallback OnValidSubmit { get; set; }

        protected EditContext EditContext;
        private ValidationMessageStore _validationMessageStore;
        private string _currentDisplayName;

        protected override void OnInitialized()
        {
            EditContext = new EditContext(Model.User);
            EditContext.OnFieldChanged += HandleFieldChanged;
            _validationMessageStore = new ValidationMessageStore(EditContext);
            _currentDisplayName = Model.User.DisplayName;
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            _validationMessageStore.Clear(e.FieldIdentifier);
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
                    _validationMessageStore.Clear(fieldIdentifier);
                    _validationMessageStore.Add(fieldIdentifier, Loc["A user with the same display name already exists."]);
                    EditContext.NotifyValidationStateChanged();
                }
            }
        }

        private async Task<bool> NameIsUniqueAsync(EditContext editContext)
        {
            var displayNameProp = editContext.Model.GetType().GetProperty("DisplayName");
            var displayNameVal = displayNameProp.GetValue(editContext.Model).ToString();

            var isDisplayNameUnique = displayNameVal == _currentDisplayName || await ApiService.GetFromJsonAsync<bool>($"api/admin/users/is-display-name-unique/{displayNameVal}");

            return isDisplayNameUnique;
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
        }

        protected void Cancel()
        {
            NavigationManager.NavigateTo("/admin/users");
        }
    }
}