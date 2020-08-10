using System.Threading.Tasks;
using Atlas.Models.Admin.Categories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Atlas.Client.Components.Admin.Categories
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
            EditContext = new EditContext(Model.Category);
            EditContext.OnFieldChanged += HandleFieldChanged;
            _validationMessageStore = new ValidationMessageStore(EditContext);
            _currentName = Model.Category.Name;
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
                    _validationMessageStore.Add(fieldIdentifier, Loc["A category with the same name already exists."]);
                    EditContext.NotifyValidationStateChanged();
                }
            }
        }

        private async Task<bool> NameIsUniqueAsync(EditContext editContext)
        {
            var nameProp = editContext.Model.GetType().GetProperty("Name");
            var nameVal = nameProp.GetValue(editContext.Model).ToString();

            var isNameUnique = nameVal == _currentName || await ApiService.GetFromJsonAsync<bool>($"api/admin/categories/is-name-unique/{nameVal}");

            return isNameUnique;
        }

        public void Dispose()
        {
            EditContext.OnFieldChanged -= HandleFieldChanged;
        }

        protected void Cancel()
        {
            NavigationManager.NavigateTo("/admin/categories");
        }
    }
}