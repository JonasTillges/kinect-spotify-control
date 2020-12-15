using Spotify4Unity;
using Spotify4Unity.Dtos;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class HandGestures : MonoBehaviour
{
    private GameObject handLeft;
    private GameObject handRight;
    private GameObject head;
    public GameObject spotifyServiceObject;
    private SpotifyService spotifyService;
    public Material[] headMaterials;
    private Material _defaultHeadMaterial;
    private bool switched = false;

    private bool isExecuting = false;

    void Start()
    {
        spotifyService = spotifyServiceObject.GetComponent<SpotifyService>();
    }
    // Update is called once per frame
    void Update()
    {
        if (EnsureObjects())
        {
            float handLeftY = handLeft.transform.position.y;
            float handRightY = handRight.transform.position.y;
            float headY = head.transform.position.y;

            SwipeGestureAsync();
           
            if (headY < handLeftY && headY < handRightY)
            {
               
                //spotifyService.SetVolume(100)
                /*
                Task.Run(async () =>
                {
                    isExecuting = true;
                    await spotifyService.NextSongAsync();
                }).ContinueWith((response) => {
                    isExecuting = false;
                }); */
            }
     
       
            
        }
    }

    void SwipeGestureAsync()
    {
        float handRightXFirstSnapshot = handRight.transform.position.x;

        Task.Delay(1000).ContinueWith((response) =>
        {
            float handRightXSecoundSnapshot = handRight.transform.position.x;
            if ((handRightXFirstSnapshot - handRightXSecoundSnapshot) > 1)
            {
                spotifyService.SetVolume(100);
            }
        });
    }

    bool EnsureObjects()
    {
        if (handLeft == null)
        {
            handLeft = GameObject.FindWithTag("HandLeft");
        }
        if (handRight == null)
        {
            handRight = GameObject.FindWithTag("HandRight");
        }
        if (head == null)
        {
            head = GameObject.FindWithTag("Head");
        }

        if (handLeft == null || handRight == null || head == null)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
}
