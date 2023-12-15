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

namespace AvatarSDK.MetaPerson.Oculus
{
	public class ForeArmTwistAdjustment
	{
		private Transform hand;
		private Transform foreArm;
		private Transform foreArmTwist1;
		private Transform foreArmTwist2;

		private Quaternion twist1LocalRotation;
		private Quaternion twist2LocalRotation;

		private Vector3 handInitialRightProj;

		public ForeArmTwistAdjustment(Transform hand, Transform foreArm, Transform foreArmTwist1, Transform foreArmTwist2)
		{
			this.hand = hand;
			this.foreArm = foreArm;
			this.foreArmTwist1 = foreArmTwist1;
			this.foreArmTwist2 = foreArmTwist2;

			twist1LocalRotation = foreArmTwist1.localRotation;
			twist2LocalRotation = foreArmTwist2.localRotation;

			Vector3 armDirection = hand.position - foreArm.position;
			handInitialRightProj = foreArm.worldToLocalMatrix * Vector3.ProjectOnPlane(hand.right, armDirection);
		}

		public void Update(float twist1Coeff, float twist2Coeff)
		{
			Vector3 armDirection = hand.position - foreArm.position;

			Vector3 handRightProj = Vector3.ProjectOnPlane(hand.right, armDirection);
			Quaternion rot = Quaternion.FromToRotation(handInitialRightProj, handRightProj);

			float angle = Vector3.SignedAngle(handInitialRightProj, foreArm.worldToLocalMatrix * handRightProj, foreArm.worldToLocalMatrix * armDirection);
			if (angle > 180)
				angle = 360 - angle;
			if (angle < -180)
				angle = 360 + angle;

			Quaternion twist2Rot = Quaternion.AngleAxis(angle * twist2Coeff, armDirection);
			Quaternion twist1Rot = Quaternion.AngleAxis(angle * twist1Coeff, armDirection);

			foreArmTwist1.rotation = twist1Rot * twist1LocalRotation * foreArm.rotation;
			foreArmTwist2.rotation = twist2Rot * twist2LocalRotation * foreArm.rotation;
		}
	}
}
