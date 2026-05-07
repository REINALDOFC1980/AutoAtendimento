using AutoAtedimento.API.Models;
using FluentValidation;

namespace AutoAtedimento.API.Validator
{
    public class ProdutoValidator : AbstractValidator<ProdutoModel>
    {
        public ProdutoValidator()
        {
            RuleFor(x => x.Pro_Nome)
                .NotEmpty()
                .WithMessage("O nome do produto é obrigatório.");

            RuleFor(x => x.Pro_Preco)
                .GreaterThan(0)
                .WithMessage("O preço deve ser maior que zero.");

            RuleFor(x => x.Pro_CategoriaId)
            .GreaterThan(0)
            .WithMessage("A categoria é obrigatória.");
        }
    }
}