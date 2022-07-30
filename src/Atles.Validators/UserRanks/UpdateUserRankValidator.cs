using Atles.Commands.UserRanks;
using Atles.Core;
using FluentValidation;

namespace Atles.Validators.UserRanks;

public class UpdateUserRankValidator : AbstractValidator<UpdateUserRank>
{
    private readonly IDispatcher _dispatcher;

    public UpdateUserRankValidator(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
}