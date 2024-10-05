using UnityEngine;

public class GizmoUtils : MonoBehaviour
{
	public static void DrawArrow(Vector3 pos, Vector3 direction)
	{
		if (direction.magnitude != 0f)
		{
			float num = 0.35f;
			Vector3 vector = Quaternion.AngleAxis(30f + 180f, Vector3.forward) * direction;
			Vector3 vector2 = Quaternion.AngleAxis(0f - 30f - 180f, Vector3.forward) * direction;
			Gizmos.DrawRay(pos, direction);
			Gizmos.DrawRay(pos + direction, vector * num);
			Gizmos.DrawRay(pos + direction, vector2 * num);
		}
	}
}
