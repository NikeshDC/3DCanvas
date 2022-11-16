using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class BrushPaletteUI : MonoBehaviour
{
    //[SerializeField]
    //private RectTransform brushSizeParent;
    [SerializeField]
    private RectTransform brushSample;
    private Image brushImage;
    [SerializeField]
    private float maxBrushImageSize = 1.3f;  //max size is 30% more than min size(default size)

    [SerializeField]
    private Dropdown brushTypeSelector;
    String[] brushTypeNames;
    Drawable.BrushType[] brushTypes;
    Drawable.BrushType selectedBrush;

    void Start()
    {
        brushImage = brushSample.GetComponent<Image>();
        brushImage.color = Color.black;
        SetBrushTypes();
    }

    void SetBrushTypes()
    {
        brushTypes = (Drawable.BrushType[])Enum.GetValues(typeof(Drawable.BrushType));
        brushTypeNames = Enum.GetNames(typeof(Drawable.BrushType));
        brushTypeSelector.ClearOptions();
        brushTypeSelector.AddOptions(new List<String>(brushTypeNames));
    }

    public Drawable.BrushType SelectBrushType(int index)
    {//brushtype selected from dropdown
        selectedBrush = brushTypes[index];
        //Debug.Log("In UI" + index);
        return selectedBrush;
    }

    public Drawable.BrushType GetSelectedBrushType()
    {
        return selectedBrush;
    }

    public Color GetColor()
    { return brushImage.color; }

    public void SetRed(float _r) 
    { brushImage.color = new Color(_r, brushImage.color.g, brushImage.color.b, brushImage.color.a); }

    public void SetGreen(float _g)
    { brushImage.color = new Color(brushImage.color.r, _g, brushImage.color.b, brushImage.color.a); }

    public void SetBlue(float _b)
    { brushImage.color = new Color(brushImage.color.r, brushImage.color.g, _b, brushImage.color.a); }

    public void SetAlpha(float _a)
    { brushImage.color = new Color(brushImage.color.r, brushImage.color.g, brushImage.color.b, _a); }

    public void SetSize(float _size)
    { //_size ranges from 0 to 1f which is translated to a value between 1f and maxBrushImageSize
        _size = Mathf.Lerp(1f, maxBrushImageSize, _size);
        brushSample.localScale = new Vector3(_size, _size, 1f); }
}
