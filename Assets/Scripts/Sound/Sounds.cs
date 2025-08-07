using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Sounds
{
    public static void MakeSound(Sound sound)
    {
        Collider[] col = Physics.OverlapSphere(sound.Pos, sound.Range);

        for (int i = 0; i < col.Length; i++)
        {
            if (col[i].TryGetComponent(out IHear soundListener))
            {
                soundListener.RespondToSound(sound);
                Debug.Log($"Sound made at {sound.Pos} with range {sound.Range}. Responded by: {col[i].name}");
            }
        }
    }
}
