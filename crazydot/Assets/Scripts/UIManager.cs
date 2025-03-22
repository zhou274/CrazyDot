using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using StarkSDKSpace;
using TTSDK.UNBridgeLib.LitJson;
using TTSDK;

public class UIManager : MonoBehaviour
{
	[Header("GUI Components")]
	public GameObject mainMenuGui;

	public GameObject pauseGui;

	public GameObject gameplayGui;

	public GameObject gameOverGui;

	public GameState gameState;

	private bool clicked;

    private StarkAdManager starkAdManager;

    public string clickid;

    private void Start()
	{
		mainMenuGui.SetActive(value: true);
		pauseGui.SetActive(value: false);
		gameplayGui.SetActive(value: false);
		gameOverGui.SetActive(value: false);
		gameState = GameState.MENU;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && gameState == GameState.MENU && !clicked)
		{
			if (!IsButton())
			{
				AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
				ShowGameplay();
			}
		}
		else if (Input.GetMouseButtonUp(0) && clicked && gameState == GameState.MENU)
		{
			clicked = false;
		}
	}

	public void ShowMainMenu()
	{
		ScoreManager.Instance.StopCounting();
		ScoreManager.Instance.ResetCurrentScore();
		clicked = true;
		mainMenuGui.SetActive(value: true);
		pauseGui.SetActive(value: false);
		gameplayGui.SetActive(value: false);
		gameOverGui.SetActive(value: false);
		if (gameState == GameState.PAUSED)
		{
			Time.timeScale = 1f;
		}
		gameState = GameState.MENU;
		AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
		GameManager.Instance.ClearScene();
	}

	public void ShowPauseMenu()
	{
		if (gameState != GameState.PAUSED)
		{
			pauseGui.SetActive(value: true);
			Time.timeScale = 0f;
			gameState = GameState.PAUSED;
			AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
		}
	}

	public void HidePauseMenu()
	{
		pauseGui.SetActive(value: false);
		Time.timeScale = 1f;
		gameState = GameState.PLAYING;
		AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
	}

	public void ShowGameplay()
	{
        
        mainMenuGui.SetActive(value: false);
		pauseGui.SetActive(value: false);
		gameplayGui.SetActive(value: true);
		gameOverGui.SetActive(value: false);
		gameState = GameState.PLAYING;
		AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
		AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);
	}

	public void ShowGameOver()
	{
        ShowInterstitialAd("9jjei2g8e3r655465d",
            () => {
                Debug.LogError("--插屏广告完成--");

            },
            (it, str) => {
                Debug.LogError("Error->" + str);
            });
        mainMenuGui.SetActive(value: false);
		pauseGui.SetActive(value: false);
		gameplayGui.SetActive(value: false);
		gameOverGui.SetActive(value: true);
		gameState = GameState.GAMEOVER;
		AudioManager.Instance.PlayMusic(AudioManager.Instance.menuMusic);
	}
	//todo
	public void ContinueGameOver()
	{
        mainMenuGui.SetActive(value: false);
        pauseGui.SetActive(value: false);
        gameplayGui.SetActive(value: true);
        gameOverGui.SetActive(value: false);
        gameState = GameState.PLAYING;
        AudioManager.Instance.PlayEffects(AudioManager.Instance.buttonClick);
        AudioManager.Instance.PlayMusic(AudioManager.Instance.gameMusic);
    }

	public bool IsButton()
	{
		bool flag = false;
		PointerEventData eventData = new PointerEventData(EventSystem.current)
		{
			position = UnityEngine.Input.mousePosition
		};
		List<RaycastResult> list = new List<RaycastResult>();
		EventSystem.current.RaycastAll(eventData, list);
		foreach (RaycastResult item in list)
		{
			flag |= (item.gameObject.GetComponent<Button>() != null);
		}
		return flag;
	}

    /// <summary>
    /// 播放插屏广告
    /// </summary>
    /// <param name="adId"></param>
    /// <param name="errorCallBack"></param>
    /// <param name="closeCallBack"></param>
    public void ShowInterstitialAd(string adId, System.Action closeCallBack, System.Action<int, string> errorCallBack)
    {
        starkAdManager = StarkSDK.API.GetStarkAdManager();
        if (starkAdManager != null)
        {
            var mInterstitialAd = starkAdManager.CreateInterstitialAd(adId, errorCallBack, closeCallBack);
            mInterstitialAd.Load();
            mInterstitialAd.Show();
        }
    }
}
