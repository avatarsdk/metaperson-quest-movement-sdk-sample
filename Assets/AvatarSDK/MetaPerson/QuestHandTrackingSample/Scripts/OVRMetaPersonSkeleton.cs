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

		public MPBindPose bindPose;

		public OVRTransformsPositioner transformsPositioner;

		private IOVRSkeletonDataProvider skeletonDataProvider;

		private GameObject additionalBones;
		private HashSet<BoneId> additionalBonesIds = new HashSet<BoneId>();

		private ForeArmTwistAdjustment leftHandTwistAdjustment;
		private ForeArmTwistAdjustment rightHandTwistAdjustment;

		private List<TwoBoneIK> bonesIKs = new List<TwoBoneIK>();

		protected override void Start()
		{
			ApplyBindPose();
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
		}

		protected override void Update()
		{
			UpdateMetaPersonSkeleton();

			if (leftHandTwistAdjustment != null)
				leftHandTwistAdjustment.Update(twist1Coeff, twist2Coeff);
			if (rightHandTwistAdjustment != null)
				rightHandTwistAdjustment.Update(twist1Coeff, twist2Coeff);
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

		public void UpdateNotInitializedSkeleton(SkeletonPoseData data)
		{
			Vector3 transformPosition = transform.position;
			Quaternion transformRotation = transform.rotation;
			Matrix4x4 transformMat = transform.localToWorldMatrix;
			for (var i = 0; i < CustomBones.Count; ++i)
			{
				var boneTransform = CustomBones[i];
				if (boneTransform == null) continue;

				if (i == (int)BoneId.Body_Hips)
				{
					if (moveHips)
					{
						boneTransform.position = transformMat.MultiplyPoint(data.BoneTranslations[i].FromFlippedZVector3f());
						boneTransform.rotation = transformRotation * data.BoneRotations[i].FromFlippedZQuatf();
					}
				}
				else
					boneTransform.rotation = transformRotation * data.BoneRotations[i].FromFlippedZQuatf();
			}


			if (transformsPositioner != null)
				transformsPositioner.UpdatePositions(data, transformPosition, transformRotation);

			foreach (var twoBoneIk in bonesIKs)
				twoBoneIk.ForceUpdate();
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

		protected void UpdateMetaPersonSkeleton()
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

			/*if (_updateRootPose)
			{
				transform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
				transform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
			}

			if (_updateRootScale)
			{
				transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);
			}*/

			Vector3 transformPosition = transform.position;
			Quaternion transformRotation = transform.rotation;
			Matrix4x4 transformMat = transform.localToWorldMatrix;
			for (var i = 0; i < _bones.Count; ++i)
			{
				var boneTransform = _bones[i].Transform;
				if (boneTransform == null) continue;

				if (additionalBonesIds.Contains(_bones[i].Id))
					boneTransform.position = transformMat.MultiplyPoint(data.BoneTranslations[i].FromFlippedZVector3f());

				if (_bones[i].Id == BoneId.Body_Hips)
				{
					if (moveHips)
					{
						boneTransform.position = transformMat.MultiplyPoint(data.BoneTranslations[i].FromFlippedZVector3f());
						boneTransform.rotation = transformRotation * data.BoneRotations[i].FromFlippedZQuatf();
					}
				}
				else
					boneTransform.rotation = transformRotation * data.BoneRotations[i].FromFlippedZQuatf();
			}

			if (transformsPositioner != null)
				transformsPositioner.UpdatePositions(data, transformPosition, transformRotation);

			foreach (var twoBoneIk in bonesIKs)
				twoBoneIk.ForceUpdate();
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
			Debug.LogWarningFormat("[DBG] angle={0}", angle);
		}

		private void ApplyBindPose()
		{
			if (bindPose != null)
				bindPose.ApplyBindPose(gameObject);
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
