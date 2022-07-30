using FluentValidation;
using FluentValidation.Results;

namespace Atles.Commands.Handlers
{
    /// <summary>
    /// Validator Extensions
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Validates the command and throws an exception if not valid.
        /// </summary>
        /// <param name="validator"></param>
        /// <param name="command"></param>
        /// <typeparam name="TCommand"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ApplicationException"></exception>
        public static async Task ValidateCommand<TCommand>(this IValidator<TCommand> validator, TCommand command)
        {
            if (validator == null)
            {
                throw new ArgumentNullException(nameof(validator));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var validationResult = await validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                throw new ApplicationException(BuildErrorMessage(validationResult.Errors));
            }
        }

        private static string BuildErrorMessage(IEnumerable<ValidationFailure> errors)
        {
            var errorsText = errors.Select(x => $"\r\n - {x.ErrorMessage}").ToArray();
            return $"Validation failed: {string.Join("", errorsText)}";
        }
    }
}
