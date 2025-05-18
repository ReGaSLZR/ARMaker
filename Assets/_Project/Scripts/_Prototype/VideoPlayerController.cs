using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (videoPlayer.isPlaying)
            {
                Stop();
            }
            else
            {
                Play();
            }
        }
    }

    [ContextMenu("Play")]
    public void Play()
    {
        videoPlayer.Play();
    }

    [ContextMenu("Stop")]
    public void Stop()
    {
        videoPlayer.Stop();
    }
}
