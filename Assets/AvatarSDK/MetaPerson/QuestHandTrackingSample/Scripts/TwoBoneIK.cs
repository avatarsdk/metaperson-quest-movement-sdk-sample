/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, November 2023
*/

using UnityEngine;

namespace AvatarSDK.MetaPerson.Oculus
{
	public enum UpwardsDirection
	{
		NORMAL,
		UP,
		POLE
	}

	public class TwoBoneIK : MonoBehaviour
	{
		public Transform upper;
		public Transform lower;
		public Transform end;

		public Transform target;
		public Transform pole;

		public bool leftHand = false;

		public UpwardsDirection upwardsDirection = UpwardsDirection.NORMAL;

		public bool debug = false;

		private void LateUpdate()
		{
			SolveTwoBoneIK();
		}

		public void ForceUpdate()
		{
			SolveTwoBoneIK();
		}

		private float CosAngle(float a, float b, float c)
		{
			var angle = Mathf.Clamp((a * a + b * b - c * c) / (2.0f * a * b), -1.0f, 1.0f);
			return Mathf.Acos(angle) * Mathf.Rad2Deg;
		}

		private void SolveTwoBoneIK()
		{
			Vector3 targetPosition = target.position;

			float a = lower.localPosition.magnitude;
			float b = end.localPosition.magnitude;
			float c = Vector3.Distance(upper.position, targetPosition);
			Vector3 n = Vector3.Cross(targetPosition - upper.position, pole.position - upper.position);
			if (leftHand)
				n *= -1.0f;

			Vector3 upRotated = Quaternion.LookRotation(targetPosition - upper.position) * Vector3.up;
			float angle = Vector3.SignedAngle(upRotated, n, targetPosition - upper.position);
			if (leftHand)
				angle = -Mathf.Clamp(angle, 0f, 90.0f);
			else
				angle = Mathf.Clamp(angle, -90f, 0.0f);

			Vector3 nClamped = Quaternion.AngleAxis(angle, Vector3.Cross(n, upRotated)) * upRotated;
			if (leftHand)
				nClamped *= -1.0f;
			
			Vector3 upwards = leftHand ? -nClamped : nClamped;
			if (upwardsDirection == UpwardsDirection.UP)
				upwards = Vector3.up;
			if (upwardsDirection == UpwardsDirection.POLE)
				upwards = -pole.forward;

			upper.rotation = Quaternion.LookRotation(targetPosition - upper.position, upwards);
			upper.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -nClamped) * upper.rotation;
			upper.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, lower.localPosition));

			lower.rotation = Quaternion.LookRotation(targetPosition - lower.position, upwards);
			lower.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, end.localPosition));

			end.rotation = target.rotation;

			if (debug)
			{
				Debug.LogFormat("Angle: {0}", angle);
				Vector3 centerPoint = (target.position + pole.position + upper.position) / 3.0f;
				Debug.DrawLine(centerPoint, centerPoint + n.normalized * 0.25f, Color.red);
				Debug.DrawLine(centerPoint, centerPoint + upRotated * 0.25f, Color.green);
				Debug.DrawLine(centerPoint, centerPoint + nClamped.normalized * 0.25f, Color.blue);
			}
		}
	}
}