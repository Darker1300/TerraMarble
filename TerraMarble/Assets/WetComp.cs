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
	

	//private Color StartColor;
    private void Start()
    {
		shape =  GetComponent<ShapeRenderer>();
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

		shape.Color = SetAlpha(shape.Color,percent/100);
        //Debug.Log("wetAdded" + percent / 100);
        //shape.Color = Color.blue;
        
	}

	
	

	private Color SetAlpha(Color _color, float _a)
	{
		_color.a = _a;
		return _color;
	}
	
}
