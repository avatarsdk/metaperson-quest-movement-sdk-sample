/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2023
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AvatarSDK.MetaPerson.Oculus
{
	public class OVRMetaPersonSkeleton : OVRCustomSkeleton
	{
		[Range(0.0f, 1.0f)]
		public float twist1Coeff = 0.5f;
		[Range(0.0f, 1.0f)]
		public float twist2Coeff = 1.0f;

		public bool moveHips = true;

		public bool alignFingersWithIK = false;

		public SkeletonMapping skeletonMapping;

		public OVRTransformsPositioner transformsPositioner;

		private IOVRSkeletonDataProvider skeletonDataProvider;

		private GameObject additionalBones;
		private HashSet<BoneId> additionalBonesIds = new HashSet<BoneId>();

		private ForeArmTwistAdjustment leftHandTwistAdjustment;
		private ForeArmTwistAdjustment rightHandTwistAdjustment;

		private List<TwoBoneIK> bonesIKs = new List<TwoBoneIK>();
		private List<IBonesPositioner> bonesPositioners = new List<IBonesPositioner>();

		protected override void Start()
		{
			base.Start();

			Transform leftForearmTwist1 = FindChildByName(BonesMapping.leftForeArmTwist1Name);
			Transform leftForearmTwist2 = FindChildByName(BonesMapping.leftForeArmTwist2Name);
			Transform leftHand = FindChildByName(BonesMapping.leftHandName);
			Transform leftForeArm = FindChildByName(BonesMapping.leftForeArmName);
			if (leftForearmTwist1 != null && leftForearmTwist2 != null && leftHand != null && leftForeArm != null)
				leftHandTwistAdjustment = new ForeArmTwistAdjustment(leftHand, leftForeArm, leftForearmTwist1, leftForearmTwist2);

			Transform rightForearmTwist1 = FindChildByName(BonesMapping.rightForeArmTwist1Name);
			Transform rightForearmTwist2 = FindChildByName(BonesMapping.rightForeArmTwist2Name);
			Transform rightHand = FindChildByName(BonesMapping.rightHandName);
			Transform rightForeArm = FindChildByName(BonesMapping.rightForeArmName);
			if (rightForearmTwist1 != null && rightForearmTwist2 != null && rightHand != null && rightForeArm != null)
				rightHandTwistAdjustment = new ForeArmTwistAdjustment(rightHand, rightForeArm, rightForearmTwist1, rightForearmTwist2);

			if (transformsPositioner == null)
				transformsPositioner = GetComponentInChildren<OVRTransformsPositioner>();

			bonesIKs = GetComponentsInChildren<TwoBoneIK>().ToList();
			bonesPositioners = GetComponentsInChildren<IBonesPositioner>().ToList();
		}

		protected override void Update()
		{
			ForceUpdateMetaPersonSkeleton();
		}

		public void MapBones()
		{
			var transforms = GetComponentsInChildren<Transform>();
			foreach (var boneMap in BonesMapping.boneIdToMetaPersonBones)
			{
				if (!string.IsNullOrEmpty(boneMap.Value))
				{
					Transform t = transforms.FirstOrDefault(trans => trans.name == boneMap.Value);
					if (t != null)
						CustomBones[(int)boneMap.Key] = t;
				}
			}
		}

		public void UpdateBonesPositions(SkeletonPoseData data)
		{
			PoseData mpPoseData = OvrToMetaPersonPose(data);

			Vector3 transformPosition = transform.position;
			Quaternion transformRotation = transform.rotation;
			Matrix4x4 transformMat = transform.localToWorldMatrix;
			if (IsInitialized)
			{
				for (var i = 0; i < _bones.Count; ++i)
				{
					var boneTransform = _bones[i].Transform;
					if (boneTransform == null) continue;

					if (additionalBonesIds.Contains(_bones[i].Id))
						boneTransform.position = transformMat.MultiplyPoint(mpPoseData.positions[i]);

					if (_bones[i].Id == BoneId.Body_Hips)
					{
						if (moveHips)
						{
							boneTransform.position = transformMat.MultiplyPoint(mpPoseData.positions[i]);
							boneTransform.rotation = transformRotation * mpPoseData.rotations[i];
						}
					}
					else
						boneTransform.rotation = transformRotation * mpPoseData.rotations[i];
				}
			}
			else
			{
				for (var i = 0; i < CustomBones.Count; ++i)
				{
					var boneTransform = CustomBones[i];
					if (boneTransform == null) continue;

					if (i == (int)BoneId.Body_Hips)
					{
						if (moveHips)
						{
							boneTransform.position = transformMat.MultiplyPoint(mpPoseData.positions[i]);
							boneTransform.rotation = transformRotation * mpPoseData.rotations[i];
						}
					}
					else
						boneTransform.rotation = transformRotation * mpPoseData.rotations[i];
				}
			}

			if (transformsPositioner != null)
				transformsPositioner.UpdatePositions(mpPoseData, transformPosition, transformRotation);

			foreach (var twoBoneIk in bonesIKs)
				twoBoneIk.ForceUpdate();

			if (alignFingersWithIK)
			{
				foreach (var bonePositioner in bonesPositioners)
				{
					bonePositioner.UpdateBonesPositions(mpPoseData, transformPosition, transformRotation);
				}
			}

			if (skeletonMapping.skeletonType == MetaPersonSkeletonType.Male)
			{
				//right hand adjustments
				{
					Transform rightLittleProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleProximal];
					Transform rightLittleIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleIntermediate];
					Transform rightLittleDistalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleDistal];
					Transform rightRingProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandRingProximal];
					Transform rightRingIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandRingIntermediate];
					Transform rightMiddleProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleProximal];
					Transform rightMiddleIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate];
					Transform rightMiddleDistalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleDistal];
					Transform rightIndexProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandIndexProximal];
					Transform rightThumbMetaTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal];
					Transform rightThumbProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandThumbProximal];

					rightLittleIntermediateTransform.Rotate(rightLittleIntermediateTransform.position - rightLittleProximalTransform.position, 10, Space.World);
					rightMiddleIntermediateTransform.Rotate(rightMiddleIntermediateTransform.position - rightMiddleProximalTransform.position, -5, Space.World);
					rightMiddleDistalTransform.Rotate(rightMiddleDistalTransform.position - rightMiddleIntermediateTransform.position, 10, Space.World);

					ShiftFingerIfTooCloseToNearby(rightRingProximalTransform, rightRingIntermediateTransform, rightMiddleIntermediateTransform, 17);
					ShiftFingerIfTooCloseToNearby(rightLittleProximalTransform, rightLittleIntermediateTransform, rightRingIntermediateTransform, 20);
					ShiftFingerIfTooCloseToNearby(rightThumbMetaTransform, rightThumbProximalTransform, rightIndexProximalTransform, 40);
				}

				//left hand adjustments
				{
					Transform leftLittleProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleProximal];
					Transform leftLittleIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate];
					Transform leftLittleDistalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleDistal];
					Transform leftRingProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandRingProximal];
					Transform leftRingIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandRingIntermediate];
					Transform leftMiddleProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleProximal];
					Transform leftMiddleIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate];
					Transform leftMiddleDistalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleDistal];
					Transform leftIndexProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandIndexProximal];
					Transform leftThumbMetaTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbMetacarpal];
					Transform leftThumbProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbProximal];

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
				{
					Transform rightLittleProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleProximal];
					Transform rightLittleIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandLittleIntermediate];
					Transform rightRingProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandRingProximal];
					Transform rightRingIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandRingIntermediate];
					Transform rightMiddleIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate];
					Transform rightMiddleDistalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleDistal];
					Transform rightIndexProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandIndexProximal];
					Transform rightThumbMetaTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal];
					Transform rightThumbProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_RightHandThumbProximal];

					rightLittleIntermediateTransform.Rotate(rightLittleIntermediateTransform.position - rightLittleProximalTransform.position, 10, Space.World);
					rightMiddleDistalTransform.Rotate(rightMiddleDistalTransform.position - rightMiddleIntermediateTransform.position, 10, Space.World);
					rightRingIntermediateTransform.Rotate(rightRingIntermediateTransform.position - rightRingProximalTransform.position, 10, Space.World);

					ShiftFingerIfTooCloseToNearby(rightThumbMetaTransform, rightThumbProximalTransform, rightIndexProximalTransform, 40);
				}

				//left hand adjustments
				{
					Transform leftLittleProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleProximal];
					Transform leftLittleIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate];
					Transform leftRingProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandRingProximal];
					Transform leftRingIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandRingIntermediate];
					Transform leftMiddleIntermediateTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate];
					Transform leftMiddleDistalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleDistal];
					Transform leftIndexProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandIndexProximal];
					Transform leftThumbMetaTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbMetacarpal];
					Transform leftThumbProximalTransform = CustomBones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbProximal];

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

		protected override void InitializeBones()
		{
			if (!additionalBones)
			{
				additionalBones = new GameObject("Bones");
				additionalBones.transform.SetParent(transform, false);
				additionalBones.transform.localPosition = Vector3.zero;
				additionalBones.transform.localRotation = Quaternion.identity;
			}

			if (_bones == null || _bones.Count != _skeleton.NumBones)
			{
				_bones = new List<OVRBone>(new OVRBone[_skeleton.NumBones]);
				Bones = _bones.AsReadOnly();
			}

			for (int i = 0; i < _bones.Count; ++i)
			{
				OVRBone bone = _bones[i] ?? (_bones[i] = new OVRBone());
				bone.Id = (OVRSkeleton.BoneId)_skeleton.Bones[i].Id;
				bone.ParentBoneIndex = _skeleton.Bones[i].ParentBoneIndex;

				if (bone.Transform == null)
				{
					bone.Transform = GetBoneTransform(bone.Id);
					if (bone.Transform == null)
					{
						bone.Transform = new GameObject(BoneLabelFromBoneId(_skeletonType, bone.Id)).transform;
						bone.Transform.name = BoneLabelFromBoneId(_skeletonType, bone.Id);
						bone.Transform.SetParent(additionalBones.transform, false);
						additionalBonesIds.Add(bone.Id);
					}
				}

				//var pose = _skeleton.Bones[i].Pose;
				//bone.Transform.localPosition = pose.Position.FromFlippedZVector3f();
				//bone.Transform.localRotation = pose.Orientation.FromFlippedZQuatf();
			}
		}

		protected void ForceUpdateMetaPersonSkeleton()
		{
			if (!IsInitialized)
				return;

			if (skeletonDataProvider == null)
			{
				skeletonDataProvider = FindSkeletonDataProvider();
				if (skeletonDataProvider == null)
					return;
			}

			var data = skeletonDataProvider.GetSkeletonPoseData();
			if (!data.IsDataValid)
			{
				return;
			}

			UpdateBonesPositions(data);
		}

		private void RotateRightLittleFinger()
		{
			Transform rightLittleProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandLittleProximal].Transform;
			Transform rightLittleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandLittleIntermediate].Transform;
			rightLittleProximalTransform.Rotate(rightLittleIntermediateTransform.position - rightLittleProximalTransform.position, 10, Space.World);
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

		private void ShiftFingerIfTooCloseToNearbyTemp(Transform proximal, Transform intermediate, Transform nearby, float maxAngle)
		{
			Vector3 proximalToNearby = nearby.position - proximal.position;
			Vector3 proximalToIntermediate = intermediate.position - proximal.position;

			float angle = Vector3.Angle(proximalToNearby, proximalToIntermediate);
		}


		private IOVRSkeletonDataProvider FindSkeletonDataProvider()
		{
			var dataProviders = gameObject.GetComponentsInParent<IOVRSkeletonDataProvider>();
			foreach (var dataProvider in dataProviders)
			{
				if (dataProvider.GetSkeletonType() == _skeletonType)
				{
					return dataProvider;
				}
			}

			return null;
		}

		private Transform FindChildByName(string transformName)
		{
			var transforms = GetComponentsInChildren<Transform>();
			return transforms.FirstOrDefault(t => t.name == transformName);
		}

		private PoseData OvrToMetaPersonPose(SkeletonPoseData ovrPoseData)
		{
			PoseData mpPoseData = new PoseData();
			mpPoseData.positions = new Vector3[ovrPoseData.BoneTranslations.Length];
			mpPoseData.rotations = new Quaternion[ovrPoseData.BoneRotations.Length];
			for (int i = 0; i < ovrPoseData.BoneTranslations.Length; i++)
			{
				mpPoseData.positions[i] = ovrPoseData.BoneTranslations[i].FromFlippedZVector3f();
				mpPoseData.rotations[i] = skeletonMapping.OVRToMPRotation((OVRPlugin.BoneId)i, ovrPoseData.BoneRotations[i].FromFlippedZQuatf());
			}
			return mpPoseData;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(OVRMetaPersonSkeleton))]
	public class OVRMetaPersonControllerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.Space(10);

			var skeleton = (OVRMetaPersonSkeleton)target;
			if (GUILayout.Button("Auto Map Bones"))
			{
				skeleton.MapBones();
			}

			EditorGUILayout.LabelField("Bones", EditorStyles.boldLabel);

			var start = skeleton.GetCurrentStartBoneId();
			var end = skeleton.GetCurrentEndBoneId();
			if (skeleton.IsValidBone(start) && skeleton.IsValidBone(end) && skeleton.CustomBones != null)
			{
				for (var i = (int)start; i < (int)end; ++i)
				{
					var boneName = OVRSkeleton.BoneLabelFromBoneId(skeleton.GetSkeletonType(), (OVRSkeleton.BoneId)i);
					EditorGUI.BeginChangeCheck();
					var val =
						EditorGUILayout.ObjectField(boneName, skeleton.CustomBones[i], typeof(Transform), true);
					if (EditorGUI.EndChangeCheck())
					{
						skeleton.CustomBones[i] = (Transform)val;
						EditorUtility.SetDirty(skeleton);
					}
				}
			}
		}
	}
#endif
}
