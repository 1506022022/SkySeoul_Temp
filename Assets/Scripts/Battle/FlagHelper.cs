using System;

namespace Battle
{
    public static class FlagHelper
    {
        internal static void AddFlag(ref BodyState flags, BodyState flag)
        {
            flags |= flag;
        }
        internal static void RemoveFlag(ref BodyState flags, BodyState flag)
        {
            flags &= ~flag;
        }
        internal static bool EqualsFlag(BodyState flags, BodyState flag)
        {
            return flags == flag;
        }
        internal static bool HasFlag(BodyState flags, BodyState flag)
        {
            return (flags & flag) != 0;
        }
    }

}
