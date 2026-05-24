using System.Linq.Expressions;
using GrabAndGo.BuildingBlocks.Specifications;

namespace GrabAndGo.BuildingBlocks.UnitTests;

public class BaseSpecificationTests
{
    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    private class TestSpecification : BaseSpecification<TestEntity>
    {
        public TestSpecification(Expression<Func<TestEntity, bool>> criteria) : base(criteria)
        {
        }

        public void TestAddInclude(Expression<Func<TestEntity, object>> includeExpression)
        {
            AddInclude(includeExpression);
        }

        public void TestAddOrderBy(Expression<Func<TestEntity, object>> orderByExpression)
        {
            AddOrderBy(orderByExpression);
        }

        public void TestAddOrderByDescending(Expression<Func<TestEntity, object>> orderByDescendingExpression)
        {
            AddOrderByDescending(orderByDescendingExpression);
        }

        public void TestApplyPaging(int skip, int take)
        {
            ApplyPaging(skip, take);
        }
    }

    [Fact]
    public void Constructor_ShouldSetCriteria()
    {
        // Arrange
        Expression<Func<TestEntity, bool>> criteria = x => x.Id == 1;

        // Act
        var spec = new TestSpecification(criteria);

        // Assert
        Assert.Equal(criteria, spec.Criteria);
    }

    [Fact]
    public void AddInclude_ShouldAddIncludeExpression()
    {
        // Arrange
        var spec = new TestSpecification(x => true);
        Expression<Func<TestEntity, object>> include = x => x.Name;

        // Act
        spec.TestAddInclude(include);

        // Assert
        Assert.Single(spec.Includes);
        Assert.Equal(include, spec.Includes[0]);
    }

    [Fact]
    public void AddOrderBy_ShouldSetOrderBy()
    {
        // Arrange
        var spec = new TestSpecification(x => true);
        Expression<Func<TestEntity, object>> orderBy = x => x.Name;

        // Act
        spec.TestAddOrderBy(orderBy);

        // Assert
        Assert.Equal(orderBy, spec.OrderBy);
    }

    [Fact]
    public void AddOrderByDescending_ShouldSetOrderByDescending()
    {
        // Arrange
        var spec = new TestSpecification(x => true);
        Expression<Func<TestEntity, object>> orderByDesc = x => x.Name;

        // Act
        spec.TestAddOrderByDescending(orderByDesc);

        // Assert
        Assert.Equal(orderByDesc, spec.OrderByDescending);
    }

    [Fact]
    public void ApplyPaging_ShouldSetPagingProperties()
    {
        // Arrange
        var spec = new TestSpecification(x => true);
        int skip = 10;
        int take = 5;

        // Act
        spec.TestApplyPaging(skip, take);

        // Assert
        Assert.True(spec.IsPagingEnabled);
        Assert.Equal(skip, spec.Skip);
        Assert.Equal(take, spec.Take);
    }
}
