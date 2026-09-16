using UnityEngine;

public class TrapManager : MonoBehaviour
{
    [SerializeField] private FlameTrap[] traps;

    public void StopAllTraps()
    {
        if (traps == null)
            return;

        for (int i = 0; i < traps.Length; i++)
            traps[i]?.StopTrap();
    }
}
