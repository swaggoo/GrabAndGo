using GrabAndGo.Catalog.Application.Dtos;
using MediatR;

namespace GrabAndGo.Catalog.Application.Queries;

public record GetCategoriesQuery() : IRequest<IEnumerable<CategoryDto>>;
