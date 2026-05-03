using GrabAndGo.Catalog.Application.Dtos;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public record GetBusinessByIdQuery(string Id) : IRequest<BusinessDto?>;
