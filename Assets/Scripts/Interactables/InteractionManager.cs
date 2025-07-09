using System.Collections.Generic;

public static class InteractionManager
{
    private static readonly HashSet<InteractionGroup> disabledGroups = new();

    public static bool IsGroupEnabled(InteractionGroup group) => !disabledGroups.Contains(group);

    public static void DisableGroup(InteractionGroup group) => disabledGroups.Add(group);

    public static void EnableGroup(InteractionGroup group) => disabledGroups.Remove(group);

    public static void ResetAll() => disabledGroups.Clear();
}
