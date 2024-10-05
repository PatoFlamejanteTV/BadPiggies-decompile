using UnityEngine;

namespace MentalTools;

public class BezierMesh : MonoBehaviour
{
	public Material mainMaterial;

	public Material secondMaterial;

	public float borderWidth;

	private BezierCurve bezierCurve;

	[SerializeField]
	private GameObject polygon;

	[SerializeField]
	private GameObject borderPolygon;

	public void CreateMesh()
	{
		this.bezierCurve = GetComponent<BezierCurve>();
		if (this.bezierCurve == null)
		{
			return;
		}
		if (polygon != null)
		{
			DeleteMesh();
		}
		polygon = MeshTool.CreateMeshFromBezier(this.bezierCurve, base.transform, MentalMath.AxisSpace.XY);
		if (mainMaterial != null)
		{
			polygon.GetComponent<MeshRenderer>().sharedMaterial = mainMaterial;
		}
		if (secondMaterial != null)
		{
			GameObject gameObject = new GameObject("Border");
			gameObject.transform.SetParent(polygon.transform);
			gameObject.transform.localPosition = Vector3.forward * 0.25f;
			gameObject.transform.localScale = Vector3.one;
			BezierCurve bezierCurve = gameObject.AddComponent<BezierCurve>();
			bezierCurve.bezierPointCount = this.bezierCurve.bezierPointCount;
			bezierCurve.loop = this.bezierCurve.loop;
			bezierCurve.Curve = new Bezier();
			for (int i = 0; i < this.bezierCurve.Curve.Count; i++)
			{
				BezierNode bezierNode = bezierCurve.Curve.AddNode(this.bezierCurve.Curve[i].Position, this.bezierCurve.Curve[i].ForwardTangent, this.bezierCurve.Curve[i].BackwardTangent);
				bezierNode.ForwardTangentType = this.bezierCurve.Curve[i].ForwardTangentType;
				bezierNode.BackwardTangentType = this.bezierCurve.Curve[i].BackwardTangentType;
			}
			borderPolygon = MeshTool.CreateMeshStripFromBezier(bezierCurve, gameObject.transform, MentalMath.AxisSpace.XY, borderWidth);
			borderPolygon.GetComponent<MeshRenderer>().sharedMaterial = secondMaterial;
		}
	}

	public void DeleteMesh()
	{
		if (polygon != null)
		{
			Object.DestroyImmediate(polygon);
		}
	}
}
