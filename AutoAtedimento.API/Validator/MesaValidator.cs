using AutoAtedimento.API.Models;
using FluentValidation;

namespace AutoAtedimento.API.Validator
{
    public class MesaValidator : AbstractValidator<MesaModel>
    {
        public MesaValidator()
        {
            RuleFor(x => x.Mes_Numero)
                .GreaterThan(0)
                .WithMessage("Número da mesa inválido.");
        }
    }
}