using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ShipEventType {Landed, Departed};
public delegate void SpaceShipEvent(SpaceShip ship, ShipEventType eventType);

public class SpaceShip : MonoBehaviour {

    static SpaceShip _ship;

    public static SpaceShip ship
    {
        get
        {
            if (_ship == null) { _ship = FindObjectOfType<SpaceShip>(); }
            return _ship;
        }
    }

    public event SpaceShipEvent OnShipEvent;

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

    public Vector2 disembarkmentOffset;

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

    private StepTiler _planet;
    public StepTiler planet
    {
        get
        {
            return _planet;
        }
    }

    GridRect _landingSpot;
    public GridRect landingSpot
    {
        get
        {
            return _landingSpot;
        }
    }

    private void StepTiler_OnNewWorld(StepTiler world)
    {
        _planet = world;
        List<GridRect> permissablePositions = world.GetAllPaddedPositions(widthNeeded, heightNeeded, edgePadding, allowedHeights).ToList();

        //TODO: Check if really there is any position!

        _landingSpot = permissablePositions[Random.Range(0, permissablePositions.Count)];

        transform.position = world.GridRectToWorld(_landingSpot) + offset;

        CamZoomToShip.Zoom();

        if (OnShipEvent != null)
        {
            OnShipEvent(this, ShipEventType.Landed);
        }
    }

    void Awake()
    {
        if (_ship == null || _ship == this)
        {
            _ship = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

    }
}
