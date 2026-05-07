
using AutoAtedimento.API.Models;
using FluentValidation;

namespace AutoAtedimento.API.Validator
{
    public class AtualizarStatusValidator : AbstractValidator<AtualizarStatusModel>
    {
        public AtualizarStatusValidator()
        {

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Status inválido.");
        }
    }
}