using UnityEngine;

public class SideLine : MonoBehaviour
{
	public BorderLine borderLine;

	private void Start()
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
		if (borderLine == BorderLine.LEFT)
		{
			base.transform.position = new Vector3(0f - vector.x - 0.1f, 0f, 0f);
		}
		else if (borderLine == BorderLine.RIGHT)
		{
			base.transform.position = new Vector3(vector.x + 0.1f, 0f, 0f);
		}
		else if (borderLine == BorderLine.BOTTOM)
		{
			base.transform.position = new Vector3(0f, 0f - vector.y, 0f);
		}
	}
}
