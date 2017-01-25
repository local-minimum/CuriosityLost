using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SpaceShip : MonoBehaviour {

    [SerializeField, Range(1, 4)]
    int widthNeeded;

    [SerializeField, Range(1, 4)]
    int heightNeeded;

    [SerializeField]
    int[] allowedHeights = new int[1] { 0 };

    [SerializeField]
    Vector3 offset = new Vector3(0.1f, 0.2f, 0.1f);

    [SerializeField]
    int edgePadding = 10;

    void OnEnable()
    {
        StepTiler.OnNewWorld += StepTiler_OnNewWorld;
    }

    void OnDisable()
    {
        StepTiler.OnNewWorld -= StepTiler_OnNewWorld;
    }

    void OnDestroy()
    {
        StepTiler.OnNewWorld -= StepTiler_OnNewWorld;
    }

    private void StepTiler_OnNewWorld(StepTiler world)
    {
        List<GridRect> permissablePositions = world.GetAllPaddedPositions(widthNeeded, heightNeeded, edgePadding, allowedHeights).ToList();

        //TODO: Check if really there is any position!

        GridRect landingSpot = permissablePositions[Random.Range(0, permissablePositions.Count)];

        transform.position = world.GridRectToWorld(landingSpot) + offset;

        CamZoomToShip.Zoom();
    }
}
