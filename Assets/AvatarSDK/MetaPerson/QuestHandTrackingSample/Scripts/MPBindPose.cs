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

namespace AvatarSDK.MetaPerson.Oculus
{
	public enum MetaPersonSkeletonType
	{
		Male,
		Female
	}

	[Serializable]
	public class TransformPosePair
	{
		public string transformName;
		public Matrix4x4 pose;
	}

	[CreateAssetMenu(fileName = "MPBindPose", menuName = "Avatar SDK/Meta Person Bind Pose", order = 1)]
	public class MPBindPose : ScriptableObject
	{
		public MetaPersonSkeletonType skeletonType;

		public List<TransformPosePair> transformPoses = new List<TransformPosePair>();

		public void AddTransformPose(Transform t, Matrix4x4 m)
		{
			TransformPosePair transformPose = transformPoses.FirstOrDefault(p => p.transformName == t.name);
			if (transformPose != null)
			{
				transformPose.pose = m;
			}
			else
			{
				transformPose = new TransformPosePair()
				{
					transformName = t.name,
					pose = m
				};
				transformPoses.Add(transformPose);
			}
		}

		public Matrix4x4 GetPose(Transform t)
		{
			TransformPosePair transformPose = transformPoses.FirstOrDefault(p => p.transformName == t.name);
			if (transformPose != null)
				return transformPose.pose;
			else
				throw new Exception(string.Format("No pose for transform: {0}", t.name));

		}

		public void ApplyBindPose(GameObject gameObject)
		{
			SkinnedMeshRenderer[] skinnedMeshRenderers = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer meshRenderer in skinnedMeshRenderers)
			{
				Matrix4x4[] bindposes = meshRenderer.sharedMesh.bindposes;
				Transform[] bones = meshRenderer.bones;
				Mesh mesh = meshRenderer.sharedMesh;
				for (int i = 0; i < bones.Length; i++)
				{
					bindposes[i] = GetPose(bones[i]);
					Matrix4x4 m = Matrix4x4.Inverse(bindposes[i]);
					bones[i].position = m.GetPosition();
					bones[i].rotation = m.rotation;
				}
				mesh.bindposes = bindposes;
				meshRenderer.sharedMesh = mesh;
			}
		}
	}
}
