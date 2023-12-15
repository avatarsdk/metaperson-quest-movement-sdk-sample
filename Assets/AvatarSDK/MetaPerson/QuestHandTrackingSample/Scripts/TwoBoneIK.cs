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
	public class TwoBoneIK : MonoBehaviour
	{
		public Transform upper;
		public Transform lower;
		public Transform end;

		public Transform target;
		public Transform pole;

		/*private void LateUpdate()
		{
			SolveTwoBoneIK();
		}*/

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

			Vector3 upperForward = upper.rotation * Vector3.forward;
			upper.rotation = Quaternion.LookRotation(targetPosition - upper.position, -upperForward/* -pole.forward*/);
			upper.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, lower.localPosition));
			upper.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -n) * upper.rotation;

			Vector3 lowerForward = lower.rotation * Vector3.forward;
			lower.rotation = Quaternion.LookRotation(targetPosition - lower.position, -lowerForward/* -pole.forward*/);
			lower.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, end.localPosition));

			end.rotation = target.rotation;
		}
	}
}