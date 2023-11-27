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
using System.IO;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Oculus.Dev
{
	[Serializable]
	public class PoseData
	{
		public Vector3[] positions;
		public Quaternion[] rotations;
	}

	public class PoseDataSerializer
	{
		public string PoseToJson(OVRSkeleton.SkeletonPoseData skeletonPoseData)
		{
			PoseData poseData = new PoseData();
			poseData.positions = new Vector3[skeletonPoseData.BoneTranslations.Length];
			poseData.rotations = new Quaternion[skeletonPoseData.BoneTranslations.Length];
			for(int i=0; i<skeletonPoseData.BoneTranslations.Length; i++)
			{
				poseData.positions[i] = skeletonPoseData.BoneTranslations[i].FromFlippedZVector3f();
				poseData.rotations[i] = skeletonPoseData.BoneRotations[i].FromFlippedZQuatf();
			}
			return JsonUtility.ToJson(poseData, true);
		}

		public void WritePoseToFile(string fileName, OVRSkeleton.SkeletonPoseData skeletonPoseData)
		{
			File.WriteAllText(fileName, PoseToJson(skeletonPoseData));
		}
	}
}
