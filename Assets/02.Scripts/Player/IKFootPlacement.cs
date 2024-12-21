using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IKFootPlacement : MonoBehaviour
{
    private static readonly int Moving = Animator.StringToHash("Moving");
    private static readonly int Grounded = Animator.StringToHash("Grounded");

    // Distance from Foot (ankle) to floor
    [SerializeField, Range(0f, 1f)] private float distanceToGround = 0.1f;
    // Layers to cast IK to (including Walkables, but excluding the Player itself)
    [SerializeField] private LayerMask raycastLayerMask;
    
    [SerializeField] private AnimationCurve leftFootWalkIKCurve;
    [SerializeField] private AnimationCurve rightFootWalkIKCurve;
    [SerializeField] private AnimationCurve leftFootRunIKCurve;
    [SerializeField] private AnimationCurve rightFootRunIKCurve;
    
    private Animator playerAnimator;
    private int runLayer = 1;
    
    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!playerAnimator) return;
        
        // IK only works when the foot touches the ground
        bool moving = playerAnimator.GetBool(Moving);
        bool grounded = playerAnimator.GetBool(Grounded);
        
        float nTime = playerAnimator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
        float runWeight = playerAnimator.GetLayerWeight(runLayer);
        float leftMovingWeight = Mathf.Lerp(leftFootWalkIKCurve.Evaluate(nTime), leftFootRunIKCurve.Evaluate(nTime), runWeight);
        float rightMovingWeight = Mathf.Lerp(rightFootWalkIKCurve.Evaluate(nTime), rightFootRunIKCurve.Evaluate(nTime), runWeight);
        
        float leftFootIKWeight = grounded ? (moving ? leftMovingWeight : 1f) : 0f;
        float rightFootIKWeight = grounded ? (moving ? rightMovingWeight : 1f) : 0f;
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootIKWeight);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootIKWeight);
        playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootIKWeight);
        playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootIKWeight);
        
        float detectDistance = 0.5f;
        
        // Raising the origin so that it can be detected even when the foot is buried
        transform.localPosition = Vector3.zero;
        Ray leftRay = new Ray(playerAnimator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
        Ray rightRay = new Ray(playerAnimator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
        bool leftIntersected = Physics.Raycast(leftRay, out RaycastHit leftHit, distanceToGround + detectDistance + 1f,
            raycastLayerMask);
        bool rightIntersected = Physics.Raycast(rightRay, out RaycastHit rightHit, distanceToGround + detectDistance + 1f,
            raycastLayerMask);
        
        if (leftIntersected && rightIntersected && grounded)
        {
            // Staying low on slopes where it's hard to reach your feet
            float lowestHeight = Mathf.Min(leftHit.point.y, rightHit.point.y);
            float originHeight = transform.parent.position.y;
            Vector3 loweredPosition = new Vector3(0f, lowestHeight - originHeight, 0f);
            transform.localPosition = loweredPosition;
            
            // if (leftHit.transform.tag == "Walkable")
            Vector3 leftFootPosition = leftHit.point;
            leftFootPosition.y += distanceToGround;
            playerAnimator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPosition);
            playerAnimator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, leftHit.normal));
            Debug.DrawRay(leftFootPosition, leftHit.normal, Color.green);
            
            // if (rightHit.transform.tag == "Walkable")
            Vector3 rightFootPosition = rightHit.point;
            rightFootPosition.y += distanceToGround;
            playerAnimator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPosition);
            playerAnimator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, rightHit.normal));
            Debug.DrawRay(rightFootPosition, rightHit.normal, Color.green);
        }
    }
}
