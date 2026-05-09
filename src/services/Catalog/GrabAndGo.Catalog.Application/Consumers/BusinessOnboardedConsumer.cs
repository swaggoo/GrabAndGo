using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Catalog.Domain.Entities;
using GrabAndGo.Catalog.Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace GrabAndGo.Catalog.Application.Consumers;

public class BusinessOnboardedConsumer(
    IBusinessRepository businessRepository,
    ILogger<BusinessOnboardedConsumer> logger)
    : IConsumer<BusinessOnboardedEvent>
{
    public async Task Consume(ConsumeContext<BusinessOnboardedEvent> context)
    {
        var @event = context.Message;
        
        logger.LogInformation("Creating Business for Onboarded Event: {BusinessId}", @event.BusinessId);

        var existing = await businessRepository.GetBusinessById(@event.BusinessId);
        if (existing != null)
        {
            logger.LogInformation("Business already exists for ID {BusinessId}. Updating...", @event.BusinessId);
            existing.Name = @event.BusinessName;
            existing.Description = @event.Description;
            existing.LogoUrl = @event.LogoUrl;
            existing.CoverImageUrl = @event.CoverImageUrl;
            existing.Phone = @event.Phone;
            existing.Website = @event.Website;
            existing.TotalOrders = @event.TotalOrders;
            existing.Address = new BusinessAddress
            {
                Street = @event.Street,
                City = @event.City,
                PostalCode = @event.PostalCode,
                Country = @event.Country
            };
            existing.Location = new BusinessLocation
            {
                Latitude = @event.Latitude,
                Longitude = @event.Longitude
            };
            existing.IsActive = true;
            
            await businessRepository.UpdateBusiness(existing);
            return;
        }

        var business = new Business
        {
            BusinessId = @event.BusinessId,
            Name = @event.BusinessName,
            Description = @event.Description,
            LogoUrl = @event.LogoUrl,
            CoverImageUrl = @event.CoverImageUrl,
            Phone = @event.Phone,
            Website = @event.Website,
            Email = @event.Email,
            TotalOrders = @event.TotalOrders,
            Address = new BusinessAddress 
            { 
                Street = @event.Street, 
                City = @event.City, 
                PostalCode = @event.PostalCode, 
                Country = @event.Country 
            },
            Location = new BusinessLocation 
            { 
                Latitude = @event.Latitude, 
                Longitude = @event.Longitude 
            },
            IsActive = true,
            IsVerified = false
        };

        await businessRepository.CreateBusiness(business);
    }
}
