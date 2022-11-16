﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Drawer))]
public class CanvasManager : MonoBehaviour
{
    public BrushPaletteUI brushPalette;
    public Drawable drawingCanvas;
    public Drawer drawingAgent;

    void Start()
    {
        drawingAgent = GetComponent<Drawer>();
    }
    public void SetBrushRed(float _r)
    { 
        brushPalette.SetRed(_r);
        drawingCanvas.brushColor = brushPalette.GetColor();
    }

    public void SetBrushGreen(float _g)
    {
        brushPalette.SetGreen(_g);
        drawingCanvas.brushColor = brushPalette.GetColor();
    }

    public void SetBrushBlue(float _b)
    {
        brushPalette.SetBlue(_b);
        drawingCanvas.brushColor = brushPalette.GetColor();
    }

    public void SetBrushAlpha(float _a)
    {
        brushPalette.SetAlpha(_a);
        drawingCanvas.brushColor = brushPalette.GetColor();
    }

    public void SetBrushSize(float _size)
    {
        brushPalette.SetSize(_size);
        drawingCanvas.SetBrushSize(_size);
        drawingAgent.SetInterpolationPixelCount(drawingCanvas.GetBrushSizePixel());
    }

    public void SetBrushType(int index)
    {
        Drawable.BrushType selectedBrushtype = brushPalette.SelectBrushType(index);
        drawingCanvas.SetBrush(selectedBrushtype);
        //Debug.Log("In CM");
    }

    public void SaveCurrentCanvas()
    {
        drawingCanvas.SaveImage();
    }
}