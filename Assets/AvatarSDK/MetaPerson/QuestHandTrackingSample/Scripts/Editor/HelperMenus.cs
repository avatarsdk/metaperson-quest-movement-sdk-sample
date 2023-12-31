/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2023
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Oculus.Editor
{
	internal static class HelperMenus
	{
		[MenuItem("GameObject/Movement/Setup Character for Body Tracking/Format: MetaPerson Male")]
		private static void SetupMetaPersonMaleForBodyTracking()
		{
			string skeletonPresetPath = "Assets/AvatarSDK/MetaPerson/QuestHandTrackingSample/Presets/OVRMetaPersonSkeletonMale.preset";
			string ikPrefabPath = "Assets/AvatarSDK/MetaPerson/QuestHandTrackingSample/Prefabs/IK_setup_male.prefab";
			SetupMetaPersonForBodyTracking(skeletonPresetPath, ikPrefabPath);
		}

		[MenuItem("GameObject/Movement/Setup Character for Body Tracking/Format: MetaPerson Female")]
		private static void SetupMetaPersonFemaleForBodyTracking()
		{
			string skeletonPresetPath = "Assets/AvatarSDK/MetaPerson/QuestHandTrackingSample/Presets/OVRMetaPersonSkeletonFemale.preset";
			string ikPrefabPath = "Assets/AvatarSDK/MetaPerson/QuestHandTrackingSample/Prefabs/IK_setup_female.prefab";
			SetupMetaPersonForBodyTracking(skeletonPresetPath, ikPrefabPath);
		}

		private static void SetupMetaPersonForBodyTracking(string skeletonPresetPath, string ikPrefabPath)
		{
			var activeGameObject = Selection.activeGameObject;

			OVRBody bodyComp = activeGameObject.GetComponent<OVRBody>();
			if (!bodyComp)
			{
				bodyComp = activeGameObject.AddComponent<OVRBody>();
				Undo.RegisterCreatedObjectUndo(bodyComp, "Add OVRBody component");
			}

			OVRMetaPersonSkeleton metaPersonSkeleton = activeGameObject.GetComponent<OVRMetaPersonSkeleton>();
			if (!metaPersonSkeleton)
			{
				metaPersonSkeleton = activeGameObject.AddComponent<OVRMetaPersonSkeleton>();
				Preset skeletonPreset = AssetDatabase.LoadAssetAtPath<Preset>(skeletonPresetPath);
				if (skeletonPreset != null)
					skeletonPreset.ApplyTo(metaPersonSkeleton);
				metaPersonSkeleton.MapBones();
				Undo.RegisterCreatedObjectUndo(metaPersonSkeleton, "Add OVRMetaPersonSkeleton component");
			}

			GameObject ikPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ikPrefabPath);
			GameObject ikSetupObject = Object.Instantiate(ikPrefab);
			ikSetupObject.name = "IK";
			ikSetupObject.transform.SetParent(activeGameObject.transform);
			metaPersonSkeleton.transformsPositioner = ikSetupObject.GetComponent<OVRTransformsPositioner>();
			Undo.RegisterCreatedObjectUndo(ikSetupObject, "Add IK prefab");

			Transform[] transforms = activeGameObject.GetComponentsInChildren<Transform>();
			ConfigureTwoBonesIKs(ikSetupObject.GetComponentsInChildren<TwoBoneIK>(), transforms);
			ConfigureFingerPositioners(ikSetupObject.GetComponentsInChildren<FingerBonesPositioner>(), transforms);
		}

		private static void ConfigureTwoBonesIKs(TwoBoneIK[] twoBoneIKs, Transform[] bones)
		{
			Dictionary<string, string[]> boneNamesForIK = new Dictionary<string, string[]>()
			{
				{ "LeftHandIK", new string[]{ "LeftArm", "LeftForeArm", "LeftHand" } },
				{ "RightHandIK", new string[]{ "RightArm", "RightForeArm", "RightHand" } },
				{ "RightIndexIK", new string[]{ "RightHandIndex1", "RightHandIndex2", "RightHandIndex3" } },
				{ "LeftIndexIK", new string[]{ "LeftHandIndex1", "LeftHandIndex2", "LeftHandIndex3" } },
				{ "RightMiddleIK", new string[]{ "RightHandMiddle1", "RightHandMiddle2", "RightHandMiddle3" } },
				{ "LeftMiddleIK", new string[]{ "LeftHandMiddle1", "LeftHandMiddle2", "LeftHandMiddle3" } },
				{ "RightRingIK", new string[]{ "RightHandRing1", "RightHandRing2", "RightHandRing3" } },
				{ "LeftRingIK", new string[]{ "LeftHandRing1", "LeftHandRing2", "LeftHandRing3" } },
				{ "RightPinkyIK", new string[]{ "RightHandPinky1", "RightHandPinky2", "RightHandPinky3" } },
				{ "LeftPinkyIK", new string[]{ "LeftHandPinky1", "LeftHandPinky2", "LeftHandPinky3" } },
				{ "RightThumbIK", new string[]{ "RightHandThumb1", "RightHandThumb2", "RightHandThumb3" } },
				{ "LeftThumbIK", new string[]{ "LeftHandThumb1", "LeftHandThumb2", "LeftHandThumb3" } },
			};

			foreach(TwoBoneIK boneIK in twoBoneIKs)
			{
				if (boneNamesForIK.ContainsKey(boneIK.name))
				{
					string[] boneNames = boneNamesForIK[boneIK.name];
					boneIK.upper = bones.FirstOrDefault(t => t.name == boneNames[0]);
					boneIK.lower = bones.FirstOrDefault(t => t.name == boneNames[1]);
					boneIK.end = bones.FirstOrDefault(t => t.name == boneNames[2]);
				}
				else
					Debug.LogWarningFormat("Unexpected TwoBoneIK: {0}", boneIK.name);
			}
		}

		private static void ConfigureFingerPositioners(FingerBonesPositioner[] fingerBonesPositioners, Transform[] bones)
		{
			foreach(FingerBonesPositioner positioner in fingerBonesPositioners)
			{
				positioner.proximal.target = bones.FirstOrDefault(t => t.name == BonesMapping.boneIdToMetaPersonBones[(OVRPlugin.BoneId)positioner.proximal.boneId]);
				positioner.middle.target = bones.FirstOrDefault(t => t.name == BonesMapping.boneIdToMetaPersonBones[(OVRPlugin.BoneId)positioner.middle.boneId]);
				positioner.distal.target = bones.FirstOrDefault(t => t.name == BonesMapping.boneIdToMetaPersonBones[(OVRPlugin.BoneId)positioner.distal.boneId]);
			}
		}
	}
}
