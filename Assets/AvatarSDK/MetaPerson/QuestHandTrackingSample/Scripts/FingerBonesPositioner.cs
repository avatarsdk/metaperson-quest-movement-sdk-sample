/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, December 2023
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRSkeleton;

namespace AvatarSDK.MetaPerson.Oculus
{
	public class FingerBonesPositioner : MonoBehaviour, IBonesPositioner
	{
		public BoneTarget proximal;
		public BoneTarget middle;
		public BoneTarget distal;

		private Vector3 proximalToMiddle;
		private Vector3 middleToDistal;

		private void Start()
		{
			proximalToMiddle = middle.target.position - proximal.target.position;
			middleToDistal = distal.target.position - middle.target.position;
		}

		public void UpdateBonesPositions(SkeletonPoseData data, Vector3 modelPosition, Quaternion modelRotation)
		{
			Matrix4x4 toModelMat = Matrix4x4.TRS(modelPosition, modelRotation, Vector3.one);
			BonePosition proximalPosition = new BonePosition()
			{
				position = toModelMat.MultiplyPoint(data.BoneTranslations[(int)proximal.boneId].FromFlippedZVector3f()),
				rotation = modelRotation * data.BoneRotations[(int)proximal.boneId].FromFlippedZQuatf()
			};
			BonePosition middlePosition = new BonePosition()
			{
				position = toModelMat.MultiplyPoint(data.BoneTranslations[(int)middle.boneId].FromFlippedZVector3f()),
				rotation = modelRotation * data.BoneRotations[(int)middle.boneId].FromFlippedZQuatf()
			};
			BonePosition distalPosition = new BonePosition()
			{
				position = toModelMat.MultiplyPoint(data.BoneTranslations[(int)distal.boneId].FromFlippedZVector3f()),
				rotation = modelRotation * data.BoneRotations[(int)distal.boneId].FromFlippedZQuatf()
			};

			SolveTwoBoneIK(proximalPosition, middlePosition, distalPosition);
		}

		private void SolveTwoBoneIK(BonePosition proximalPosition, BonePosition middlePosition, BonePosition distalPosition)
		{
			Vector3 targetProximalToMiddle = middlePosition.position - proximalPosition.position;
			Vector3 targetMiddlePosition = proximal.target.position + targetProximalToMiddle * (proximalToMiddle.magnitude / targetProximalToMiddle.magnitude);

			Vector3 targetMIddleToDistal = distalPosition.position - middlePosition.position;
			Vector3 targetDistalPosition = targetMiddlePosition + (targetMIddleToDistal) * (middleToDistal.magnitude / targetMIddleToDistal.magnitude);
			
			float a = middle.target.localPosition.magnitude;
			float b = distal.target.localPosition.magnitude;
			float c = Vector3.Distance(proximal.target.position, targetDistalPosition);
			Vector3 n = Vector3.Cross(targetDistalPosition - proximal.target.position, targetMiddlePosition - proximal.target.position);

			Vector3 proximalUp = proximalPosition.rotation * Vector3.up;
			proximal.target.rotation = Quaternion.LookRotation(targetDistalPosition - proximal.target.position, proximalUp);
			proximal.target.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, middle.target.localPosition));
			proximal.target.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -n) * proximal.target.rotation;

			Vector3 middlelUp = middlePosition.rotation * Vector3.up;
			middle.target.rotation = Quaternion.LookRotation(targetDistalPosition - middle.target.position, middlelUp);
			middle.target.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, distal.target.localPosition));

			distal.target.rotation = distalPosition.rotation;
		}

		private float CosAngle(float a, float b, float c)
		{
			var angle = Mathf.Clamp((a * a + b * b - c * c) / (2.0f * a * b), -1.0f, 1.0f);
			return Mathf.Acos(angle) * Mathf.Rad2Deg;
		}
	}
}
