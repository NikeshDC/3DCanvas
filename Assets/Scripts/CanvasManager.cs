using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Drawer))]
public class CanvasManager : MonoBehaviour
{
    public GameObject UIcontainer;
    public BrushPaletteUI brushPalette;
    //public Drawable drawingCanvas;
    Drawer drawingAgent;

    void Start()
    {
        drawingAgent = GetComponent<Drawer>();
    }
    public void SetBrushRed(float _r)
    {
        if (drawingAgent.drawingCanvas == null)
            return;
        brushPalette.SetRed(_r);
        drawingAgent.drawingCanvas.brushColor = brushPalette.GetColor();
    }

    public void SetBrushGreen(float _g)
    {
        if (drawingAgent.drawingCanvas == null)
            return;
        brushPalette.SetGreen(_g);
        drawingAgent.drawingCanvas.brushColor = brushPalette.GetColor();
    }

    public void SetBrushBlue(float _b)
    {
        if (drawingAgent.drawingCanvas == null)
            return;
        brushPalette.SetBlue(_b);
        drawingAgent.drawingCanvas.brushColor = brushPalette.GetColor();
    }

    public void SetBrushAlpha(float _a)
    {
        if (drawingAgent.drawingCanvas == null)
            return;
        brushPalette.SetAlpha(_a);
        drawingAgent.drawingCanvas.brushColor = brushPalette.GetColor();
    }

    public void SetBrushSize(float _size)
    {
        if (drawingAgent.drawingCanvas == null)
            return;
        brushPalette.SetSize(_size);
        drawingAgent.drawingCanvas.SetBrushSize(_size);
        drawingAgent.SetInterpolationPixelCount(drawingAgent.drawingCanvas.GetBrushSizePixel());
    }

    public void SetBrushType(int index)
    {
        if (drawingAgent.drawingCanvas == null)
            return;
        Drawable.BrushType selectedBrushtype = brushPalette.SelectBrushType(index);
        drawingAgent.drawingCanvas.SetBrush(selectedBrushtype);
    }

    public void SaveCurrentCanvas()
    {
        //drawingCanvas.SaveImage();
        StartCoroutine(TakeScreenshot());
    }

    private IEnumerator TakeScreenshot()
    {
        UIcontainer.SetActive(false);      //take screenshot without UI elements
        yield return new WaitForEndOfFrame();
        ScreenCapture.CaptureScreenshot("savedScreen.png");
        UIcontainer.SetActive(true);
    }
}
