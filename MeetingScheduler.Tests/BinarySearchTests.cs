using MeetingScheduler.Tools;

namespace MeetingScheduler.Tests;


public class BinarySearchTests
{
    private const int NumbersCount = 5;

    [Theory]
    [MemberData(nameof(EvenNums))]
    [MemberData(nameof(OddNumbers))]
    public void FindExactIndex(int index, IList<int> list)
    {
        Assert.Equal(index, list.BinarySearch(list[index]));
    }
    
    [Theory]
    [MemberData(nameof(EvenNums))]
    [MemberData(nameof(OddNumbers))]
    public void FindNearestFromAboveIndex(int index, IList<int> list)
    {
        Assert.Equal(index, ~list.BinarySearch(list[index] - 1));
    }
    
    [Theory]
    [MemberData(nameof(EvenNums))]
    [MemberData(nameof(OddNumbers))]
    public void FindNearestFromBelowIndex(int index, IList<int> list)
    {
        Assert.Equal(index + 1, ~list.BinarySearch(list[index] + 1));
    }

    public static IEnumerable<object[]> OddNumbers()
    {
        var numIndexes = Enumerable.Range(0, NumbersCount);
        var nums = numIndexes.Select(t => 1 + t * 2).ToList();

        return numIndexes
            .Select(t => new object[] { t, nums });
    }

    public static IEnumerable<object[]> EvenNums()
    {
        var numIndexes = Enumerable.Range(0, NumbersCount);
        var nums = numIndexes.Select(t => t * 2).ToList();

        return numIndexes
            .Select(t => new object[] { t, nums });
    }
}
