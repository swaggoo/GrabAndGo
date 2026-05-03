using GrabAndGo.Catalog.Application.Dtos;
using GrabAndGo.Catalog.Application.Extensions;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public class GetBusinessByIdHandler(IBusinessRepository businessRepository) : IRequestHandler<GetBusinessByIdQuery, BusinessDto?>
{
    public async Task<BusinessDto?> Handle(GetBusinessByIdQuery request, CancellationToken cancellationToken)
    {
        // Try by internal Guid first
        var business = await businessRepository.GetBusiness(request.Id);

        if (business == null)
        {
             // Fallback to checking by BusinessId (Identity User ID)
             business = await businessRepository.GetBusinessById(request.Id);
        }

        return business?.ToDto();
    }
}
