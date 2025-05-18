using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerController : MonoBehaviour
{
    private Transform quadTransform;
    private VideoPlayer videoPlayer;
    private float maxWidthHeight = 1f;

    private void Awake()
    {
        quadTransform = GetComponent<Transform>();
        videoPlayer = GetComponent<VideoPlayer>();
    }

    private void Start()
    {
        if (videoPlayer.clip)
        {
            videoPlayer.prepareCompleted += OnVideoPrepared;
            videoPlayer.Prepare();
        }
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

    private void OnVideoPrepared(VideoPlayer vp)
    {
        // Get video resolution
        float videoWidth = vp.width;
        float videoHeight = vp.height;

        float aspectRatio = videoWidth / videoHeight;

        if (videoWidth < videoHeight)
        {
            quadTransform.localScale = new Vector3(maxWidthHeight * aspectRatio, maxWidthHeight, 1f);
        }
        else
        {
            quadTransform.localScale = new Vector3(maxWidthHeight, maxWidthHeight / aspectRatio, 1f);
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
