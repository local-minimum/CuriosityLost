using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanAnimate : MonoBehaviour {

    [SerializeField]
    bool debugging;

    bool over = false;

    [SerializeField]
    float speed = 1f;

    [SerializeField]
    float duration = 10;

    static bool waving = false;

    static Vector4 curWave;

    static void SetWaveOrigin(Vector3 pos)
    {
        curWave.Set(pos.x, pos.y, pos.y, -1);
        UpdateMaterials();
    }

    static void SetWaveDuration(float t)
    {
        curWave.w = t;
        UpdateMaterials();
    }

    static void UpdateMaterials()
    {

        for (int i = 0; i < nRends; i++)
        {
            waveRenderers[i].material.SetVector("_ScanOrigin", curWave);
        }        
    }


    static List<Renderer> waveRenderers = new List<Renderer>();
    static float nRends = 0;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            waveRenderers.Add(rend);
            nRends++;
        }
    }

	void Update () {
		if (debugging && over && Input.GetMouseButtonDown(0))
        {
            Scan();
        }
	}

    public void Scan()
    {
        StartCoroutine(Wave());
    }

    IEnumerator<WaitForSeconds> Wave()
    {
        if (waving)
        {
            yield break;
        }

        Debug.Log("Scanning Start");
        waving = true;

        float startT = Time.timeSinceLevelLoad;
        float curT = 0;
        SetWaveOrigin(transform.position);        
        while (curT < duration)
        {
            curT = Time.timeSinceLevelLoad - startT;
            SetWaveDuration(curT * speed);
            yield return new WaitForSeconds(0.01f);
        }

        SetWaveDuration(-1);
        waving = false;
        Debug.Log("Scanning End");
    }

    void OnMouseEnter()
    {
        over = true;
    }

    void OnMouseExit()
    {
        over = false;
    }
}
