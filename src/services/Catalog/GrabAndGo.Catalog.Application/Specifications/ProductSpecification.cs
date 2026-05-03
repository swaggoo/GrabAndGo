using GrabAndGo.BuildingBlocks.Specifications;
using GrabAndGo.Catalog.Domain.Entities;

namespace GrabAndGo.Catalog.Application.Specifications;

public class ProductSpecification : BaseSpecification<Product>
{
    public ProductSpecification(string? businessId, string? categoryId, bool? isActive) 
        : base(x => 
            (string.IsNullOrEmpty(businessId) || x.BusinessId == businessId) &&
            (string.IsNullOrEmpty(categoryId) || x.CategoryId == categoryId) &&
            (!isActive.HasValue || x.IsActive == isActive))
    {
        AddInclude(x => x.Business);
    }

    public void ApplySorting(string? sort)
    {
        if (string.IsNullOrEmpty(sort)) return;

        switch (sort.ToLower())
        {
            case "price":
                AddOrderBy(x => x.Price);
                break;
            case "-price":
                AddOrderByDescending(x => x.Price);
                break;
            case "name":
                AddOrderBy(x => x.Name);
                break;
            case "-name":
                AddOrderByDescending(x => x.Name);
                break;
        }
    }
}
