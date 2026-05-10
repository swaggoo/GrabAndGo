using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using MediatR;

namespace GrabAndGo.Catalog.Application.Commands;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _repository;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteProductHandler(IProductRepository repository, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.DeleteProduct(request.Id);
        if (result)
        {
            await _publishEndpoint.Publish(new ProductDeletedEvent(Guid.Parse(request.Id)), cancellationToken);
        }
        return result;
    }
}
