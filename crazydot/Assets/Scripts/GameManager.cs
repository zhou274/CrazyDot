using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public UIManager uIManager;

	public ScoreManager scoreManager;

	[Space(5f)]
	public GameObject player;

	[Header("Game settings")]
	[Space(5f)]
	public Material trailMaterial;

	[Space(5f)]
	public Color[] colorTable;

	[Space(5f)]
	public GameObject obstaclePrefab;

	[Space(5f)]
	public float minTimeBetweenObstacles = 0.5f;

	public float startTimeBetweenObstacles = 1f;

	private float currentTimeBetweenObstacles;

	private bool spawning;

	private GameObject tempObstacle;

	private Vector2 tempPos;

	private Vector3 screenSize;

	private Color color;

	public static GameManager Instance
	{
		get;
		set;
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void Start()
	{
		Application.targetFrameRate = 60;
		screenSize = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0f));
		Color color = colorTable[Random.Range(0, colorTable.Length)];
		player.GetComponent<Player>().SetColor(color);
		trailMaterial.color = color;
	}

	private void Update()
	{
		if (uIManager.gameState == GameState.PLAYING && Input.GetMouseButton(0) && !uIManager.IsButton() && !spawning)
		{
			spawning = true;
			ScoreManager.Instance.StartCounting();
			InitVariables();
			StartCoroutine(SpawnObstacle());
		}
	}

	private void InitVariables()
	{
		currentTimeBetweenObstacles = startTimeBetweenObstacles;
	}

	private IEnumerator SpawnObstacle()
	{
		if (ScoreManager.Instance.currentScore > 50f)
		{
			currentTimeBetweenObstacles = minTimeBetweenObstacles;
		}
		else if (ScoreManager.Instance.currentScore > 35f)
		{
			currentTimeBetweenObstacles = startTimeBetweenObstacles - 0.25f;
		}
		else if (ScoreManager.Instance.currentScore > 15f)
		{
			currentTimeBetweenObstacles = startTimeBetweenObstacles - 0.15f;
		}
		tempObstacle = UnityEngine.Object.Instantiate(obstaclePrefab);
		tempPos = new Vector2(UnityEngine.Random.Range(0f - screenSize.x + obstaclePrefab.GetComponent<SpriteRenderer>().bounds.size.x, screenSize.x - obstaclePrefab.GetComponent<SpriteRenderer>().bounds.size.x), screenSize.y + obstaclePrefab.GetComponent<SpriteRenderer>().bounds.size.y);
		color = colorTable[Random.Range(0, colorTable.Length)];
		tempObstacle.GetComponent<Obstacle>().InitObstacle(tempPos, color);
		yield return new WaitForSecondsRealtime(currentTimeBetweenObstacles);
		StartCoroutine(SpawnObstacle());
	}

	public void RestartGame()
	{
		if (uIManager.gameState == GameState.PAUSED)
		{
			Time.timeScale = 1f;
		}
		uIManager.ShowGameplay();
		ClearScene();
		scoreManager.ResetCurrentScore();
	}
	
	public void ClearScene()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Obstacle");
		for (int i = 0; i < array.Length; i++)
		{
			UnityEngine.Object.Destroy(array[i]);
		}
		player.GetComponent<SpriteRenderer>().enabled = true;
		player.transform.position = new Vector2(0f, -2.5f);
		color = colorTable[Random.Range(0, colorTable.Length)];
		player.GetComponent<Player>().SetColor(color);
		trailMaterial.color = color;
		player.GetComponent<TrailRenderer>().enabled = true;
	}
	public void ContinueGame()
	{
        if(uIManager.gameState == GameState.GAMEOVER)
		{
            player.GetComponent<TrailRenderer>().enabled = true;
            player.GetComponent<SpriteRenderer>().enabled = true;
            uIManager.ContinueGameOver();
		}
    }
	public void GameOver()
	{
		if (uIManager.gameState == GameState.PLAYING)
		{
			player.GetComponent<TrailRenderer>().enabled = false;
			player.GetComponent<SpriteRenderer>().enabled = false;
			StopAllCoroutines();
			spawning = false;
			ScoreManager.Instance.StopCounting();
			AudioManager.Instance.PlayEffects(AudioManager.Instance.gameOver);
			uIManager.ShowGameOver();
			scoreManager.UpdateScoreGameover();
		}
	}
}
