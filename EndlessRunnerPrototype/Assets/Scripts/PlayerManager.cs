using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static bool gameOver;
    public GameObject gameOverPanel;
    public GameObject videoBackgroundPanel;
    public static bool isGameStarted;

    public GameObject startingTxt;
    public static int totalPoints;
    public VideoManager instance;
    private bool gameOverFlag;
    // Start is called before the first frame update
    void Start()
    {
        gameOver = false;
        Time.timeScale = 1;
        isGameStarted = false;
        totalPoints = 0;
        gameOverFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver && gameOverFlag)
        {
        //    Time.timeScale = 0;
            videoBackgroundPanel.SetActive(true);
            instance.VideoBackground(true);
            Invoke("LosingVideo",2f);
        }

        if (SwipeManager.tap)
        {
            isGameStarted = true;
            Destroy(startingTxt);
        }
    }
     private void LosingVideo()
    {
        Time.timeScale = 0;
        gameOverFlag = false;
        videoBackgroundPanel.SetActive(false);
        instance.VideoBackground(false);
        gameOverPanel.SetActive(true);
        Debug.Log("LosingVideo called");
    }
}
