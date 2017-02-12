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

    int sWidth;
    int sHeight;

    void SetupSprite()
    {
        sWidth = Screen.width;
        sHeight = Screen.height;

        coverTex = new Texture2D(Screen.width / blockSize, Screen.height / blockSize);
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
            return Screen.width == sWidth && Screen.height == sHeight;
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
        int rightOn = w - leftOn - 1;
        int bottomOn = Mathf.FloorToInt(yOptimum);
        int topOn = h - bottomOn - 1;        

        int leftOff = Mathf.CeilToInt(xOptimum);
        int rightOff = w - leftOff - 1;

        int bottomOff = Mathf.CeilToInt(yOptimum);
        int topOff = h - bottomOff - 1;

        //int stepSizeMiddleOff = Mathf.CeilToInt(((xOptimum - Mathf.Floor(xOptimum)) + (yOptimum - Mathf.Floor(yOptimum))) / .2f) + 3;
        int steps = 0;

        for (int x=0; x< w; x++)
        {
            bool xOff = x > leftOff && x < rightOff;
            int deltaX = xOff ? 0 : Mathf.Min(Mathf.Abs(x - leftOn), Mathf.Abs(x - rightOn));
            
            for (int y=0; y< h; y++)
            {
                bool yOff = y > bottomOff && y < topOff;
                int deltaY = yOff ? 0 : Mathf.Min(Mathf.Abs(y - bottomOn), Mathf.Abs(y - topOn));

                steps++;
                coverTex.SetPixel(x, y, 0.25f > (1f - vigorEffect) * (2 * deltaY + 2 * deltaX) ? offColor : onColor);
                
            }
        }
        coverTex.Apply();
    }
}
