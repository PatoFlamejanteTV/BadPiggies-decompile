using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "INGameData", menuName = "ScriptableObjects/INGameData", order = 1)]
public class INGameData : ScriptableObject
{
	public List<Font> Fonts;

	public List<GameObject> Prefabs;

	public List<Material> Materials;

	public List<Shader> Shaders;

	public List<TextAsset> TextAssets;
}
