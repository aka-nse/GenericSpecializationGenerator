namespace GenericSpecializationGenerator;

internal static class MemoryEx
{
    public static int IndexOf<T>(
        this ReadOnlySpan<T> sequence,
        T value,
        IEqualityComparer<T> comparer)
    {
        for (var i = 0; i < sequence.Length; ++i)
        {
            if (comparer.Equals(sequence[i], value))
            {
                return i;
            }
        }
        return -1;
    }
}
