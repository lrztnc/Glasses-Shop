public static class CartService
{
    public static bool[] added = new bool[5];

    public static void Add(int index)
    {
        if (index >= 0 && index < added.Length) added[index] = true;
    }

    public static void Remove(int index)
    {
        if (index >= 0 && index < added.Length) added[index] = false;
    }

    public static void Clear()
    {
        for (int i = 0; i < added.Length; i++) added[i] = false;
    }
}