
using AutoAtedimento.API.Models;
using FluentValidation;

namespace AutoAtedimento.API.Validator
{

    public class PedidoItemValidator : AbstractValidator<PedidoItemModel>
    {
        public PedidoItemValidator()
        {
            RuleFor(x => x.It_ProdutoId)
                .NotEmpty().WithMessage("O produto é obrigatório.")
                .GreaterThan(0).WithMessage("Produto inválido.");

            RuleFor(x => x.It_Quantidade)
                .NotEmpty().WithMessage("A quantidade é obrigatória.")
                .GreaterThan(0).WithMessage("A quantidade deve ser maior que zero.");
        }
    }
}
