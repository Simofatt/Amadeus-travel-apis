using FluentValidation;
using Microsoft.Extensions.Localization;
using MudBlazorApp.Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazorApp.Shared.Validators
{


    public class TravelRequestValidator : AbstractValidator<TravelRequest>
    {
        public TravelRequestValidator(IStringLocalizer<TravelRequestValidator> localizer)
        {
            RuleFor(request => request.Origin)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["First Name is required"]);
            RuleFor(request => request.Destination)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Destination is required"]);
            RuleFor(request => request.TravelClass)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Type de classe is required"]);
            RuleFor(request => request.NonStop)
                .Must(x => !string.IsNullOrWhiteSpace(x.ToString())).WithMessage(x => localizer["Non Stop is required"]);
            RuleFor(request => request.DateAller)
                .Must(x => x != null).WithMessage(x => localizer["Date d'aller is required"]);
            RuleFor(request => request.DateRetour)
               .Must(x => x != null).WithMessage(x => localizer["Date de retour is required"]);
            RuleFor(request => request.Adults)
               .NotEmpty().WithMessage(localizer["Nombre de personnes is required"]);
  
        }
    }
}
