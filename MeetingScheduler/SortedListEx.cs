namespace MeetingScheduler;


internal static class SortedListEx
{
    public static int BinarySearch<TKey, TValue>(this SortedList<TKey, TValue> sortedList, TKey value) where TKey : notnull
    {
        return BinarySearch(sortedList.Keys, 0, sortedList.Count, value);
    }

    // algorithm courtesy of http://referencesource.microsoft.com/#mscorlib/system/collections/generic/arraysorthelper.cs#114ea99d8baee1be
    public static int BinarySearch<T>(this IList<T> list, int index, int length, T value)
    {
        var comparer = Comparer<T>.Default;

        int lo = index;
        int hi = index + length - 1;
        while (lo <= hi)
        {
            int i = lo + ((hi - lo) >> 1);
            int order = comparer.Compare(list[i], value);

            if (order == 0) return i;
            if (order < 0)
                lo = i + 1;
            else
                hi = i - 1;
        }
        return ~lo;
    }
}
