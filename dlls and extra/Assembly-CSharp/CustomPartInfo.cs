using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CustomPartInfo
{
	[SerializeField]
	private BasePart.PartType partType;

	[SerializeField]
	private List<BasePart> customParts;

	public BasePart.PartType PartType => partType;

	public List<BasePart> PartList => customParts;
}
