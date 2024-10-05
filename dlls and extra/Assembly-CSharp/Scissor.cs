using UnityEngine;

public class Scissor : MonoBehaviour
{
	public Rect scissorRect = new Rect(0f, 0f, 1f, 1f);

	public void SetRectangle(Rect rect)
	{
		scissorRect = rect;
	}

	private void SetScissorRect(Camera cam, Rect r)
	{
		if (r.x < 0f)
		{
			r.width += r.x;
			r.x = 0f;
		}
		if (r.y < 0f)
		{
			r.height += r.y;
			r.y = 0f;
		}
		r.width = Mathf.Min(1f - r.x, r.width);
		r.height = Mathf.Min(1f - r.y, r.height);
		cam.rect = new Rect(0f, 0f, 1f, 1f);
		cam.ResetProjectionMatrix();
		Matrix4x4 projectionMatrix = cam.projectionMatrix;
		cam.rect = r;
		Matrix4x4 matrix4x = Matrix4x4.TRS(new Vector3(1f / r.width - 1f, 1f / r.height - 1f, 0f), Quaternion.identity, new Vector3(1f / r.width, 1f / r.height, 1f));
		Matrix4x4 matrix4x2 = Matrix4x4.TRS(new Vector3((0f - r.x) * 2f / r.width, (0f - r.y) * 2f / r.height, 0f), Quaternion.identity, Vector3.one);
		cam.projectionMatrix = matrix4x2 * matrix4x * projectionMatrix;
	}

	private void OnPreRender()
	{
		SetScissorRect(GetComponent<Camera>(), scissorRect);
	}
}
