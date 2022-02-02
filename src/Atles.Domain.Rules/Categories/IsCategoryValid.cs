using Atles.Core.Queries;

namespace Atles.Domain.Rules.Categories;

public class IsCategoryValid : QueryBase<bool>
{
    public Guid Id { get; set; }
}