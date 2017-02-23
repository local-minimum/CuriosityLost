using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Garderner : MonoBehaviour {

    [SerializeField]
    Discoverable[] prefabs;

    [SerializeField]
    int[] abundancies;

    [SerializeField]
    Transform gardenParent;

    [SerializeField]
    float yOffset = 0.15f;

    Dictionary<string, List<Discoverable>> objectCache = new Dictionary<string, List<Discoverable>>();

    void Start()
    {
        if (gardenParent == null)
        {
            gardenParent = transform;
        }
    }

    void OnEnable()
    {
        StepTiler.OnNewWorld += StepTiler_OnNewWorld;
    }

    void OnDisable()
    {
        StepTiler.OnNewWorld -= StepTiler_OnNewWorld;
    }

    private void StepTiler_OnNewWorld(StepTiler world)
    {
        for (int i=0; i<prefabs.Length; i++)
        {
            Discoverable prefab = prefabs[i];
            GridRect[] positions = world.GetAllPositions(prefab.worldEntity.size, prefab.worldEntity.size, -1, 0, 1, 2).OrderBy(a => Random.value).ToArray();

            for (int j=0, l=abundancies[i]; j< l; j++)
            {
                Discoverable d = GetDiscoverableInstance(prefab);
                //Translate object so pivot point is at center position
                SpriteRenderer sr = d.GetComponent<SpriteRenderer>();
                Vector2 pivot = sr.sprite.pivot;
                pivot.x /= sr.sprite.rect.size.x;
                pivot.y /= sr.sprite.rect.size.y;
                Debug.Log(pivot);
                d.transform.position = world.GridRectToWorld(positions[j]) + Vector3.up * yOffset + d.transform.TransformVector(new Vector3(-pivot.x, -pivot.y, 0));

                world.Occupy(positions[j]);
                d.worldEntity.gridPosition = positions[j].Center;
            }
        }
    }

    Discoverable GetDiscoverableInstance(Discoverable discoverablePrefab)
    {
        Discoverable instance = null;
        if (objectCache.ContainsKey(discoverablePrefab.typeName))
        {
            if (objectCache[discoverablePrefab.typeName].Count > 0)
            {
                instance = objectCache[discoverablePrefab.typeName][0];
                objectCache[discoverablePrefab.typeName].RemoveAt(0);
                instance.gameObject.SetActive(true);
                instance.discovered = false;
            }
        }

        if (instance == null)
        {
            instance = Instantiate(discoverablePrefab);
            instance.transform.SetParent(gardenParent);
        }

        return instance;
    }

    void RemoveRemainingDisabledObjects()
    {
        foreach(string key in objectCache.Keys)
        {
            for (int i=0,l=objectCache[key].Count; i< l; i++)
            {
                Destroy(objectCache[key][i]);
            }
        }

        objectCache.Clear();
    }

    void DisableGardenObjects()
    {
        RemoveRemainingDisabledObjects();

        foreach(Transform t in gardenParent)
        {
            Discoverable d = t.GetComponent<Discoverable>();
            if (d != null)
            {
                if (!objectCache.ContainsKey(d.typeName))
                {
                    objectCache[d.typeName] = new List<Discoverable>();
                }
                objectCache[d.typeName].Add(d);
                t.gameObject.SetActive(false);
            }
        }
    }
}
