using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Ali.Helper;

public class UIManager : MonoSingleton<UIManager>
{
    private Bermuda.Runner.BermudaRunnerCharacter bermudaRunnerCharacter;

    [Header("Main UI's")]
    [SerializeField] private GameObject tapToPlayUI;
    [SerializeField] private GameObject nextLvMenuUI;
    [SerializeField] private GameObject restartMenuUI;

    [Header("Sound and Toggle")]
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle soundToggle;

    [Header("Slap Button")]
    [SerializeField] private TMP_Text slapBoardText;

    [Header("Status UI Texts")]
    [SerializeField] private TMP_Text currentLV;
    [SerializeField] private TMP_Text totalMoneyText;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private Image healthImage;

    public float multiplier;
    private int multiplierCounter = 0;

    //status texts
    [SerializeField] private GameObject playerHealtUI;
    [SerializeField] private GameObject bossHealtUI;
    [SerializeField] private Image progressBarImage;
    [SerializeField] private GameObject progressBar;
    [SerializeField] private Image playerHealthImage;
    [SerializeField] private Image bossHealthImage;

    [Space]

    //randommultiplier selection
    [SerializeField] private Transform arrow;
    private bool clickBonusCheck = false;
    [Space]

    private BossManager bossManager;
    public bool isPaused;

    private void Awake()
    {
        if (PlayerPrefs.GetInt("vibrationOnOff") == 0)
        {
            vibrationToggle.GetComponent<Toggle>().isOn = false;
        }

        if (PlayerPrefs.GetInt("soundOnOff") == 0)
        {
            soundToggle.GetComponent<Toggle>().isOn = false;
        }
    }

    private void Start()
    {
        isPaused = true;
        DOTween.Init();
        LevelText();
    }

    public void PlayResButton()
    {
        if (tapToPlayUI.activeSelf)
        {
            tapToPlayUI.SetActive(false);
            isPaused = false;
        }

        if (nextLvMenuUI.activeSelf)
        {
            nextLvMenuUI.SetActive(false);
            isPaused = false;
            ResSlapButton();

            HCLevelManager.Instance.LevelUp();
            LevelText();
            HCLevelManager.Instance.GenerateCurrentLevel(); //SAHNEYI YUKLE BASTAN
        }

        if (restartMenuUI.activeSelf)
        {
            restartMenuUI.SetActive(false);
            isPaused = false;
            Debug.Log("YUKLEME");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            HCLevelManager.Instance.GenerateCurrentLevel();
            Debug.Log("YUKLEMEME");
        }
    }

    public void NextLvUI()
    {
        if (!isPaused) //if the game not stopped
        {
            tapToPlayUI.SetActive(false);
            nextLvMenuUI.SetActive(true);
            isPaused = true;
            //collectedMoneyText.text = "" + PlayerManagement.Instance.currentLvMoneyAmount + "$";
        }
    }

    public void RestartButtonUI()
    {
        if (!isPaused) //if the game not stopped
        {
            restartMenuUI.SetActive(true);
            isPaused = true;
        }
    }

    public void PauseButtonUI()
    {
        if (!isPaused) //if the game not stopped
        {
            tapToPlayUI.SetActive(true);
            isPaused = true;
        }
    }

    public void UIVibrationToggle(bool checkOnOff)
    {
        if (checkOnOff)
        {
            vibrationToggle.GetComponent<Toggle>().isOn = true;
            PlayerPrefs.SetInt("vibrationOnOff", 1);
        }
        else
        {
            vibrationToggle.GetComponent<Toggle>().isOn = false;
            PlayerPrefs.SetInt("vibrationOnOff", 0);
        }
    }

    public void UISoundToggle(bool checkOnOff)
    {
        if (checkOnOff)
        {
            soundToggle.GetComponent<Toggle>().isOn = true;
            PlayerPrefs.SetInt("soundOnOff", 1);
        }
        else
        {
            soundToggle.GetComponent<Toggle>().isOn = false;
            PlayerPrefs.SetInt("soundOnOff", 0);
        }
    }

    public void LevelText()
    {
        int levelInt = HCLevelManager.Instance.GetGlobalLevelIndex() + 1;
        currentLV.text = "Level " + levelInt;
    }

    public void SetProgress(float progress)
    {
        progressBarImage.fillAmount = progress;
    }

    public void SetActiveProgressBar(bool check)
    {
        if(check)
            progressBar.SetActive(true);
        else
            progressBar.SetActive(false);
    }

    public void SetTotalMoney()
    {
        totalMoneyText.text = "" + PlayerPrefs.GetInt("TotalMoney", 0) + "$";
    }

    public void MoveMultiplierArrow()
    {
        arrow.DOLocalMoveX(-4.4f, 0);
        arrow.DOLocalMoveX(4.4f, 2).SetEase(Ease.InOutQuint).SetLoops(-1, LoopType.Yoyo);
        StartCoroutine(HandTransform());
    }

    IEnumerator HandTransform()
    {
        float arrowXpos = arrow.localPosition.x;

        if (arrowXpos > -4.5f && arrowXpos < -3.5f)
        {
            SetMuiltiplier(1);
        }

        if (arrowXpos > -3.5f && arrowXpos < -2.5f)
        {
            SetMuiltiplier(2);
        }

        if (arrowXpos > -2.5f && arrowXpos < -1.5f)
        {
            SetMuiltiplier(3);
        }

        if (arrowXpos > -1.5f && arrowXpos < -0.5f)
        {
            SetMuiltiplier(4);
        }

        if (arrowXpos > -0.5f && arrowXpos < 0.5f)
        {
            SetMuiltiplier(5);
        }

        if (arrowXpos > 0.5f && arrowXpos < 1.5f)
        {
            SetMuiltiplier(4);
        }

        if (arrowXpos > 1.5f && arrowXpos < 2.5f)
        {
            SetMuiltiplier(3);
        }

        if (arrowXpos > 2.5f && arrowXpos < 3.5f)
        {
            SetMuiltiplier(2);
        }

        if (arrowXpos > 3.5f && arrowXpos < 4.5f)
        {
            SetMuiltiplier(1);
        }

        if (!clickBonusCheck)
        {
            yield return new WaitForFixedUpdate();
            StartCoroutine(HandTransform());
        }
        else
        {
            SlapButton();
        }
    }

    //RENKLERI AYARLAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
    private void SetMuiltiplier(float multiplierInt)
    {
        slapBoardText.text = "POWER ->" + PlayerManagement.Instance.ReturnPower() * multiplierInt;
        multiplier = multiplierInt;

        if (multiplierInt == 1)
        {
            slapBoardText.color = new Color32(231, 12, 12, 255);
        }

        if (multiplierInt == 2)
        {
            slapBoardText.color = new Color32(255, 153, 21, 255);
        }

        if (multiplierInt == 3)
        {
            slapBoardText.color = new Color32(250, 205, 51, 255);
        }

        if (multiplierInt == 4)
        {
            slapBoardText.color = new Color32(105, 179, 76, 255);
        }

        if (multiplierInt == 5)
        {
            slapBoardText.color = new Color32(105, 179, 76, 255);
        }
    }

    public void SlapButton()
    {
        clickBonusCheck = true;
        StopCoroutine(HandTransform());
        DOTween.Kill(arrow.transform);
        ResSlapButton();
    }

    public void TurnOnOffUIs(bool check)
    {
        if (check)
        {
            bossManager = GameObject.FindObjectOfType<BossManager>();
            playerHealtUI.SetActive(true);
            bossHealtUI.SetActive(true);
        }
        else
        {
            playerHealtUI.SetActive(false);
            bossHealtUI.SetActive(false);
        }
    }

    public void SetHealthUIs()
    {
        playerHealthImage.fillAmount = PlayerManagement.Instance.ReturnHealth();
        //Debug.Log(PlayerManagement.Instance.ReturnHealth());
        bossHealthImage.fillAmount = bossManager.ReturnHealth();
        //Debug.Log(bossHealthImage.fillAmount);
    }

    public void ResSlapButton()
    {
        clickBonusCheck = false;
        slapBoardText.text = "POWER -> 50";
    }

    public void UIQuitGame()
    {
        Application.Quit();
    }
}
