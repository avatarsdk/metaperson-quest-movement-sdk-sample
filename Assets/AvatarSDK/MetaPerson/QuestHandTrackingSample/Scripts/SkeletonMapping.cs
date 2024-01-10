/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, December 2023
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Oculus
{
	[Serializable]
	public class BoneInitialRotations
	{
		public OVRBodyBoneId boneId;
		public Quaternion ovrInitialRotation;
		public Quaternion invOVRInitialRotation;
		public Quaternion mpInitialRotation;
		public Quaternion invMPInitialRotation;
	}

	[CreateAssetMenu(fileName = "SkeletonMapping", menuName = "Avatar SDK/Skeleton Mapping", order = 1)]
	public class SkeletonMapping : ScriptableObject
	{
		public MetaPersonSkeletonType skeletonType = MetaPersonSkeletonType.Male;

		public List<BoneInitialRotations> initialRotations = new List<BoneInitialRotations>();

		private Dictionary<OVRBodyBoneId, BoneInitialRotations> initialRotationsDict = new Dictionary<OVRBodyBoneId, BoneInitialRotations>();

		public void MapBonesRotations(Transform[] ovrTransforms, Transform[] mpTransforms)
		{
			initialRotations.Clear();
			for(int i=(int)OVRBodyBoneId.Body_Root; i<(int)OVRBodyBoneId.LowerBody_End; i++)
			{
				OVRBodyBoneId boneId = (OVRBodyBoneId)i;
				BoneInitialRotations boneRotations = new BoneInitialRotations();
				boneRotations.boneId = boneId;

				string ovrBoneName = BonesMapping.boneIdToOVRBones[boneId];
				if (!string.IsNullOrEmpty(ovrBoneName))
				{
					Transform ovrTransform = ovrTransforms.FirstOrDefault(t => t.name == ovrBoneName);
					if (ovrTransform != null)
					{
						boneRotations.ovrInitialRotation = ovrTransform.rotation;
						boneRotations.invOVRInitialRotation = Quaternion.Inverse(ovrTransform.rotation);
					}
				}

				string mpBoneName = BonesMapping.boneIdToMetaPersonBones[boneId];
				if (!string.IsNullOrEmpty(mpBoneName))
				{
					Transform mpTransform = mpTransforms.FirstOrDefault(t => t.name == mpBoneName);
					if (mpTransform != null)
					{
						boneRotations.mpInitialRotation = mpTransform.rotation;
						boneRotations.invMPInitialRotation = Quaternion.Inverse(mpTransform.rotation);
					}
				}

				initialRotations.Add(boneRotations);
			}
			BuildRotationsDict();
		}

		public Quaternion OVRToMPRotation(OVRBodyBoneId boneId, Quaternion ovrRotation)
		{
			if (initialRotationsDict.Count == 0)
				BuildRotationsDict();

			BoneInitialRotations initialRotations = initialRotationsDict[boneId];
			if (initialRotations != null)
				return ovrRotation * initialRotations.invOVRInitialRotation * initialRotations.mpInitialRotation;
			else
				return ovrRotation;
		}

		public Quaternion MPToOVRRotation(OVRBodyBoneId boneId, Quaternion mpRotation)
		{
			if (initialRotationsDict.Count == 0)
				BuildRotationsDict();

			BoneInitialRotations initialRotations = initialRotationsDict[boneId];
			if (initialRotations != null)
				return mpRotation * initialRotations.invMPInitialRotation * initialRotations.ovrInitialRotation;
			else
				return mpRotation;
		}

		private void BuildRotationsDict()
		{
			initialRotationsDict.Clear();
			foreach (BoneInitialRotations rotations in initialRotations)
				initialRotationsDict.Add(rotations.boneId, rotations);
		}
	}
}
