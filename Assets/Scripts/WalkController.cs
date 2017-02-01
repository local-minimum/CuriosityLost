﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpacerMode {Standing, Walking, Jumping, Investigating};
public delegate void Disembark();

public class WalkController : MonoBehaviour {

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

    void Awake()
    {
        ship = SpaceShip.ship;
        worldEntity = GetComponent<WorldEntity>();
        
    }

    void Start()
    {
        col = GetComponent<Collider>();
        sRend = GetComponent<SpriteRenderer>();
        sRend.enabled = false;
        SetIsAnimated();        
    }

    void OnEnable()
    {
        ship.OnShipEvent += Ship_OnShipEvent;
    }

    void OnDisable()
    {
        ship.OnShipEvent -= Ship_OnShipEvent;
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

    void SetCharacterPosition()
    {
        Vector3 pos = ship.planet.GridPositionToWorld(worldEntity.gridPosition);
        pos.y = ship.planet.WorldPosHeight(feetPosition.position);
        transform.position = pos + Vector3.up * heightOverGround;
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

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 100, walkMask))
            {
                walkTarget = ship.planet.WorldToFloatPosition(hit.point);
                proximityThreshold = (walkTarget - worldEntity.gridPosition).magnitude * .15f;
                if (proximityThreshold < 5)
                {
                    proximityThreshold = 0.01f;
                }
                SetSpacerMode(SpacerMode.Walking);

            }
        }

        if (spacerMode == SpacerMode.Walking)
        {
            float delta = (walkTarget - worldEntity.gridPosition).magnitude;
            if (delta < proximityThreshold)
            {
                SetSpacerMode(SpacerMode.Standing);
            } else
            {
                Vector2 direction = (walkTarget - worldEntity.gridPosition).normalized;
                Vector2 walk = direction * walkSpeed * Time.deltaTime;
                if (walk.magnitude > delta)
                {
                    walk = walk.normalized * delta;
                }
                worldEntity.gridPosition += walk;
                SetCharacterPosition();
            }
        }

        if (animatedState)
        {
            SetSprite();
        }

        if (EnterDiscoverable() && !selectedDiscoverable.discovered)
        {
            MessageBar.instance.Prompt("INVESTIGATE?");
        }
    }

    Discoverable selectedDiscoverable = null;

    bool EnterDiscoverable()
    {
        Bounds b = col.bounds;
        Collider[] overlaps = Physics.OverlapBox(b.center, b.size / 2f, Quaternion.identity, discoverableCasting);
        Discoverable firstFound = null;

        for (int i=0; i<overlaps.Length; i++)
        {
            Discoverable tmp = overlaps[i].GetComponent<Discoverable>();
            if (tmp == null)
            {
                continue;
            }

            if (selectedDiscoverable == null)
            {
                selectedDiscoverable = tmp;
                return true;
            } else if (selectedDiscoverable == tmp)
            {
                return false;
            } else if (firstFound == null)
            {
                firstFound = tmp;
            }
        }
        if (firstFound == null && selectedDiscoverable != null && !selectedDiscoverable.discovered && !MessageBar.instance.showing)
        {
            MessageBar.instance.Prompt("YEAH LETS NOT...");
        }
        selectedDiscoverable = firstFound;
        return firstFound != null;
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

    void SetSpacerMode(SpacerMode mode)
    {
        if (spacerMode == mode)
        {
            return;
        }

        spacerMode = mode;
        animationStart = Time.timeSinceLevelLoad;
        SetIsAnimated();
        SetSprite();        
    }
}