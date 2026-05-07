using AutoAtedimento.API.Models;
using FluentValidation;

namespace AutoAtedimento.API.Validator
{
    public class PedidoValidator : AbstractValidator<PedidoModel>
    {
        public PedidoValidator()
        {
            RuleFor(x => x.Ped_MesaId)
                .NotEmpty().WithMessage("A mesa é obrigatória.")
                .GreaterThan(0).WithMessage("Mesa inválida.");

            RuleFor(x => x.Itens)
                .NotNull().WithMessage("O pedido deve conter itens.")
                .Must(x => x.Count > 0).WithMessage("O pedido deve conter pelo menos um item.");

            RuleForEach(x => x.Itens)
                .SetValidator(new PedidoItemValidator());

            RuleFor(x => x.Ped_Observacao)
                .MaximumLength(300).WithMessage("Observação muito longa.");
        }
    }
}
