using System;

public static class EnumUtils
{
    public static bool TryToEnum<T>(string value, out T result, bool ignoreCase = true) where T : struct, Enum
    {
        return Enum.TryParse(value, ignoreCase, out result) && Enum.IsDefined(typeof(T), result);
    }
}
