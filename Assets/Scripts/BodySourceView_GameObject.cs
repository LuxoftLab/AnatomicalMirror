using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;

public class BodySourceView_GameObject : MonoBehaviour {
    //public Material BoneMaterial;
    public GameObject BodySourceManager;
    public GameObject goBody;

    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager _BodyManager;

    private Dictionary<Kinect.JointType, string[]> _MixamoRigLinkMap = new Dictionary<Kinect.JointType, string[]>() {
        { Kinect.JointType.SpineBase, new string[] {"mixamorig:Hips/mixamorig:Spine"} },// /mixamorig:Spine
        { Kinect.JointType.SpineMid, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1"} },///mixamorig:Spine1
        //{ Kinect.JointType.SpineShoulder, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2" },
        { Kinect.JointType.Neck, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck"}},
        { Kinect.JointType.Head, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:Neck/mixamorig:Head/mixamorig:HeadTop_End"}},

        //{ Kinect.JointType.FootLeft, "mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot/mixamorig:LeftToeBase" },
        //{ Kinect.JointType.AnkleLeft, "mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot" },
        //{ Kinect.JointType.KneeLeft, "mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg" },
        //{ Kinect.JointType.HipLeft, "mixamorig:Hips/mixamorig:LeftUpLeg" },

        //{ Kinect.JointType.FootRight, "mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot/mixamorig:RightToeBase" },
        //{ Kinect.JointType.AnkleRight, "mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot" },
        //{ Kinect.JointType.KneeRight, "mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg" },
        //{ Kinect.JointType.HipRight, "mixamorig:Hips/mixamorig:RightUpLeg" },


        { Kinect.JointType.FootRight, new string[] {"mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot/mixamorig:LeftToeBase" } },
        { Kinect.JointType.AnkleRight, new string[] {"mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot" } },
        { Kinect.JointType.KneeRight, new string[] {"mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg" } },
        { Kinect.JointType.HipRight, new string[] {"mixamorig:Hips/mixamorig:LeftUpLeg" } },

        { Kinect.JointType.FootLeft, new string[] {"mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot/mixamorig:RightToeBase" } },
        { Kinect.JointType.AnkleLeft, new string[] {"mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot" } },
        { Kinect.JointType.KneeLeft, new string[] {"mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg" } },
        { Kinect.JointType.HipLeft, new string[] {"mixamorig:Hips/mixamorig:RightUpLeg" } },



        { Kinect.JointType.HandTipRight, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/mixamorig:LeftHandIndex1" } },
        { Kinect.JointType.ThumbRight, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/mixamorig:LeftHandThumb1" } },
        { Kinect.JointType.HandRight, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand" } },
        { Kinect.JointType.WristRight, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm" } },
        { Kinect.JointType.ElbowRight, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm" } },
        { Kinect.JointType.ShoulderRight, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder" } },

        { Kinect.JointType.HandTipLeft, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/mixamorig:RightHandIndex1" } },
        { Kinect.JointType.ThumbLeft, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/mixamorig:RightHandThumb1" } },
        { Kinect.JointType.HandLeft, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand" } },
        { Kinect.JointType.WristLeft, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm" } },
        { Kinect.JointType.ElbowLeft, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm" } },
        { Kinect.JointType.ShoulderLeft, new string[] {"mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder" } },

        //{ Kinect.JointType.HandTipRight, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/mixamorig:LeftHandIndex1" },
        //{ Kinect.JointType.ThumbRight, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand/mixamorig:LeftHandThumb1" },
        //{ Kinect.JointType.HandRight, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm/mixamorig:LeftHand" },
        //{ Kinect.JointType.WristRight, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm/mixamorig:LeftForeArm" },
        //{ Kinect.JointType.ElbowRight, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder/mixamorig:LeftArm" },
        //{ Kinect.JointType.ShoulderRight, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:LeftShoulder" },

        //{ Kinect.JointType.HandTipLeft, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/mixamorig:RightHandIndex1" },
        //{ Kinect.JointType.ThumbLeft, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand/mixamorig:RightHandThumb1" },
        //{ Kinect.JointType.HandLeft, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand" },
        //{ Kinect.JointType.WristLeft, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm" },
        //{ Kinect.JointType.ElbowLeft, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm" },
        //{ Kinect.JointType.ShoulderLeft, "mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder" },


    };

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

    private Kinect.Body _body = null;

    void Update() {
        Animator av = goBody.GetComponent<Animator>();

        if (BodySourceManager == null) {
            return;
        }

        _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
        if (_BodyManager == null) {
            return;
        }

        Kinect.Body[] data = _BodyManager.GetData();
        if (data == null) {
            return;
        }

        List<ulong> trackedIds = new List<ulong>();
        foreach (var body in data) {
            if (body == null) {
                continue;
            }

            if (body.IsTracked) {
                trackedIds.Add(body.TrackingId);
            }
        }

        List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

        // First delete untracked bodies
        foreach (ulong trackingId in knownIds) {
            if (!trackedIds.Contains(trackingId)) {
                //Destroy(_Bodies[trackingId]);
                _Bodies.Remove(trackingId);
            }
        }

        

        foreach (var body in data) {
            if (body == null) {
                continue;
                _body = null;
            }
            _body = body;

            if (body.IsTracked) {
                if (!_Bodies.ContainsKey(body.TrackingId)) {
                    _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                }
                
                RefreshBodyObject(body, _Bodies[body.TrackingId]);
            }
        }
    }

    private GameObject CreateBodyObject(ulong id) {
        //GameObject body = Instantiate(bodyPrefab, Vector3.zero, Quaternion.identity) as GameObject;//new GameObject("Body:" + id);
        //body.name = string.Format("Body: {0}", id);
        
        //for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
        //    string[] jointNames;
        //    if (!_MixamoRigLinkMap.TryGetValue(jt, out jointNames)) {
        //        Debug.LogWarning("Can't find joint: " + jt.ToString());
        //        continue;
        //    }
        //    foreach (var name in jointNames) {
        //        Transform jointObj = goBody.transform.FindChild(name);
        //    }
        //    //GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    //transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        //    //body.transform.FindChild(_MixamoRigLinkMap[jt]);

        //    //LineRenderer lr = jointObj.AddComponent<LineRenderer>();
        //    //lr.SetVertexCount(2);
        //    //lr.material = BoneMaterial;
        //    //lr.SetWidth(0.05f, 0.05f);

        //    //jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        //    //jointObj.name = jt.ToString();
        //    //jointObj.transform.parent = body.transform;
        //}

        return goBody;
    }

    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject) {
        Transform jointObj = bodyObject.transform.FindChild("mixamorig:Hips");// / mixamorig:Spine / mixamorig:Spine1
        var joint = body.Joints[Kinect.JointType.SpineBase];
        var jointOrientation = body.JointOrientations[Kinect.JointType.SpineBase];
        var offset = bodyObject.transform.position;
        //bodyObject.transform.position = GetVector3FromJoint(joint);
        if (joint != null) {
            jointObj.position = GetVector3FromJoint(joint) + offset;
            jointObj.rotation = GetQuaternionFromJointOrientation(jointOrientation);
        }

        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
            Kinect.Joint sourceJoint = body.Joints[jt];
            Kinect.JointOrientation sourceJointOrientation = body.JointOrientations[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt)) {
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            string[] jointNames;
            if (!_MixamoRigLinkMap.TryGetValue(jt, out jointNames)) {
                Debug.LogWarning("Can't find joint: " + jt.ToString());
                continue;
            }

            foreach(string name in jointNames) {
                jointObj = bodyObject.transform.FindChild(name);//
                jointObj.position/*localPosition */= GetVector3FromJoint(sourceJoint) + offset;
                jointObj.rotation = GetQuaternionFromJointOrientation(sourceJointOrientation);
            }
            

            //LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue) {
                //lr.SetPosition(0, jointObj.localPosition);
                //lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                //lr.SetColors(GetColorForState (sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
            }
            //else
            //{
            //    //lr.enabled = false;
            //}
        }
    }

    //private static Color GetColorForState(Kinect.TrackingState state) {
    //    switch (state) {
    //        case Kinect.TrackingState.Tracked:
    //            return Color.green;

    //        case Kinect.TrackingState.Inferred:
    //            return Color.red;

    //        default:
    //            return Color.black;
    //    }
    //}

    

    private static Vector3 GetVector3FromJoint(Kinect.Joint joint) {
        float k = 1f;
        return new Vector3(joint.Position.X /** k*/, joint.Position.Y /** k*/, joint.Position.Z /** k*/);
    }

    private static Quaternion GetQuaternionFromJointOrientation(Kinect.JointOrientation orientation) {
        var v4 = orientation.Orientation;
        float k = 1f;
        return new Quaternion(v4.X * k, v4.Y * k, v4.Z * k, v4.W * k);
    }

    void OnDrawGizmos() {
        if(_body == null) {
            return;
        }
        //Color[] colors = { Color.red, Color.blue, Color.cyan, Color.yellow, Color.magenta, Color.white, Color.black };
        Gizmos.color = Color.red;//colors[Random.Range(0, colors.Length)];
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++) {
            Kinect.Joint sourceJoint = _body.Joints[jt];
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt)) {
                targetJoint = _body.Joints[_BoneMap[jt]];
            }

            string[] jointNames;
            if (!_MixamoRigLinkMap.TryGetValue(jt, out jointNames)) {
                Debug.LogWarning("Can't find joint: " + jt.ToString());
                continue;
            }

            foreach(var name in jointNames) {
                var jointObj = goBody.transform.FindChild(name);
                Gizmos.DrawCube(jointObj.position, new Vector3(0.1f, 0.1f, 0.1f));
            }
            
        }
    }
}
