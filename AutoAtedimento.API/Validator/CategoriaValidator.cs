using AutoAtedimento.API.Models;
using FluentValidation;

namespace AutoAtedimento.API.Validator
{
    public class CategoriaValidator : AbstractValidator<CategoriaModel>
    {
        public CategoriaValidator()
        {
            RuleFor(x => x.Cat_Nome)
                .NotEmpty()
                .WithMessage("O nome da categoria é obrigatório.");
        }
    }
}