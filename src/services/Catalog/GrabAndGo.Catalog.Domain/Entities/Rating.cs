namespace GrabAndGo.Catalog.Domain.Entities;

public class Rating
{
    public float OverallRating { get; set; } = 0.0f;
    public int TotalRatings { get; set; } = 0;
    public float CollectionRating { get; set; } = 0.0f;
    public float QualityRating { get; set; } = 0.0f;
    public float VarietyRating { get; set; } = 0.0f;
    public float QuantityRating { get; set; } = 0.0f;
}
