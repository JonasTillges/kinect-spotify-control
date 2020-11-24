using Spotify4Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGestures : MonoBehaviour
{
    public GameObject handLeft;
    public GameObject handRight;
    public GameObject head;
    public GameObject spotifyServiceObject;
    private SpotifyService spotifyService;
    public Material[] headMaterials;
    private Material _defaultHeadMaterial;
    private bool switched = false;


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

            if (headY < handLeftY && headY < handRightY && headMaterials.Length > 0 && switched == false)
            {
                spotifyService.PlaySong("spotify:track:5rb9QrpfcKFHM1EUbSIurX");
            }
            
        }
    }

    bool EnsureObjects()
    {
        if (handLeft == null)
        {
            handLeft = transform.Find("HandLeft").gameObject;
        }
        if (handRight == null)
        {
            handRight = transform.Find("HandRight").gameObject;
        }
        if (head == null)
        {
            head = transform.Find("Head").gameObject;
            if (head != null)
            {
                _defaultHeadMaterial = head.GetComponent<Renderer>().material;
            }
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
