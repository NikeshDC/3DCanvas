using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.IO;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(MeshCollider))]
public class Drawable : MonoBehaviour
{
    /*naming convention:- 'brush' is what is used to draw and 'canvas' is where it is drawn
     * ignore comments strting with //**
     */
    private Texture2D originalTexture;

    public enum BrushType { Square, Circle }; //possible types of brushes to use while drawing

    public BrushType brushType = BrushType.Square; //the current selected brushtype to use while drawing
    //public int brushWidth;
    public Color brushColor = Color.black;
    [HideInInspector] public Vector2Int brushPosition;
    private int brushSize; //size of pixels around the center i.e. actual width of brush is 2*brushSize+1
    private int MIN_BRUSH_SIZE = 1;
    private int MAX_BRUSH_SIZE = 20;
    private List<Vector2Int> brushCoords; //the coordinates of pixels that the brush represents(a template of the brush)
                                   //i.e. the pixels at this coorinates(around the brushcenter) are set when drawing
                                   //it depends on brush Type and size

    Renderer canvasRenderer; //the renderer component of the gameobject which draws the texture
    Texture2D canvasTexture; //the texture where the actaul pixels are drawn
    [SerializeField] private int textureSizeX = 1024;
    [SerializeField] private int textureSizeY = 1024;

    public static string Tag = "DrawableCanvas";  //tag the gameobject that uses Drawable script
    private bool canvasUpdateRequired = false; //set to true whenever something is drawn and canvas is required to be re-rendered
    [SerializeField]
    private float canvasUpdatePeriod = 0.1f; //the interval when to re-render

    void Start()
    {
        canvasRenderer = GetComponent<Renderer>();

        if (canvasRenderer.material.mainTexture == null)
        {//if there is no texture in the material create one else use the pre-existing material
            canvasTexture = new Texture2D(textureSizeX, textureSizeY);
        }
        else
        {
            if (canvasRenderer.material.mainTexture is Texture2D)
            {
                originalTexture = (Texture2D)canvasRenderer.material.mainTexture;
                textureSizeX = originalTexture.width;
                textureSizeY = originalTexture.height;
                canvasTexture = new Texture2D(originalTexture.width, originalTexture.height);
                canvasTexture.SetPixels32(originalTexture.GetPixels32());
                canvasTexture.Apply();
                
            }
            else
                Debug.LogError("Provided texture is not of type Texture2D");
        }
        canvasRenderer.material.mainTexture = canvasTexture;

        brushCoords = new List<Vector2Int>();
        SetBrushSize(0.0f);
        //brushSize = (brushWidth - 1) / 2;

        gameObject.tag = Tag;
        StartCoroutine(UpdateCanvas());  //start checking for update to the canvas
    }

    public int GetBrushSizePixel()
    {
        return brushSize;
    }

    public void SetBrushSize(float scale)
    {//sets brush size within the minimum and maximum range specified by 'scale'
        brushSize = (int) Mathf.Lerp(MIN_BRUSH_SIZE ,MAX_BRUSH_SIZE, scale);
        SetBrush(); //call this method less frequently or call SetBrush seperately if performance is an issue
    }
    private void SetVerticalLineBrush(int starty, int endy, int x)
    {//helper method for SetBrush
        for (int y = starty; y <= endy; y++)
            brushCoords.Add(new Vector2Int(x,y));
    }
    private void SetSquareBrush()
    {
        for (int x = -brushSize; x <= brushSize; x++)
            SetVerticalLineBrush(-brushSize, brushSize, x);
        //**for (int y = -brushSize; y <= brushSize; y++)
        //**{
        //**    brushCoords.Add(new Vector2Int(x, y));
        //**}
    }
    private void SetCircleBrush()
    {
        //midpoint circle drawing algorithm
        //**Debug.Log("Circle brush");
        int x = 0;
        int y = brushSize;
        int p = 1 - brushSize;
        SetVerticalLineBrush(-y, y, x);
        while (x < y)
        {
            x++;
            if (p < 0)
            { p += 2 * x + 1; }
            else
            {
                y--;
                p += 2 * x + 1 - 2 * y;
            }
            SetVerticalLineBrush(-y, y, x);
            SetVerticalLineBrush(-y, y, -x);
            SetVerticalLineBrush(-x, x, y);
            SetVerticalLineBrush(-x, x, -y);
            //**Debug.Log("Added to Circle brush");
        }
        //**Debug.Log("Circle brush Count:" + brushCoords.Count);
    }

    public void SetBrush(BrushType _brushType)
    {
        brushType = _brushType;
        SetBrush();
    }

    public void SetBrush()
    {//sets the brushCoords where pixels are to be drawn(considering center as (0,0))
        brushCoords.Clear(); //remove pevious brush template

        switch(brushType)
        {
            case BrushType.Square:
                SetSquareBrush();
                break;
            case BrushType.Circle:
                SetCircleBrush();
                break;
        }
    }


    public void SetPixels()
    {//sets the pixels around brushPos(x,y) given by adding brushPos(x,y) to each brushCoords to the color of brushColor
        int pixx, pixy;
        foreach (Vector2Int brushCoord in brushCoords)
        {
            pixx = brushCoord.x + brushPosition.x;
            pixy = brushCoord.y + brushPosition.y;
            if (pixx >= 0 && pixy >= 0 && pixx < textureSizeX && pixy < textureSizeY)
                canvasTexture.SetPixel(pixx, pixy, brushColor);
        }

        canvasUpdateRequired = true;
    }

    public void SetPixels(int x, int y)
    {//sets the pixels around (x,y) given by adding (x,y) to each brushCoords to the color of brushColor
        //**Debug.Log("Center: (" + x + ", " + y + ")");
        int pixx, pixy;
        foreach (Vector2Int brushCoord in brushCoords)
        {
            pixx = brushCoord.x + x;
            pixy = brushCoord.y + y;
            if (pixx >= 0 && pixy >= 0 && pixx < textureSizeX && pixy < textureSizeY)
            {
                canvasTexture.SetPixel(pixx, pixy, brushColor);
                //**Debug.Log("Pixel set at ("+pixx+", "+pixy+")");
            }
        }

        canvasUpdateRequired = true;
    }

    public void erasePixels(int x, int y)
    {//uses the same brush as painting brush to set the texture to that of original
        if (originalTexture == null)
            return;
        int pixx, pixy;
        foreach (Vector2Int brushCoord in brushCoords)
        {
            pixx = brushCoord.x + x;
            pixy = brushCoord.y + y;
            if (pixx >= 0 && pixy >= 0 && pixx < textureSizeX && pixy < textureSizeY)
            {
                canvasTexture.SetPixel(pixx, pixy, originalTexture.GetPixel(pixx, pixy));
                //**Debug.Log("Pixel set at ("+pixx+", "+pixy+")");
            }
        }
        canvasUpdateRequired = true;
    }

    public void erasePixels()
    {//uses the same brush as painting brush to set the texture to that of original
        if (originalTexture == null)
            return;
        int pixx, pixy;
        foreach (Vector2Int brushCoord in brushCoords)
        {
            pixx = brushCoord.x + brushPosition.x;
            pixy = brushCoord.y + brushPosition.y;
            if (pixx >= 0 && pixy >= 0 && pixx < textureSizeX && pixy < textureSizeY)
            {
                canvasTexture.SetPixel(pixx, pixy, originalTexture.GetPixel(pixx, pixy));
                //**Debug.Log("Pixel set at ("+pixx+", "+pixy+")");
            }
        }
        canvasUpdateRequired = true;
    }

    private IEnumerator UpdateCanvas()
    { //check after fixed interval if update is required (for performance reason)
      //i.e. call to Texture2D.Apply() should be infrequent 
        while (true)
        {
            if (canvasUpdateRequired)
            {
                canvasTexture.Apply();
                canvasUpdateRequired = false;
            }
            yield return new WaitForSeconds(canvasUpdatePeriod);
        }
    }

    public int GetTextureSizeX()
    { return textureSizeX; }

    public int GetTextureSizeY()
    { return textureSizeY; }

    public void SaveImage(string fileName = "SavedImage.png")
    {
        byte[] imageBytes = canvasTexture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath +"/"+ fileName, imageBytes);
    }
}
