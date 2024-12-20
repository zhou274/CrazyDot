using UnityEngine;

public class Obstacle : MonoBehaviour
{
	public Color color;

	public bool hit;

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!hit && (collision.gameObject.CompareTag("Obstacle") || collision.gameObject.CompareTag("Line")))
		{
			hit = true;
		}
	}

	public void InitObstacle(Vector2 _position, Color _color)
	{
		color = _color;
		GetComponent<SpriteRenderer>().color = _color;
		base.transform.position = _position;
	}

	private void Update()
	{
	}
}
