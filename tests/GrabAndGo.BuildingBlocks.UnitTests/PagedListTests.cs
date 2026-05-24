using GrabAndGo.BuildingBlocks.Pagination;

namespace GrabAndGo.BuildingBlocks.UnitTests;

public class PagedListTests
{
    [Fact]
    public void Constructor_ShouldInitializeMetaDataCorrectly()
    {
        // Arrange
        var items = new List<int> { 1, 2, 3 };
        var totalCount = 10;
        var pageNumber = 2;
        var pageSize = 3;

        // Act
        var pagedList = new PagedList<int>(items, totalCount, pageNumber, pageSize);

        // Assert
        Assert.Equal(items, pagedList);
        Assert.Equal(totalCount, pagedList.MetaData.TotalCount);
        Assert.Equal(pageSize, pagedList.MetaData.PageSize);
        Assert.Equal(pageNumber, pagedList.MetaData.CurrentPage);
        Assert.Equal(4, pagedList.MetaData.TotalPages); // ceil(10/3) = 4
    }

    [Theory]
    [InlineData(1, 4, false, true)]
    [InlineData(2, 4, true, true)]
    [InlineData(4, 4, true, false)]
    [InlineData(1, 1, false, false)]
    public void PaginationMetaData_ShouldCalculateHasPreviousAndHasNextCorrectly(
        int currentPage, int totalPages, bool expectedHasPrevious, bool expectedHasNext)
    {
        // Arrange
        var metaData = new PaginationMetaData
        {
            CurrentPage = currentPage,
            TotalPages = totalPages
        };

        // Assert
        Assert.Equal(expectedHasPrevious, metaData.HasPrevious);
        Assert.Equal(expectedHasNext, metaData.HasNext);
    }
}
