namespace MeetingScheduler.Tools;


internal static class SortedListEx
{
    public static int BinarySearch<TKey, TValue>(this SortedList<TKey, TValue> sortedList, TKey value) where TKey : notnull
    {
        return sortedList.Keys.BinarySearch(value);
    }

    // algorithm courtesy of http://referencesource.microsoft.com/#mscorlib/system/collections/generic/arraysorthelper.cs#114ea99d8baee1be
    public static int BinarySearch<T>(this IList<T> list, T value)
    {
        var comparer = Comparer<T>.Default;

        int lo = 0;
        int hi = list.Count - 1;
        while (lo <= hi)
        {
            int i = lo + (hi - lo >> 1);
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
