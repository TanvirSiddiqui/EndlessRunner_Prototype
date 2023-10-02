using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoManager : MonoBehaviour
{
    public GameObject videoPlayer;
    public static VideoManager videoManager;
    // Start is called before the first frame update
    void Start()
    {
        videoPlayer.SetActive(false);
    }

   public void VideoBackground(bool condition)
    {
        videoPlayer.SetActive(condition);
    }
}
