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
using UnityEngine;
using static OVRSkeleton;

namespace AvatarSDK.MetaPerson.Oculus
{
	public class OVRTransformsPositioner : MonoBehaviour
	{
		public List<BoneTarget> boneTargets = new List<BoneTarget>();

		public void UpdatePositions(SkeletonPoseData data, Vector3 rootPosition, Quaternion rootRotation)
		{
			Matrix4x4 rootMat = Matrix4x4.TRS(rootPosition, rootRotation, Vector3.one);
			foreach (BoneTarget boneTarget in boneTargets)
			{
				Transform target = boneTarget.target;
				target.rotation = data.BoneRotations[(int)boneTarget.boneId].FromFlippedZQuatf();
				Vector3 offset = target.right * boneTarget.positionOffset.x + target.up * boneTarget.positionOffset.y + target.forward * boneTarget.positionOffset.z;
				target.position = rootMat.MultiplyPoint(data.BoneTranslations[(int)boneTarget.boneId].FromFlippedZVector3f() + offset);
				target.rotation = rootRotation * target.rotation;
			}
		}
	}
}
