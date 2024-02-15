/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, January 2024
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static OVRSkeleton;

namespace AvatarSDK.MetaPerson.Oculus
{
	[Serializable]
	public class BoneTransform
	{
		public OVRBodyBoneId boneId;
		public Transform transform;
	}

	public class OVRToMetaPersonSkeletonSync : MonoBehaviour
	{
		[Range(0.0f, 1.0f)]
		public float twist1Coeff = 0.5f;
		[Range(0.0f, 1.0f)]
		public float twist2Coeff = 1.0f;

		public bool moveHips = true;

		public SkeletonMapping skeletonMapping;

		public bool syncBonesWithOtherModel = false;

		public GameObject sourceBonesModel = null;

		public bool feetPositionInWorldSpace = true;

		public List<BoneTransform> sourceBones = new List<BoneTransform>();

		private Transform[] mpBones;

		private IOVRSkeletonDataProvider skeletonDataProvider;

		private OVRTransformsPositioner transformsPositioner;

		private ForeArmTwistAdjustment leftHandTwistAdjustment;
		private ForeArmTwistAdjustment rightHandTwistAdjustment;

		private List<TwoBoneIK> bonesIKs = new List<TwoBoneIK>();

		private void Start()
		{
			Transform[] avatarTransforms = GetComponentsInChildren<Transform>();

			mpBones = new Transform[(int)OVRBodyBoneId.LowerBody_End];
			for (int i = (int)OVRBodyBoneId.Body_Start; i < (int)OVRBodyBoneId.LowerBody_End; i++)
			{
				OVRBodyBoneId boneId = (OVRBodyBoneId)i;

				string mpBoneName = BonesMapping.boneIdToMetaPersonBones[boneId];
				if (!string.IsNullOrEmpty(mpBoneName))
					mpBones[i] = avatarTransforms.FirstOrDefault(t => t.name == mpBoneName);
			}

			skeletonDataProvider = FindSkeletonDataProvider();

			Transform leftForearmTwist1 = GetTransformByName(avatarTransforms, BonesMapping.leftForeArmTwist1Name);
			Transform leftForearmTwist2 = GetTransformByName(avatarTransforms, BonesMapping.leftForeArmTwist2Name);
			Transform leftHand = GetTransformByName(avatarTransforms, BonesMapping.leftHandName);
			Transform leftForeArm = GetTransformByName(avatarTransforms, BonesMapping.leftForeArmName);
			if (leftForearmTwist1 != null && leftForearmTwist2 != null && leftHand != null && leftForeArm != null)
				leftHandTwistAdjustment = new ForeArmTwistAdjustment(leftHand, leftForeArm, leftForearmTwist1, leftForearmTwist2);

			Transform rightForearmTwist1 = GetTransformByName(avatarTransforms, BonesMapping.rightForeArmTwist1Name);
			Transform rightForearmTwist2 = GetTransformByName(avatarTransforms, BonesMapping.rightForeArmTwist2Name);
			Transform rightHand = GetTransformByName(avatarTransforms, BonesMapping.rightHandName);
			Transform rightForeArm = GetTransformByName(avatarTransforms, BonesMapping.rightForeArmName);
			if (rightForearmTwist1 != null && rightForearmTwist2 != null && rightHand != null && rightForeArm != null)
				rightHandTwistAdjustment = new ForeArmTwistAdjustment(rightHand, rightForeArm, rightForearmTwist1, rightForearmTwist2);

			if (transformsPositioner == null)
				transformsPositioner = GetComponentInChildren<OVRTransformsPositioner>();

			bonesIKs = GetComponentsInChildren<TwoBoneIK>().ToList();
		}

		private void LateUpdate()
		{
			PoseData poseData = CreateBlankPoseData();
			if (skeletonDataProvider != null)
			{
				var data = skeletonDataProvider.GetSkeletonPoseData();
				if (data.IsDataValid)
					TakePoseFromSkeletonPoseData(ref poseData, data);
			}

			if (syncBonesWithOtherModel)
			{
				TakePoseFromSourceBones(ref poseData);
			}

			UpdateBonesPositions(poseData);
		}

		public void UpdateBonesPositions(SkeletonPoseData skeletonPoseData)
		{
			PoseData poseData = CreateBlankPoseData();
			TakePoseFromSkeletonPoseData(ref poseData, skeletonPoseData);
			UpdateBonesPositions(poseData);
		}

		public void UpdateBonesPositions(PoseData poseData)
		{
			Vector3 avatarPosition = transform.position;
			Quaternion avatarRotation = transform.rotation;
			Matrix4x4 avatartTransformMat = transform.localToWorldMatrix;

			for (int i = (int)OVRBodyBoneId.Body_Start; i < (int)OVRBodyBoneId.LowerBody_End; i++)
			{
				if (poseData.isValidData[i])
				{
					OVRBodyBoneId boneId = (OVRBodyBoneId)i;
					Transform mpBone = mpBones[i];
					if (mpBone != null)
					{
						Quaternion rotation = avatarRotation * poseData.rotations[i];
						if (boneId == OVRBodyBoneId.Body_Hips)
						{
							if (moveHips)
							{
								mpBone.position = avatartTransformMat.MultiplyPoint(poseData.positions[i]);
								mpBone.rotation = rotation;
							}
						}
						else
							mpBone.rotation = rotation;
					}
				}
			}

			if (transformsPositioner != null)
				transformsPositioner.UpdatePositions(poseData, avatarPosition, avatarRotation);

			foreach (var twoBoneIk in bonesIKs)
				twoBoneIk.ForceUpdate();

			if (skeletonMapping.skeletonType == MetaPersonSkeletonType.Male)
			{
				//right hand adjustments
				if (poseData.isValidData[(int)OVRBodyBoneId.Body_RightHandWrist])
				{
					Transform rightLittleProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleProximal];
					Transform rightLittleIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleIntermediate];
					Transform rightLittleDistalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleDistal];
					Transform rightRingProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandRingProximal];
					Transform rightRingIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandRingIntermediate];
					Transform rightMiddleProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleProximal];
					Transform rightMiddleIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate];
					Transform rightMiddleDistalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleDistal];
					Transform rightIndexProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandIndexProximal];
					Transform rightThumbMetaTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal];
					Transform rightThumbProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandThumbProximal];

					rightLittleIntermediateTransform.Rotate(rightLittleIntermediateTransform.position - rightLittleProximalTransform.position, 10, Space.World);
					rightMiddleIntermediateTransform.Rotate(rightMiddleIntermediateTransform.position - rightMiddleProximalTransform.position, -5, Space.World);
					rightMiddleDistalTransform.Rotate(rightMiddleDistalTransform.position - rightMiddleIntermediateTransform.position, 10, Space.World);

					ShiftFingerIfTooCloseToNearby(rightRingProximalTransform, rightRingIntermediateTransform, rightMiddleIntermediateTransform, 17);
					ShiftFingerIfTooCloseToNearby(rightLittleProximalTransform, rightLittleIntermediateTransform, rightRingIntermediateTransform, 20);
					ShiftFingerIfTooCloseToNearby(rightThumbMetaTransform, rightThumbProximalTransform, rightIndexProximalTransform, 40);
				}

				//left hand adjustments
				if (poseData.isValidData[(int)OVRBodyBoneId.Body_LeftHandWrist])
				{
					Transform leftLittleProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleProximal];
					Transform leftLittleIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate];
					Transform leftLittleDistalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleDistal];
					Transform leftRingProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandRingProximal];
					Transform leftRingIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandRingIntermediate];
					Transform leftMiddleProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleProximal];
					Transform leftMiddleIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate];
					Transform leftMiddleDistalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleDistal];
					Transform leftIndexProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandIndexProximal];
					Transform leftThumbMetaTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbMetacarpal];
					Transform leftThumbProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbProximal];

					leftLittleIntermediateTransform.Rotate(leftLittleIntermediateTransform.position - leftLittleProximalTransform.position, -10, Space.World);
					leftMiddleIntermediateTransform.Rotate(leftMiddleIntermediateTransform.position - leftMiddleProximalTransform.position, 5, Space.World);
					leftMiddleDistalTransform.Rotate(leftMiddleDistalTransform.position - leftMiddleIntermediateTransform.position, -10, Space.World);

					ShiftFingerIfTooCloseToNearby(leftRingProximalTransform, leftRingIntermediateTransform, leftMiddleIntermediateTransform, 17);
					ShiftFingerIfTooCloseToNearby(leftLittleProximalTransform, leftLittleIntermediateTransform, leftRingIntermediateTransform, 20);
					ShiftFingerIfTooCloseToNearby(leftThumbMetaTransform, leftThumbProximalTransform, leftIndexProximalTransform, 40);
				}
			}
			else if (skeletonMapping.skeletonType == MetaPersonSkeletonType.Female)
			{
				//right hand adjustments
				if (poseData.isValidData[(int)OVRBodyBoneId.Body_RightHandWrist])
				{
					Transform rightLittleProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleProximal];
					Transform rightLittleIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleIntermediate];
					Transform rightRingProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandRingProximal];
					Transform rightRingIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandRingIntermediate];
					Transform rightMiddleIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate];
					Transform rightMiddleDistalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleDistal];
					Transform rightIndexProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandIndexProximal];
					Transform rightThumbMetaTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal];
					Transform rightThumbProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_RightHandThumbProximal];

					rightLittleIntermediateTransform.Rotate(rightLittleIntermediateTransform.position - rightLittleProximalTransform.position, 10, Space.World);
					rightMiddleDistalTransform.Rotate(rightMiddleDistalTransform.position - rightMiddleIntermediateTransform.position, 10, Space.World);
					rightRingIntermediateTransform.Rotate(rightRingIntermediateTransform.position - rightRingProximalTransform.position, 10, Space.World);

					ShiftFingerIfTooCloseToNearby(rightThumbMetaTransform, rightThumbProximalTransform, rightIndexProximalTransform, 40);
				}

				//left hand adjustments
				if (poseData.isValidData[(int)OVRBodyBoneId.Body_LeftHandWrist])
				{
					Transform leftLittleProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleProximal];
					Transform leftLittleIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate];
					Transform leftRingProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandRingProximal];
					Transform leftRingIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandRingIntermediate];
					Transform leftMiddleIntermediateTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate];
					Transform leftMiddleDistalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleDistal];
					Transform leftIndexProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandIndexProximal];
					Transform leftThumbMetaTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbMetacarpal];
					Transform leftThumbProximalTransform = mpBones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbProximal];

					leftLittleIntermediateTransform.Rotate(leftLittleIntermediateTransform.position - leftLittleProximalTransform.position, -10, Space.World);
					leftMiddleDistalTransform.Rotate(leftMiddleDistalTransform.position - leftMiddleIntermediateTransform.position, -10, Space.World);
					leftRingIntermediateTransform.Rotate(leftRingIntermediateTransform.position - leftRingProximalTransform.position, -10, Space.World);

					ShiftFingerIfTooCloseToNearby(leftThumbMetaTransform, leftThumbProximalTransform, leftIndexProximalTransform, 40);
				}
			}

			if (leftHandTwistAdjustment != null)
				leftHandTwistAdjustment.Update(twist1Coeff, twist2Coeff);
			if (rightHandTwistAdjustment != null)
				rightHandTwistAdjustment.Update(twist1Coeff, twist2Coeff);
		}

		public void TakeAllBonesToSyncFromSourceModel()
		{
			if (sourceBonesModel != null)
			{
				sourceBones.Clear();

				Transform[] ovrTransforms = sourceBonesModel.GetComponentsInChildren<Transform>();
				for (int i = (int)OVRBodyBoneId.Body_Start; i < (int)OVRBodyBoneId.LowerBody_End; i++)
				{
					OVRBodyBoneId boneId = (OVRBodyBoneId)i;
					string ovrBoneName = BonesMapping.boneIdToOVRBones[boneId];
					if (!string.IsNullOrEmpty(ovrBoneName))
					{
						Transform ovrTransform = ovrTransforms.FirstOrDefault(t => t.name == ovrBoneName);
						if (ovrTransform != null)
							sourceBones.Add(new BoneTransform() { boneId = boneId, transform = ovrTransform });
					}
				}
			}
		}

		private Transform GetTransformByName(Transform[] transforms, string name)
		{
			return transforms.FirstOrDefault(t => t.name == name);
		}

		private void ShiftFingerIfTooCloseToNearby(Transform proximal, Transform intermediate, Transform nearby, float maxAngle)
		{
			Vector3 proximalToNearby = nearby.position - proximal.position;
			Vector3 proximalToIntermediate = intermediate.position - proximal.position;

			float angle = Vector3.Angle(proximalToNearby, proximalToIntermediate);
			if (angle < maxAngle)
			{
				Vector3 rotationAxis = Vector3.Cross(proximalToNearby, proximalToIntermediate).normalized;
				proximal.Rotate(rotationAxis, maxAngle - angle, Space.World);
			}
		}

		private IOVRSkeletonDataProvider FindSkeletonDataProvider()
		{
			var dataProviders = gameObject.GetComponentsInParent<IOVRSkeletonDataProvider>();
			foreach (var dataProvider in dataProviders)
			{
				if (dataProvider.GetSkeletonType() == SkeletonType.Body)
				{
					return dataProvider;
				}
			}

			return null;
		}

		private PoseData CreateBlankPoseData()
		{
			PoseData mpPoseData = new PoseData();
			mpPoseData.positions = new Vector3[(int)OVRBodyBoneId.LowerBody_End];
			mpPoseData.rotations = new Quaternion[(int)OVRBodyBoneId.LowerBody_End];
			mpPoseData.isValidData = new bool[(int)OVRBodyBoneId.LowerBody_End];
			for (int i = (int)OVRBodyBoneId.Body_Start; i < (int)OVRBodyBoneId.LowerBody_End; i++)
			{
				mpPoseData.positions[i] = Vector3.zero;
				mpPoseData.rotations[i] = Quaternion.identity;
				mpPoseData.isValidData[i] = false;
			}
			return mpPoseData;
		}

		private void TakePoseFromSkeletonPoseData(ref PoseData poseData, SkeletonPoseData ovrPoseData)
		{
			for (int i = (int)OVRBodyBoneId.Body_Start; i < (int)OVRBodyBoneId.LowerBody_End; i++)
			{
				if (i < ovrPoseData.BoneTranslations.Length)
				{
					poseData.positions[i] = ovrPoseData.BoneTranslations[i].FromFlippedZVector3f();
					poseData.rotations[i] = skeletonMapping.OVRToMPRotation((OVRBodyBoneId)i, ovrPoseData.BoneRotations[i].FromFlippedZQuatf());
					poseData.isValidData[i] = true;
				}
			}
		}

		private void TakePoseFromSourceBones(ref PoseData poseData)
		{
			Quaternion invRootRotation = Quaternion.identity;
			Matrix4x4 toOvrRootMat = Matrix4x4.identity;
			if (sourceBonesModel != null)
			{
				invRootRotation = Quaternion.Inverse(sourceBonesModel.transform.rotation);
				toOvrRootMat = sourceBonesModel.transform.worldToLocalMatrix;
			}
			
			foreach(BoneTransform boneTransform in sourceBones)
			{
				if (feetPositionInWorldSpace &&
					(boneTransform.boneId == OVRBodyBoneId.LowerBody_LeftFoot ||
					boneTransform.boneId == OVRBodyBoneId.LowerBody_RightFoot))
				{
					int idx = (int)boneTransform.boneId;
					poseData.positions[idx] = boneTransform.transform.position;
					poseData.rotations[idx] = skeletonMapping.OVRToMPRotation(boneTransform.boneId, boneTransform.transform.rotation);
					poseData.isValidData[idx] = true;
				}
				else
				{
					int idx = (int)boneTransform.boneId;
					poseData.positions[idx] = toOvrRootMat.MultiplyPoint(boneTransform.transform.position);
					poseData.rotations[idx] = skeletonMapping.OVRToMPRotation(boneTransform.boneId, invRootRotation * boneTransform.transform.rotation);
					poseData.isValidData[idx] = true;
				}
			}
		}
	}
}
