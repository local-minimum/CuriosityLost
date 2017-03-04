using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpacerMode {Standing, Walking, Jumping, Investigating};
public delegate void Disembark();
public delegate void ModeChange(SpacerMode mode);

public class WalkController : MonoBehaviour {

    public event ModeChange OnModeChange;
    public event Disembark OnDisembark;

    WorldEntity worldEntity;

    [SerializeField]
    Sprite[] walkCycle;

    [SerializeField]
    Sprite[] standingCycle;

    [SerializeField]
    Sprite[] investigateCycle;

    [SerializeField]
    Sprite[] jumpCycle;

    [SerializeField]
    LayerMask walkMask;
    
    SpacerMode spacerMode = SpacerMode.Standing;

    [SerializeField]
    float animationSpeed = 2f;

    float animationStart = 0f;
    bool animatedState = false;
    SpriteRenderer sRend;

    float heightOverGround = 0.24f;

    SpaceShip ship;

    [SerializeField, Range(0, 2)]
    float disembarkmentDelay;

    [SerializeField]
    Transform feetPosition;

    Collider col;

    [SerializeField]
    LayerMask discoverableCasting;

    SelectController selectController;

    void Awake()
    {
        ship = SpaceShip.ship;
        worldEntity = GetComponent<WorldEntity>();
        selectController = GetComponent<SelectController>();
        
    }

    void Start()
    {
        col = GetComponent<Collider>();
        sRend = GetComponent<SpriteRenderer>();
        sRend.enabled = false;
        SetIsAnimated();
        DontDestroyOnLoad(gameObject);       
    }

    void OnEnable()
    {
        ship.OnShipEvent += Ship_OnShipEvent;
        Story_UI.instance.OnStoryAccept += Instance_OnStoryAccept;
    }

    void OnDisable()
    {
        ship.OnShipEvent -= Ship_OnShipEvent;
        try {
            Story_UI.instance.OnStoryAccept -= Instance_OnStoryAccept;
        } catch (System.NullReferenceException)
        {
            //This is OK
        }
    }

    private void Instance_OnStoryAccept()
    {
        SetSpacerMode(SpacerMode.Standing);
    }

    private void Ship_OnShipEvent(SpaceShip ship, ShipEventType eventType)
    {
        if (eventType == ShipEventType.Landed)
        {
            StartCoroutine(Disembark());
        }
    }

    IEnumerator<WaitForSeconds> Disembark()
    {
        yield return new WaitForSeconds(disembarkmentDelay);

        worldEntity.gridPosition = ship.disembarkmentOffset + ship.landingSpot.Center;

        SetSpacerMode(SpacerMode.Standing);   
        SetCharacterPosition();

        sRend.enabled = true;  
        
        if (OnDisembark != null)
        {
            OnDisembark();
        }      
    }

    bool SetCharacterPosition()
    {
        bool onMap = true;
        Vector3 pos = Vector3.one;
        try {
            pos = ship.planet.GridPositionToWorld(worldEntity.gridPosition);
        } catch (OutsideMapException)
        {
            onMap = false;
        } finally
    	{
            if (onMap)
            {
                pos.y = ship.planet.WorldPosHeight(feetPosition.position);
                transform.position = pos + Vector3.up * heightOverGround;
                worldEntity.ColorMyTile();
            }
        }

        return onMap;
    }

    void SetSprite() {
        float progress = ((Time.timeSinceLevelLoad - animationStart) % animationSpeed) / animationSpeed;
        sRend.sprite = GetAnimatedSprite(progress);
    }

    int spriteIndex;

    Sprite GetAnimatedSprite(float progress)
    {        
        bool increase = false;
        switch (spacerMode)
        {
            case SpacerMode.Walking:
                increase = walkCycle.Length * progress > 1;
                break;
            case SpacerMode.Standing:
                increase = standingCycle.Length * progress > 1;
                break;
            case SpacerMode.Investigating:
                increase = investigateCycle.Length * progress > 1;
                break;
            case SpacerMode.Jumping:
                increase = jumpCycle.Length * progress > 1;
                break;
        }

        if (increase)
        {
            spriteIndex++;
            animationStart = Time.timeSinceLevelLoad;
        }

        switch (spacerMode)
        {
            case SpacerMode.Walking:
                return walkCycle[spriteIndex % walkCycle.Length];
            case SpacerMode.Standing:
                return standingCycle[spriteIndex % standingCycle.Length];
            case SpacerMode.Investigating:
                return investigateCycle[spriteIndex % investigateCycle.Length];
            case SpacerMode.Jumping:
                return jumpCycle[spriteIndex % jumpCycle.Length];
        }
        return null;
    }

    Vector2 walkTarget;
    float proximityThreshold = 0;

    [SerializeField]
    float walkSpeed = 1f;

    [SerializeField]
    float scanSpeed = 4.3f;

    [SerializeField]
    float scanRollOff = 2f;

    void Update()
    {
        if (spacerMode != SpacerMode.Investigating)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool doInvestigate = false;
                if (Physics.Raycast(r, out hit, 100, discoverableCasting))
                {
                    if (selectedDiscoverable == hit.transform.GetComponent<Discoverable>())
                    {
                        doInvestigate = true;
                        SetSpacerMode(SpacerMode.Investigating);
                        selectedDiscoverable.Investigate();
                        selectController.ClickItem(selectedDiscoverable.gameObject);
                        //selectedDiscoverable.worldEntity.ColorMyTile();
                        selectedDiscoverable = null;
                    }
                }

                if (!doInvestigate && Physics.Raycast(r, out hit, 100, walkMask))
                {
                    walkTarget = ship.planet.WorldToFloatPosition(hit.point);
                    proximityThreshold = (walkTarget - worldEntity.gridPosition).magnitude * .15f;
                    if (proximityThreshold < 5)
                    {
                        proximityThreshold = 0.01f;
                    }
                    SetSpacerMode(SpacerMode.Walking);
                    selectController.ClickLocation(hit.point);

                }
            }

            if (Input.GetMouseButtonUp(1) || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Scan requested");
                
                worldEntity.ScanSource(ship.transform.position, scanSpeed, scanRollOff);
            }

            if (spacerMode == SpacerMode.Walking)
            {
                float delta = (walkTarget - worldEntity.gridPosition).magnitude;
                if (delta < proximityThreshold)
                {
                    SetSpacerMode(SpacerMode.Standing);
                }
                else
                {
                    Vector2 direction = WalkDirection;

                    Vector2 walk = direction * walkSpeed * Time.deltaTime;
                    if (walk.magnitude > delta)
                    {
                        walk = walk.normalized * delta;
                    }
                    worldEntity.gridPosition += walk;
                    if (!SetCharacterPosition())
                    {
                        worldEntity.gridPosition -= walk;
                        SetSpacerMode(SpacerMode.Standing);
                    }
                }
            }
        }

        if (animatedState)
        {
            SetSprite();
        }

        if (EnterCollider<Discoverable>(discoverableCasting, TestDiscoverable, RespondDiscoverable) && !selectedDiscoverable.discovered)
        {
            MessageBar.instance.Prompt("INVESTIGATE?");
        }
    }

    [SerializeField]
    LayerMask obstacleCasting;

    Vector2 WalkDirection
    {
        get
        {
            Vector2 direction = (walkTarget - worldEntity.gridPosition).normalized;
            Bounds b = col.bounds;
            Collider[] overlaps = Physics.OverlapBox(b.center, b.size / 2f, Quaternion.identity, obstacleCasting);
            for (int i = 0; i < overlaps.Length; i++)
            {
                WorldEntity we = overlaps[i].GetComponent<WorldEntity>();
                if (we == null || we.blockingTile)
                {
                    Vector3 oCenter = overlaps[i].transform.position - transform.position;
                    Vector2 oDirection = new Vector2(oCenter.x, oCenter.z).normalized;
                    float turnMagnitude = Vector2.Dot(direction, oDirection);
                    if (turnMagnitude > 0)
                    {
                        Vector2 delta = direction - oDirection;
                        if (turnMagnitude == 1f)
                        {
                            delta = new Vector2(direction.y, direction.x * (Random.value < 0.5f ? -1f : 1f));
                        }
                        else
                        {
                            delta = delta.normalized;
                        }
                        direction = (direction + (delta * turnMagnitude)).normalized;
                    }
                }
            }
            return direction;
        }
    }

    Discoverable selectedDiscoverable = null;

    public string DiscoverableKey
    {
        get
        {
            if (selectedDiscoverable)
            {
                return selectedDiscoverable.typeName;
            }
            return null;
        }
    }

    enum CollisionRespons { Continue, True, False, CarryOn};

    CollisionRespons TestDiscoverable(Discoverable tmp)
    {
        if (tmp == null || tmp.discovered)
        {
            return CollisionRespons.Continue;
        }

        if (selectedDiscoverable == null)
        {
            selectedDiscoverable = tmp;
            return CollisionRespons.True;
        }
        else if (selectedDiscoverable == tmp)
        {
            return CollisionRespons.False;
        }

        return CollisionRespons.CarryOn;
    }

    bool RespondDiscoverable(Discoverable firstFound)
    {
        if (firstFound == null && selectedDiscoverable != null && !selectedDiscoverable.discovered && !MessageBar.instance.showing)
        {
            MessageBar.instance.Prompt("YEAH LETS NOT...");
        }
        selectedDiscoverable = firstFound;
        return firstFound != null;
    }

    bool EnterCollider<T>(LayerMask mask, System.Func<T,CollisionRespons> test, System.Func<T, bool> response) where T : MonoBehaviour
    {
        Bounds b = col.bounds;
        Collider[] overlaps = Physics.OverlapBox(b.center, b.size / 2f, Quaternion.identity, mask);
        T firstFound = null;

        for (int i=0; i<overlaps.Length; i++)
        {
            T tmp = overlaps[i].GetComponent<T>();

            switch (test(tmp))
            {
                case CollisionRespons.Continue:
                    continue;
                case CollisionRespons.True:
                    return true;
                case CollisionRespons.False:
                    return false;
            }
                        
            if (firstFound == null)
            {
                firstFound = tmp;
            }
        }

        return response(firstFound);
    }

    void SetIsAnimated()
    {
        switch (spacerMode)
        {
            case SpacerMode.Walking:
                animatedState = walkCycle.Length > 1;
                break;
            case SpacerMode.Standing:
                animatedState = standingCycle.Length > 1;
                break;
            case SpacerMode.Investigating:
                animatedState = investigateCycle.Length > 1;
                break;
            case SpacerMode.Jumping:
                animatedState = jumpCycle.Length > 1;
                break;
        }
    }

    public void SetSpacerMode(SpacerMode mode)
    {
        if (spacerMode == mode)
        {
            return;
        }

        spacerMode = mode;
        animationStart = Time.timeSinceLevelLoad;
        SetIsAnimated();
        SetSprite();
        
        if (OnModeChange != null)
        {
            OnModeChange(mode);
        }
    }
}
