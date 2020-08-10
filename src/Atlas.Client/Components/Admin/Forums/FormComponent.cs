using System.Threading.Tasks;
using Atlas.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Atlas.Client.Components.Admin.Forums
{
    public abstract class FormComponent : AdminComponentBase
    {
        [Parameter] public FormComponentModel Model { get; set; }
        [Parameter] public string Button { get; set; }
        [Parameter] public EventCallback OnValidSubmit { get; set; }

        protected EditContext EditContext;
        private ValidationMessageStore _validationMessageStore;
        private string _currentName;

        protected override void OnInitialized()
        {
            EditContext = new EditContext(Model.Forum);
            EditContext.OnFieldChanged += HandleFieldChanged;
            _validationMessageStore = new ValidationMessageStore(EditContext);
            _currentName = Model.Forum.Name;
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
                    var fieldIdentifier = new FieldIdentifier(EditContext.Model, "Name");
                    _validationMessageStore.Clear(fieldIdentifier);
                    _validationMessageStore.Add(fieldIdentifier, Loc["A forum with the same name already exists."]);
                    EditContext.NotifyValidationStateChanged();
                }
            }
        }

        private async Task<bool> NameIsUniqueAsync(EditContext editContext)
        {
            var nameProp = editContext.Model.GetType().GetProperty("Name");
            var nameVal = nameProp.GetValue(editContext.Model).ToString();

            var isNameUnique = nameVal == _currentName || await ApiService.GetFromJsonAsync<bool>($"api/admin/forums/is-name-unique/{Model.Forum.CategoryId}/{nameVal}");

            return isNameUnique;
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
        }

        protected void Cancel()
        {
            NavigationManager.NavigateTo($"/admin/forums/{Model.Forum.CategoryId}");
        }
    }
}