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
using UnityEngine;
using static OVRSkeleton;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AvatarSDK.MetaPerson.Oculus.Dev
{
	public class PoseDataApplier : MonoBehaviour
	{
		public TextAsset poseAsset;

		public OVRCustomSkeleton ovrCustomSkeleton;

		public void ApplyPose()
		{
			OVRMetaPersonSkeleton metaPersonSkeleton = ovrCustomSkeleton as OVRMetaPersonSkeleton;

			PoseData poseData = JsonUtility.FromJson<PoseData>(poseAsset.text);
			if (metaPersonSkeleton != null)
			{
				SkeletonPoseData skeletonPoseData = new SkeletonPoseData();
				skeletonPoseData.BoneRotations = new OVRPlugin.Quatf[poseData.rotations.Length];
				skeletonPoseData.BoneTranslations = new OVRPlugin.Vector3f[poseData.positions.Length];
				for (int i = 0; i < poseData.positions.Length; i++)
				{
					skeletonPoseData.BoneRotations[i] = poseData.rotations[i].ToFlippedZQuatf();
					skeletonPoseData.BoneTranslations[i] = poseData.positions[i].ToFlippedZVector3f();
				}

				metaPersonSkeleton.UpdateNotInitializedSkeleton(skeletonPoseData);
			}
			else
			{
				for (int i = 0; i < poseData.positions.Length; i++)
				{
					Transform t = ovrCustomSkeleton.CustomBones[i];
					if (t != null)
					{
						t.position = poseData.positions[i];
						t.rotation = poseData.rotations[i];
					}
				}
			}
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(PoseDataApplier))]
	public class PoseDataApplierEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.Space(10);

			var poseDataApplier = (PoseDataApplier)target;
			if (GUILayout.Button("Apply Pose"))
			{
				poseDataApplier.ApplyPose();
			}
		}
	}
#endif
}
