﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System;
using Windows.Kinect;
using Spotify4Unity;
using System.Threading.Tasks;

public class BodySourceView : MonoBehaviour 
{
    public Material BoneMaterial;
    public Material HandMaterial;
    public Material HeadMaterial;
    public GameObject BodySourceManager;
    public GameObject spotifyServiceObject;
    public GameObject LeftHandObject;
    private SpotifyService spotifyService;
    private bool isExecuting = false;

    //SpotifyService spotifyService = GameObject.Find("SpotifyService").GetComponent<SpotifyService>();
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;
    
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },
        
        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },
        
        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    
    void Start()
    {
        spotifyService = spotifyServiceObject.GetComponent<SpotifyService>();


    }
    

    void Update()
    {
        if (BodySourceManager == null)
        {
            return;
        }
        
        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null)
        {
            return;
        }
        
        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null)
        {
            return;
        }
        
        List<ulong> trackedIds = new List<ulong>();
        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
                
            if(body.IsTracked)
            {
                trackedIds.Add (body.TrackingId);
            } 
        }
        
        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);
        
        // First delete untracked bodies
        foreach(ulong trackingId in knownIds)
        {
            if(!trackedIds.Contains(trackingId))
            {
                Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        foreach(var body in data)
        {
            if (body == null)
            {
                continue;
            }
            
            if(body.IsTracked)
            {
                if(!_Bodies.ContainsKey(body.TrackingId))
                {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);


                if (!isExecuting) {
                    switch (body.HandLeftState)
                    {
                        case HandState.Open:
                            break;
                        case HandState.Closed:
                            Task.Run(async () =>
                            {
                                isExecuting = true;
                                await spotifyService.PauseAsync();
                            }).ContinueWith((response) => {
                                isExecuting = false;
                            });
                            break;
                        case HandState.Lasso:
                            break;
                        case HandState.Unknown:
                            break;
                        case HandState.NotTracked:
                            break;
                        default:
                            break;
                    }
                    
                    switch (body.HandRightState)
                    {
                        case HandState.Open:
                            Task.Run(async () =>
                            {
                                isExecuting = true;
                                await spotifyService.PlayAsync();
                            }).ContinueWith((response) => {
                                isExecuting = false;
                            });
                            break;
                        case HandState.Closed:
                            break;
                        case HandState.Lasso:
                            break;
                        case HandState.Unknown:
                            break;
                        case HandState.NotTracked:
                            break;
                        default:
                            break;
                    }
                }
     


            }
        }
    }




    private GameObject CreateBodyObject(ulong id)
    {
        GameObject body = new GameObject("Body:" + id);
        
        
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.05f, 0.05f);
            
            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString();
            jointObj.transform.parent = body.transform;


            string jtName = jt.ToString();

            if (jtName == "Head")
            {
                jointObj.transform.localScale = new Vector3(3f, 3f, 3f);
                jointObj.GetComponent<MeshRenderer>().material = HeadMaterial;
                jointObj.tag = "Head";


            }
            else if (jtName == "HandRight")
            {
                //jointObj.transform.localScale = new Vector3(3f, 3f, 3f);
                //jointObj.transform.rotation = new Quaternion(0, 180, 0, 0);
                //jointObj.GetComponent<MeshRenderer>().material = HandMaterial;
                //jointObj.AddComponent<BoxCollider>();
                //jointObj.AddComponent<Rigidbody>();
                //jointObj.GetComponent<Rigidbody>().isKinematic = true;
                jointObj.tag = "HandRight";
            }
            else if (jtName == "HandLeft")
            {
                //jointObj.transform.localScale = new Vector3(3f, 3f, 3f);
                //jointObj.transform.rotation = new Quaternion(0, 180, 0, 0);
                //jointObj.GetComponent<MeshRenderer>().material = HandMaterial;
                //if (isExecuting) {
                  //  jointObj.GetComponent<MeshRenderer>().material.color = Color.red;
                //}

                //jointObj.AddComponent<BoxCollider>();
                //jointObj.AddComponent<Rigidbody>();
                //jointObj.GetComponent<Rigidbody>().isKinematic = true;
                jointObj.tag = "HandLeft";
            }
        }
        
        return body;
    }
    
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.Joint? targetJoint = null;
            
            if(_BoneMap.ContainsKey(jt))
            {
                targetJoint = body.Joints[_BoneMap[jt]];
            }
            
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);
            
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if(targetJoint.HasValue)
            {
                lr.SetPosition(0, jointObj.localPosition);
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                
            }
            else
            {
                lr.enabled = false;
            }

            
        }
    }
    
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
        case Kinect.TrackingState.Tracked:
            return Color.green;

        case Kinect.TrackingState.Inferred:
            return Color.red;

        default:
            return Color.black;
        }
    }
    
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
    }
}
