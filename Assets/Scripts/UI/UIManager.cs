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

    [Header("Status UI Texts")]
    [SerializeField] private TMP_Text currentLV;
    [SerializeField] private TMP_Text totalMoneyText;
    [SerializeField] private GameObject nextLevelButton;
    [SerializeField] private Image healthImage;

    private int multiplierCounter = 0;

    //status texts
    [SerializeField] private GameObject playerHealtUI;
    [SerializeField] private GameObject bossHealtUI;
    [SerializeField] private Image progressBarImage;
    [SerializeField] private GameObject progressBar;
    [SerializeField] private Image playerHealthImage;
    [SerializeField] private Image bossHealthImage;

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
            //HCLevelManager.Instance.GenerateCurrentLevel();
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

    public void UIQuitGame()
    {
        Application.Quit();
    }
}
