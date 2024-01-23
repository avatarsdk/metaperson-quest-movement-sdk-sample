/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, January 2024
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Oculus
{
	public static class MetaPersonBodyTrackingSetupHelper
	{
		private static Dictionary<MetaPersonSkeletonType, string> skeletonMappingAssetPaths = new Dictionary<MetaPersonSkeletonType, string>()
		{
			{ MetaPersonSkeletonType.Male, "SkeletonsMapping/OVRToMetaPersonMaleSkeletonMapping" },
			{ MetaPersonSkeletonType.Female, "SkeletonsMapping/OVRToMetaPersonFemaleSkeletonMapping" }
		};

		private static Dictionary<MetaPersonSkeletonType, string> ikPrefabAssetsPaths = new Dictionary<MetaPersonSkeletonType, string>()
		{
			{ MetaPersonSkeletonType.Male, "Prefabs/IK_setup_male" },
			{ MetaPersonSkeletonType.Female, "Prefabs/IK_setup_female" }
		};

		public static void SetupMetaPersonForBodyTracking(GameObject metaPersonAvatar, MetaPersonSkeletonType skeletonType)
		{
			SetupMetaPersonForBodyTracking(metaPersonAvatar, skeletonType, null, null);
		}

		public static void SetupMetaPersonForBodyTracking(GameObject metaPersonAvatar, MetaPersonSkeletonType skeletonType, GameObject sourceBonesModel, List<OVRBodyBoneId> sourceBonesIds)
		{
			OVRBody ovrBody = metaPersonAvatar.GetComponent<OVRBody>();
			if (!ovrBody)
				metaPersonAvatar.AddComponent<OVRBody>();

			OVRToMetaPersonSkeletonSync skeletonSync = metaPersonAvatar.GetComponent<OVRToMetaPersonSkeletonSync>();
			if (!skeletonSync)
				skeletonSync = metaPersonAvatar.AddComponent<OVRToMetaPersonSkeletonSync>();

			var skeletonMapping = Resources.Load(skeletonMappingAssetPaths[skeletonType]) as SkeletonMapping;
			if (skeletonMapping != null)
				skeletonSync.skeletonMapping = skeletonMapping;
			else
				Debug.LogError("Skeleton mapping wasn't loaded");

			var ikPrefab = Resources.Load(ikPrefabAssetsPaths[skeletonType]);
			if (ikPrefab != null)
			{
				var ikPrefabInstance = Object.Instantiate(ikPrefab) as GameObject;
				ikPrefabInstance.name = "IK";
				ikPrefabInstance.transform.SetParent(metaPersonAvatar.transform);
				ConfigureHandIKs(metaPersonAvatar);
			}
			else
				Debug.LogError("IK prefab wasn't loaded");

			if (sourceBonesModel != null)
			{
				skeletonSync.syncBonesWithOtherModel = true;
				skeletonSync.sourceBonesModel = sourceBonesModel;
				skeletonSync.sourceBones = new List<BoneTransform>();
				if (sourceBonesIds != null)
				{
					Transform[] sourceBonesTransforms = sourceBonesModel.GetComponentsInChildren<Transform>();
					foreach(OVRBodyBoneId boneId in sourceBonesIds)
					{
						Transform srcTransform = sourceBonesTransforms.FirstOrDefault(t => t.name == BonesMapping.boneIdToOVRBones[boneId]);
						if (srcTransform != null)
							skeletonSync.sourceBones.Add(new BoneTransform() { boneId = boneId, transform = srcTransform });
					}
				}
			}
		}

		private static void ConfigureHandIKs(GameObject metaPerson)
		{
			Transform[] avatarTransforms = metaPerson.GetComponentsInChildren<Transform>();

			TwoBoneIK[] twoBoneIKs = metaPerson.GetComponentsInChildren<TwoBoneIK>();
			if (twoBoneIKs != null)
			{
				foreach(TwoBoneIK twoBoneIK in twoBoneIKs)
				{
					if (twoBoneIK.gameObject.name == "LeftHandIK")
					{
						twoBoneIK.upper = avatarTransforms.FirstOrDefault(t => t.name == "LeftArm");
						twoBoneIK.lower = avatarTransforms.FirstOrDefault(t => t.name == "LeftForeArm");
						twoBoneIK.end = avatarTransforms.FirstOrDefault(t => t.name == "LeftHand");
					}
					else if (twoBoneIK.gameObject.name == "RightHandIK")
					{
						twoBoneIK.upper = avatarTransforms.FirstOrDefault(t => t.name == "RightArm");
						twoBoneIK.lower = avatarTransforms.FirstOrDefault(t => t.name == "RightForeArm");
						twoBoneIK.end = avatarTransforms.FirstOrDefault(t => t.name == "RightHand");
					}
				}
			}
			else
				Debug.LogError("TwoBoneIK componenents wasn't found");
		}

	}
}
