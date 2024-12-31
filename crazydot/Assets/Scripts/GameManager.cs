using System.Collections;
using UnityEngine;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;
using StarkSDKSpace;
using System.Collections.Generic;
using UnityEngine.Analytics;

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

    public string clickid;
    private StarkAdManager starkAdManager;
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
        ShowVideoAd("1onjn930jo316if83d",
            (bol) => {
                if (bol)
                {
                    if (uIManager.gameState == GameState.GAMEOVER)
                    {
                        player.GetComponent<TrailRenderer>().enabled = true;
                        player.GetComponent<SpriteRenderer>().enabled = true;
                        uIManager.ContinueGameOver();
                    }



                    clickid = "";
                    getClickid();
                    apiSend("game_addiction", clickid);
                    apiSend("lt_roi", clickid);


                }
                else
                {
                    StarkSDKSpace.AndroidUIManager.ShowToast("观看完整视频才能获取奖励哦！");
                }
            },
            (it, str) => {
                Debug.LogError("Error->" + str);
                //AndroidUIManager.ShowToast("广告加载异常，请重新看广告！");
            });
        
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


    public void getClickid()
    {
        var launchOpt = StarkSDK.API.GetLaunchOptionsSync();
        if (launchOpt.Query != null)
        {
            foreach (KeyValuePair<string, string> kv in launchOpt.Query)
                if (kv.Value != null)
                {
                    Debug.Log(kv.Key + "<-参数-> " + kv.Value);
                    if (kv.Key.ToString() == "clickid")
                    {
                        clickid = kv.Value.ToString();
                    }
                }
                else
                {
                    Debug.Log(kv.Key + "<-参数-> " + "null ");
                }
        }
    }

    public void apiSend(string eventname, string clickid)
    {
        TTRequest.InnerOptions options = new TTRequest.InnerOptions();
        options.Header["content-type"] = "application/json";
        options.Method = "POST";

        JsonData data1 = new JsonData();

        data1["event_type"] = eventname;
        data1["context"] = new JsonData();
        data1["context"]["ad"] = new JsonData();
        data1["context"]["ad"]["callback"] = clickid;

        Debug.Log("<-data1-> " + data1.ToJson());

        options.Data = data1.ToJson();

        TT.Request("https://analytics.oceanengine.com/api/v2/conversion", options,
           response => { Debug.Log(response); },
           response => { Debug.Log(response); });
    }


    /// <summary>
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="closeCallBack"></param>
    /// <param name="errorCallBack"></param>
    public void ShowVideoAd(string adId, System.Action<bool> closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            starkAdManager.ShowVideoAdWithId(adId, closeCallBack, errorCallBack);
        }
    }


    
}
