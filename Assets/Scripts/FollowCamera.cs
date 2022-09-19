using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    //components
    Transform followTarget = null;

    //constant value
    [SerializeField] float distanceCam2Target = 10f;
    [SerializeField] float pitchCam2Target = 25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (followTarget == null) return;

        transform.position = followTarget.position - followTarget.forward * distanceCam2Target;
        transform.RotateAround(followTarget.position, followTarget.right, pitchCam2Target);
        transform.LookAt(followTarget);
    }

    public void UpdateTarget(Transform target)
    {
        followTarget = target;
    }
}
