using Atles.Commands.UserRanks;
using Atles.Core;
using FluentValidation;

namespace Atles.Domain.Commands.Handlers.UserRanks.Validators;

public class UpdateUserRankValidator : AbstractValidator<UpdateUserRank>
{
    private readonly IDispatcher _dispatcher;

    public UpdateUserRankValidator(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
}