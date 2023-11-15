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
		public MPBindPose bindPose;

		private IOVRSkeletonDataProvider skeletonDataProvider;

		private GameObject additionalBones;
		private HashSet<BoneId> additionalBonesIds = new HashSet<BoneId>();

		protected override void Start()
		{
			ApplyBindPose();
			base.Start();
		}

		protected override void Update()
		{
			UpdateMetaPersonSkeleton();
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

			for (var i = 0; i < _bones.Count; ++i)
			{
				var boneTransform = _bones[i].Transform;
				if (boneTransform == null) continue;

				if (_bones[i].Id == BoneId.Body_Hips || additionalBonesIds.Contains(_bones[i].Id))
					boneTransform.position = data.BoneTranslations[i].FromFlippedZVector3f();
				boneTransform.rotation = data.BoneRotations[i].FromFlippedZQuatf();
			}

			if (bindPose.skeletonType == MetaPersonSkeletonType.Male)
			{
				//right hand adjustments
				{
					Transform rightLittleProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandLittleProximal].Transform;
					Transform rightLittleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandLittleIntermediate].Transform;
					Transform rightLittleDistalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandLittleDistal].Transform;
					Transform rightRingProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandRingProximal].Transform;
					Transform rightRingIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandRingIntermediate].Transform;
					Transform rightMiddleProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleProximal].Transform;
					Transform rightMiddleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate].Transform;
					Transform rightMiddleDistalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleDistal].Transform;
					Transform rightIndexProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandIndexProximal].Transform;
					Transform rightThumbMetaTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal].Transform;
					Transform rightThumbProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandThumbProximal].Transform;

					rightLittleIntermediateTransform.Rotate(rightLittleIntermediateTransform.position - rightLittleProximalTransform.position, 10, Space.World);
					rightMiddleIntermediateTransform.Rotate(rightMiddleIntermediateTransform.position - rightMiddleProximalTransform.position, -5, Space.World);
					rightMiddleDistalTransform.Rotate(rightMiddleDistalTransform.position - rightMiddleIntermediateTransform.position, 10, Space.World);

					ShiftFingerIfTooCloseToNearby(rightRingProximalTransform, rightRingIntermediateTransform, rightMiddleIntermediateTransform, 17);
					ShiftFingerIfTooCloseToNearby(rightLittleProximalTransform, rightLittleIntermediateTransform, rightRingIntermediateTransform, 20);
					ShiftFingerIfTooCloseToNearby(rightThumbMetaTransform, rightThumbProximalTransform, rightIndexProximalTransform, 40);
				}

				//left hand adjustments
				{
					Transform leftLittleProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleProximal].Transform;
					Transform leftLittleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate].Transform;
					Transform leftLittleDistalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleDistal].Transform;
					Transform leftRingProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandRingProximal].Transform;
					Transform leftRingIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandRingIntermediate].Transform;
					Transform leftMiddleProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleProximal].Transform;
					Transform leftMiddleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate].Transform;
					Transform leftMiddleDistalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleDistal].Transform;
					Transform leftIndexProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandIndexProximal].Transform;
					Transform leftThumbMetaTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbMetacarpal].Transform;
					Transform leftThumbProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbProximal].Transform;

					leftLittleIntermediateTransform.Rotate(leftLittleIntermediateTransform.position - leftLittleProximalTransform.position, -10, Space.World);
					leftMiddleIntermediateTransform.Rotate(leftMiddleIntermediateTransform.position - leftMiddleProximalTransform.position, 5, Space.World);
					leftMiddleDistalTransform.Rotate(leftMiddleDistalTransform.position - leftMiddleIntermediateTransform.position, -10, Space.World);

					ShiftFingerIfTooCloseToNearby(leftRingProximalTransform, leftRingIntermediateTransform, leftMiddleIntermediateTransform, 17);
					ShiftFingerIfTooCloseToNearby(leftLittleProximalTransform, leftLittleIntermediateTransform, leftRingIntermediateTransform, 20);
					ShiftFingerIfTooCloseToNearby(leftThumbMetaTransform, leftThumbProximalTransform, leftIndexProximalTransform, 40);
				}
			}
			else if (bindPose.skeletonType == MetaPersonSkeletonType.Female)
			{
				//right hand adjustments
				{
					Transform rightLittleProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandLittleProximal].Transform;
					Transform rightLittleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandLittleIntermediate].Transform;
					Transform rightRingProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandRingProximal].Transform;
					Transform rightRingIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandRingIntermediate].Transform;
					Transform rightMiddleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleIntermediate].Transform;
					Transform rightMiddleDistalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandMiddleDistal].Transform;
					Transform rightIndexProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandIndexProximal].Transform;
					Transform rightThumbMetaTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandThumbMetacarpal].Transform;
					Transform rightThumbProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_RightHandThumbProximal].Transform;

					rightLittleIntermediateTransform.Rotate(rightLittleIntermediateTransform.position - rightLittleProximalTransform.position, 10, Space.World);
					rightMiddleDistalTransform.Rotate(rightMiddleDistalTransform.position - rightMiddleIntermediateTransform.position, 10, Space.World);
					rightRingIntermediateTransform.Rotate(rightRingIntermediateTransform.position - rightRingProximalTransform.position, 10, Space.World);

					ShiftFingerIfTooCloseToNearby(rightThumbMetaTransform, rightThumbProximalTransform, rightIndexProximalTransform, 40);
				}

				//left hand adjustments
				{
					Transform leftLittleProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleProximal].Transform;
					Transform leftLittleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandLittleIntermediate].Transform;
					Transform leftRingProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandRingProximal].Transform;
					Transform leftRingIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandRingIntermediate].Transform;
					Transform leftMiddleIntermediateTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleIntermediate].Transform;
					Transform leftMiddleDistalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandMiddleDistal].Transform;
					Transform leftIndexProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandIndexProximal].Transform;
					Transform leftThumbMetaTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbMetacarpal].Transform;
					Transform leftThumbProximalTransform = _bones[(int)OVRSkeleton.BoneId.Body_LeftHandThumbProximal].Transform;

					leftLittleIntermediateTransform.Rotate(leftLittleIntermediateTransform.position - leftLittleProximalTransform.position, -10, Space.World);
					leftMiddleDistalTransform.Rotate(leftMiddleDistalTransform.position - leftMiddleIntermediateTransform.position, -10, Space.World);
					leftRingIntermediateTransform.Rotate(leftRingIntermediateTransform.position - leftRingProximalTransform.position, -10, Space.World);

					ShiftFingerIfTooCloseToNearby(leftThumbMetaTransform, leftThumbProximalTransform, leftIndexProximalTransform, 40);
				}
			}
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
			if (skeleton.IsValidBone(start) && skeleton.IsValidBone(end))
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
