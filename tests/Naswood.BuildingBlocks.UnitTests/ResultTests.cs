using Naswood.BuildingBlocks.Domain;

namespace Naswood.BuildingBlocks.UnitTests;

public class ResultTests
{
    [Fact]
    public void Success_exposes_value()
    {
        var result = Result.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public void Failure_exposes_error_and_blocks_value()
    {
        var error = Error.Validation("PLT-001", "Invalid input.");
        var result = Result.Failure<int>(error);

        Assert.True(result.IsFailure);
        Assert.Equal(error, result.Error);
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }
}

public class ValueObjectTests
{
    [Fact]
    public void Equal_value_objects_are_equal()
    {
        var left = new SampleValue("A", 1);
        var right = new SampleValue("A", 1);

        Assert.Equal(left, right);
        Assert.Equal(left.GetHashCode(), right.GetHashCode());
    }

    [Fact]
    public void Different_value_objects_are_not_equal()
    {
        var left = new SampleValue("A", 1);
        var right = new SampleValue("B", 1);

        Assert.NotEqual(left, right);
    }

    private sealed class SampleValue : ValueObject
    {
        public SampleValue(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }

        public string Name { get; }

        public int Amount { get; }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
            yield return Amount;
        }
    }
}
