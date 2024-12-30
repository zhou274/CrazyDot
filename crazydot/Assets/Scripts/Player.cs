using UnityEngine;

public class Player : MonoBehaviour
{
	public float followSpeed = 2f;

	public float offsetY = 0.8f;

	private Vector2 destination;

	private Color color;

	private bool follow;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (GameManager.Instance.uIManager.gameState != GameState.PLAYING || !collision.gameObject.CompareTag("Obstacle"))
		{
			return;
		}
		if (collision.gameObject.GetComponent<Obstacle>().color == color)
		{
			AudioManager.Instance.PlayEffects(AudioManager.Instance.sameColor);
			UnityEngine.Object.Destroy(collision.gameObject);
			GameObject[] array = GameObject.FindGameObjectsWithTag("Obstacle");
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetComponent<Obstacle>().hit)
				{
					UnityEngine.Object.Destroy(array[i]);
				}
			}
		}
		else
		{
			AudioManager.Instance.PlayEffects(AudioManager.Instance.wrongColor);
			GameManager.Instance.GameOver();
		}
	}

	public void SetColor(Color _color)
	{
		color = _color;
		GetComponent<SpriteRenderer>().color = color;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			follow = true;
		}
		if (GameManager.Instance.uIManager.gameState == GameState.PLAYING && !GameManager.Instance.uIManager.IsButton())
		{
			if (Input.GetMouseButton(0) && follow)
			{
				destination = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
				destination = new Vector2(destination.x, destination.y + offsetY);
				base.transform.position = Vector2.Lerp(base.transform.position, destination, followSpeed * Time.deltaTime);
			}
			if (Input.GetMouseButtonUp(0))
			{
				follow = false;
			}
		}
	}
}
