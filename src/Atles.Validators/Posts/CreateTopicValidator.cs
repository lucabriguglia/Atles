﻿using Atles.Commands.Posts;
using Atles.Core;
using Atles.Domain.Rules.Forums;
using FluentValidation;

namespace Atles.Validators.Posts
{
    public class CreateTopicValidator : AbstractValidator<CreateTopic>
    {
        public CreateTopicValidator(IDispatcher dispatcher)
        {
            RuleFor(c => c.Title)
                .NotEmpty().WithMessage("Topic title is required.")
                .Length(1, 100).WithMessage("Topic title must be at least 1 and at max 100 characters long.");

            RuleFor(c => c.Content)
                .NotEmpty().WithMessage("Topic content is required.");

            RuleFor(c => c.ForumId)
                .MustAsync(async (c, p, cancellation) =>
                {
                    // TODO: To be moved to a service
                    var result = await dispatcher.Get(new IsForumValid {SiteId = c.SiteId, Id = p});
                    return result.AsT0;
                })
                    .WithMessage(c => $"Forum with id {c.ForumId} does not exist.");
        }
    }
}
