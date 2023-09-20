using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class WetComp : MonoBehaviour
{
	bool isRegion = false;
	[SerializeField] private Region parentRegion;
	[SerializeField] private ShapeRenderer shape;
    [SerializeField] private SpriteRenderer sprite;

    private bool usingShape;
	//private Color StartColor;
    private void Start()
    {
        if (GetComponent<ShapeRenderer>() !=null)
        {
		    
            shape =  GetComponent<ShapeRenderer>();
            usingShape = true;

        }
        else
        {
            sprite = GetComponent<SpriteRenderer>();
        }
		//MakeTransparent();
	}

    void OnEnable()
	{
        parentRegion = GetComponentInParent<Region>();

        MakeTransparent();
        //parentRegion.comp.AddListener(UpdateWet);
        parentRegion.GetComponent<RegionWetController>().WetUpdate += UpdateWet;


    }
	public void SubscribeToRegionWetComp()
	{
		parentRegion.GetComponent<RegionWetController>().WetUpdate += UpdateWet;
	}
	public void UnsubscribeToRegionWetComp()
	{
		parentRegion.GetComponent<RegionWetController>().WetUpdate -= UpdateWet;
	}
	
	public void MakeTransparent()
	{
        //shape.Color = Color.blue;
	}
	void OnDisable()
	{
		parentRegion.GetComponent<RegionWetController>().WetUpdate += UpdateWet;
	}

	public void UpdateWet(float percent)
	{
		// apply color change to renderer
        if (usingShape)
        {
            shape.Color = SetAlpha(shape.Color, percent / 100);

        }
        else
            sprite.color = SetAlpha(sprite.color, percent / 100);
        //Debug.Log("wetAdded" + percent / 100);
        //shape.Color = Color.blue;

    }

	
	

	private Color SetAlpha(Color _color, float _a)
	{
		_color.a = _a;
		return _color;
	}
	
}
