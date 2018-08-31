using System;
using System.Linq;
using System.Net.Mail;
using FluentValidation;
using OrdersApi.Models;

namespace OrdersApi.Validators
{
    public class AddOrderModelValidator : AbstractValidator<AddOrderModel>
    {
        public const string FormatErrorMessageEmailNotValid = "'{0}' is not a valid email.";

        public AddOrderModelValidator()
        {
            RuleFor(x => x.Email)
                .Must(BeValidEmail)
                .WithMessage(y => string.Format(FormatErrorMessageEmailNotValid, y.Email));
        }

        private bool BeValidEmail(string emailAddress)
        {
            try
            {
                var email = new MailAddress(emailAddress);
                return email.Address == emailAddress;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}