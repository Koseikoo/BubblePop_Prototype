using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingVisual : MonoBehaviour
{
    [SerializeField] LineRenderer line;

    private void Start()
    {
        GameManager.acc.Shooter.ShootDirectionChanged += AimingDirectionVisual;
    }

    public void AimingDirectionVisual(List<Vector2> path)
    {
        if (path.Count == 0)
        {
            line.enabled = false;
            return;
        }

        line.enabled = true;
        line.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            if (i > 0 && path[i].y < path[i-1].y)
                continue;

            line.SetPosition(i, path[i]);
        }
    }
}
