using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VigorFieldOfView : MonoBehaviour {

    Image img;
    Sprite coverSprite;
    Texture2D coverTex;

    [SerializeField]
    int blockSize = 1;

    [SerializeField, Range(0, 1)]
    float vigorEffect = 1;

    public float vigor
    {
        set
        {
            vigorEffect = Mathf.Clamp01(value);
        }
    }

    [SerializeField]
    int maxBlockStillFreeHalf = 4;

    void Start () {
        img = GetComponent<Image>();
        SetupSprite();
	}

    void SetupSprite()
    {
        coverTex = new Texture2D(Screen.width, Screen.height);
        coverTex.filterMode = FilterMode.Point;
        coverSprite = Sprite.Create(coverTex, new Rect(0, 0, coverTex.width, coverTex.height), Vector2.one * 0.5f);
        coverSprite.name = "Dynamic Cover Sprite";
        img.sprite = coverSprite;
        img.preserveAspect = true;              
    }

    bool ScreenMatchesSprite
    {
        get
        {
            return Screen.width == coverTex.width && Screen.height == coverTex.height;
        }
    }

    void Update () {
		if (!ScreenMatchesSprite)
        {
            SetupSprite();
        }
        SetCover();
    }

    static Color onColor = new Color(1, 1, 1, 1);
    static Color offColor = new Color(1, 1, 1, 0);

    void SetCover()
    {
        int w = coverTex.width;
        int h = coverTex.height;        

        int halfWidth = w / 2;
        int halfHeight = h / 2;

        float xOptimum = (halfWidth - maxBlockStillFreeHalf) * (1 - vigorEffect);
        float yOptimum = (halfHeight - maxBlockStillFreeHalf) * (1 - vigorEffect);

        int leftOn = Mathf.FloorToInt(xOptimum);
        int rightOn = w - leftOn;
        int bottomOn = Mathf.FloorToInt(yOptimum);
        int topOn = h - bottomOn;

        int leftOff = Mathf.CeilToInt(xOptimum);
        int rightOff = w - leftOff;
        if (leftOff == leftOn)
        {
            leftOn--;
        }
        if (rightOff == rightOn)
        {
            rightOff--;
        }

        int bottomOff = Mathf.CeilToInt(yOptimum);
        int topOff = h - bottomOff;
        if (bottomOff == bottomOn)
        {
            bottomOn--;
        }
        if (topOff == topOn)
        {
            topOff--;
        }

        int stepSizeMiddleOff = Mathf.CeilToInt(((xOptimum - Mathf.Floor(xOptimum)) + (yOptimum - Mathf.Floor(yOptimum))) / .2f) + 3;
        int steps = 0;

        for (int x=0; x< w; x++)
        {
            for (int y=0; y< h; y++)
            {
                if (x <= leftOn || x >= rightOn || y >= topOn || y <= bottomOn)
                {
                    coverTex.SetPixel(x, y, onColor);
                } else if (x > leftOff && x < rightOff && y > bottomOff && y < topOff)
                {
                    coverTex.SetPixel(x, y, offColor);
                } else
                {
                    steps++;
                    coverTex.SetPixel(x, y, steps % stepSizeMiddleOff == 0 ? offColor : onColor);
                }
            }
        }
        coverTex.Apply();
    }
}
