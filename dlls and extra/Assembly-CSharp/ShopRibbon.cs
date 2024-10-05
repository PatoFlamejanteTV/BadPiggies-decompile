using System;
using UnityEngine;

[Serializable]
public class ShopRibbon
{
	public enum Ribbon
	{
		BestValue,
		MostPopular
	}

	public Ribbon ribbonType;

	public GameObject ribbon;

	public RuntimePlatform platform;

	public string itemId;
}
