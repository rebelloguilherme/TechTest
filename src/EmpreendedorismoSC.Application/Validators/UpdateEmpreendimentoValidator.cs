using EmpreendedorismoSC.Application.DTOs;
using EmpreendedorismoSC.Domain.Enums;
using FluentValidation;

namespace EmpreendedorismoSC.Application.Validators;

public class UpdateEmpreendimentoValidator : AbstractValidator<UpdateEmpreendimentoDto>
{
    public UpdateEmpreendimentoValidator()
    {
        RuleFor(x => x.NomeEmpreendimento)
            .NotEmpty().WithMessage("O nome do empreendimento é obrigatório.")
            .MaximumLength(200).WithMessage("O nome do empreendimento deve ter no máximo 200 caracteres.");

        RuleFor(x => x.NomeEmpreendedor)
            .NotEmpty().WithMessage("O nome do empreendedor é obrigatório.")
            .MaximumLength(200).WithMessage("O nome do empreendedor deve ter no máximo 200 caracteres.");

        RuleFor(x => x.Municipio)
            .NotEmpty().WithMessage("O município é obrigatório.")
            .MaximumLength(100).WithMessage("O município deve ter no máximo 100 caracteres.");

        RuleFor(x => x.SegmentoAtuacao)
            .IsInEnum().WithMessage("O segmento de atuação informado é inválido.")
            .NotEqual(default(SegmentoAtuacao)).WithMessage("O segmento de atuação é obrigatório.");

        RuleFor(x => x.Contato)
            .NotEmpty().WithMessage("O contato é obrigatório.")
            .MaximumLength(200).WithMessage("O contato deve ter no máximo 200 caracteres.");
    }
}
