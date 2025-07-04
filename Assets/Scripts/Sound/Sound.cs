using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound
{
    public Sound(Vector3 pos, float range)
    {
        Pos = pos;
        Range = range;
    }

    public readonly Vector3 Pos;
    public readonly float Range;
}
