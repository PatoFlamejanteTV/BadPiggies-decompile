using UnityEngine;

namespace Spine.Unity;

public static class SpineMesh
{
	internal const HideFlags MeshHideflags = HideFlags.DontSave;

	public static Mesh NewMesh()
	{
		Mesh mesh = new Mesh();
		mesh.MarkDynamic();
		mesh.name = "Skeleton Mesh";
		mesh.hideFlags = HideFlags.DontSave;
		return mesh;
	}
}
