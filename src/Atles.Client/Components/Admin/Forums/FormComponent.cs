using System.Threading.Tasks;
using Atles.Models.Admin.Forums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

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
        private string _currentSlug;

        protected override void OnInitialized()
        {
            EditContext = new EditContext(Model.Forum);
            EditContext.OnFieldChanged += HandleFieldChanged;
            _validationMessageStore = new ValidationMessageStore(EditContext);
            _currentName = Model.Forum.Name;
            _currentSlug = Model.Forum.Slug;
        }

        private void HandleFieldChanged(object sender, FieldChangedEventArgs e)
        {
            _validationMessageStore.Clear(e.FieldIdentifier);
        }

        protected async Task OnSubmitAsync()
        {
            if (EditContext.Validate())
            {
                var nameIsUnique = await NameIsUniqueAsync(EditContext);
                var slugIsUnique = await SlugIsUniqueAsync(EditContext);

                if (nameIsUnique && slugIsUnique)
                {
                    await OnValidSubmit.InvokeAsync(null);
                }
                else
                {
                    if (!nameIsUnique)
                    {
                        var fieldIdentifier = new FieldIdentifier(EditContext.Model, "Name");
                        _validationMessageStore.Clear(fieldIdentifier);
                        _validationMessageStore.Add(fieldIdentifier, Loc["A forum with the same name already exists."]);
                        EditContext.NotifyValidationStateChanged();
                    }

                    if (!slugIsUnique)
                    {
                        var fieldIdentifier = new FieldIdentifier(EditContext.Model, "Slug");
                        _validationMessageStore.Clear(fieldIdentifier);
                        _validationMessageStore.Add(fieldIdentifier, Loc["A forum with the same slug already exists."]);
                        EditContext.NotifyValidationStateChanged();
                    }
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

        private async Task<bool> SlugIsUniqueAsync(EditContext editContext)
        {
            var slugProp = editContext.Model.GetType().GetProperty("Slug");
            var slugVal = slugProp.GetValue(editContext.Model).ToString();

            var isSlugUnique = slugVal == _currentSlug || await ApiService.GetFromJsonAsync<bool>($"api/admin/forums/is-slug-unique/{slugVal}");

            return isSlugUnique;
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