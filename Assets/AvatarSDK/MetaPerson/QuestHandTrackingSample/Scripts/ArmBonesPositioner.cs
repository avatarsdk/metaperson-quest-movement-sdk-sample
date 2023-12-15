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
	public class ArmBonesPositioner : MonoBehaviour, IBonesPositioner
	{
		public BoneTarget arm;
		public BoneTarget foreArm;
		public BoneTarget hand;

		public void UpdateBonesPositions(PoseData data, Vector3 modelPosition, Quaternion modelRotation)
		{
			Matrix4x4 toModelMat = Matrix4x4.TRS(modelPosition, modelRotation, Vector3.one);
			BonePosition armPosition = new BonePosition();
			armPosition.rotation = modelRotation * data.rotations[(int)arm.boneId];
			armPosition.position = toModelMat.MultiplyPoint(data.positions[(int)arm.boneId]);
			//armPosition.position += armPosition.rotation * arm.positionOffset;

			BonePosition foreArmPosition = new BonePosition();
			foreArmPosition.rotation = modelRotation * data.rotations[(int)foreArm.boneId];
			foreArmPosition.position = toModelMat.MultiplyPoint(data.positions[(int)foreArm.boneId]);
			//foreArmPosition.position += foreArmPosition.rotation * foreArm.positionOffset;

			BonePosition handPosition = new BonePosition();
			handPosition.rotation = modelRotation * data.rotations[(int)hand.boneId];
			handPosition.position = toModelMat.MultiplyPoint(data.positions[(int)hand.boneId]);
			handPosition.position += handPosition.rotation * hand.positionOffset;
			/*handPosition.rotation = data.BoneRotations[(int)hand.boneId].FromFlippedZQuatf();
			Vector3 offset = (handPosition.rotation * Vector3.right) * hand.positionOffset.x +
				(handPosition.rotation * Vector3.up) * hand.positionOffset.y + (handPosition.rotation * Vector3.forward) * hand.positionOffset.z;
			handPosition.position = toModelMat.MultiplyPoint(data.BoneTranslations[(int)hand.boneId].FromFlippedZVector3f() + offset);
			handPosition.rotation = modelRotation * handPosition.rotation;*/

			SolveTwoBoneIK(armPosition, foreArmPosition, handPosition);
		}

		private void SolveTwoBoneIK(BonePosition armPosition, BonePosition foreArmPosition, BonePosition handPosition)
		{
			Vector3 targetPosition = handPosition.position;

			float a = foreArm.target.localPosition.magnitude;
			float b = hand.target.localPosition.magnitude;
			float c = Vector3.Distance(arm.target.position, targetPosition);
			Vector3 n = Vector3.Cross(targetPosition - arm.target.position, foreArmPosition.position - arm.target.position);

			Vector3 armUp = armPosition.rotation * Vector3.up;
			arm.target.rotation = Quaternion.LookRotation(targetPosition - arm.target.position, armUp);
			arm.target.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, foreArm.target.localPosition));
			arm.target.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -n) * arm.target.rotation;

			Vector3 foreArmlUp = foreArmPosition.rotation * Vector3.up;
			foreArm.target.rotation = Quaternion.LookRotation(targetPosition - foreArm.target.position, foreArmlUp);
			foreArm.target.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, hand.target.localPosition));

			hand.target.rotation = hand.target.rotation;
		}

		private float CosAngle(float a, float b, float c)
		{
			var angle = Mathf.Clamp((a * a + b * b - c * c) / (2.0f * a * b), -1.0f, 1.0f);
			return Mathf.Acos(angle) * Mathf.Rad2Deg;
		}
	}
}
