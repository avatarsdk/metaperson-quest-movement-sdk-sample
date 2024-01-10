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
using UnityEngine;
using static OVRSkeleton;

namespace AvatarSDK.MetaPerson.Oculus
{
	[Serializable]
	public class BoneTarget
	{
		public OVRBodyBoneId boneId;

		public Transform target;

		public Vector3 positionOffset;
	}

	public class BonePosition
	{
		public Vector3 position;
		public Quaternion rotation;
	}

	public interface IBonesPositioner
	{
		void UpdateBonesPositions(PoseData data, Vector3 modelPosition, Quaternion modelRotation);
	}
}
