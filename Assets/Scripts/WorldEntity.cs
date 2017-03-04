using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEntity : MonoBehaviour {

    [SerializeField]
    bool showWorldGizmo = false;

    public Vector2 gridPosition;

    [Range(1, 10)]
    public int size = 1;

    public bool blockingTile;

    void OnDrawGizmos()
    {
        if (showWorldGizmo)
        {
            Gizmos.color = Color.red;
            Bounds b = StepTiler.instance.GridWithSizeToWorldBounds(gridPosition, size, true);
            Vector3 boxSize = b.size;
            if (boxSize.y == 0)
            {
                boxSize.y = .01f;
            }
            
            Gizmos.DrawWireCube(b.center, boxSize);
        }
    }

    public void ColorMyTile()
    {

        StepTiler.instance.SetColor(gridPosition, Color.white);

    }

    public void ScanSource(float duration, float speed, float rollOff)
    {
        StepTiler.instance.ScanFrom(gridPosition, duration, speed, rollOff);
    }

    public void ScanSource(Vector3 toPosition, float speed, float rollOff)
    {
        StepTiler.instance.ScanFrom(gridPosition, toPosition, speed, rollOff);
    }
}
