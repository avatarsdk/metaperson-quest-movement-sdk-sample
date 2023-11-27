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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AvatarSDK.MetaPerson.Oculus
{
	public class TwoBoneIK : MonoBehaviour
	{
		public Transform upper;
		public Transform lower;
		public Transform end;

		public Transform target;
		public Transform pole; 

		public float upperRotationOffset;
		public float lowerRotationOffset;

		//TODO: remove
		public bool debug = false;
		public bool pinkyFix = false;
		public float pinkyFixValue = 0.02f;

		public bool moveTargetTowardsPole = false;
		[HideInInspector]
		public float minPoleAngle = 0;
		[HideInInspector]
		public float maxPoleAngle = 180;
		[HideInInspector]
		public float minAngleTagetOffset = 0;
		[HideInInspector]
		public float maxAngleTargetOffset = 0;

		public string testName = "";

		/*private void LateUpdate()
		{
			SolveTwoBoneIK();
		}*/

		public void CustomUpdate()
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
			if (pinkyFix)
			{
				Vector3 poleToUpper = upper.position - pole.position;
				Vector3 poleToTarget = targetPosition - pole.position;
				float poleToTargetDistance = poleToTarget.magnitude;
				float poleToUpperDistance = poleToUpper.magnitude;
				float upperToTargetDistance = (targetPosition - upper.position).magnitude;

				float currentAngle = CosAngle(poleToTargetDistance, poleToUpperDistance, upperToTargetDistance);
				targetPosition += target.right * (currentAngle - 90) * pinkyFixValue / 90.0f;

				if (debug)
					Debug.LogWarningFormat("CosAngle={0}", currentAngle);
			}
			else if (moveTargetTowardsPole)
			{
				//Debug.LogWarningFormat("moveTargetTowardsPole");
				Vector3 poleToUpper = upper.position - pole.position;
				Vector3 poleToTarget = targetPosition - pole.position;
				float poleToTargetDistance = poleToTarget.magnitude;
				float poleToUpperDistance = poleToUpper.magnitude;
				float upperToTargetDistance = (targetPosition - upper.position).magnitude;

				float currentPoleAngle = CosAngle(poleToTargetDistance, poleToUpperDistance, upperToTargetDistance);
				float offsetVal = 0.0f;
				if (currentPoleAngle >= minPoleAngle && currentPoleAngle <= maxPoleAngle)
				{
					offsetVal = Mathf.Lerp(Mathf.Min(minAngleTagetOffset, maxAngleTargetOffset), Mathf.Max(minAngleTagetOffset, maxAngleTargetOffset), 
						(currentPoleAngle - minPoleAngle) / (maxPoleAngle - minPoleAngle));
					targetPosition += target.right * offsetVal;
				}

				if (debug)
				{
					//Debug.LogWarningFormat("Pole Angle={0}, {1}, {2}, {3}, {4}", currentPoleAngle, minPoleAngle, maxPoleAngle, minAngleTagetOffset, maxAngleTargetOffset);
					//Debug.LogWarningFormat("Offset = {0}, right length = {1}", offsetVal, target.right.magnitude);
					Debug.LogWarningFormat("[DBG] {0}", testName);
				}
			}

			float a = lower.localPosition.magnitude;
			float b = end.localPosition.magnitude;
			float c = Vector3.Distance(upper.position, targetPosition);
			Vector3 n = Vector3.Cross(targetPosition - upper.position, pole.position - upper.position);

			upper.rotation = Quaternion.LookRotation(targetPosition - upper.position, Quaternion.AngleAxis(upperRotationOffset, lower.position - upper.position) * (n));
			upper.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, lower.localPosition));
			upper.rotation = Quaternion.AngleAxis(-CosAngle(a, c, b), -n) * upper.rotation;

			lower.rotation = Quaternion.LookRotation(targetPosition - lower.position, Quaternion.AngleAxis(lowerRotationOffset, end.position - lower.position) * (n));
			lower.rotation *= Quaternion.Inverse(Quaternion.FromToRotation(Vector3.forward, end.localPosition));

			end.rotation = target.rotation;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(TwoBoneIK))]
	public class TwoBoneIKEditor : Editor
	{
		private float minAngleLimit = 0;
		private float maxAngleLimit = 180;

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var twoBoneIK = (TwoBoneIK)target;
			if (twoBoneIK.moveTargetTowardsPole)
			{
				twoBoneIK.minPoleAngle = EditorGUILayout.FloatField("Min Pole Angle:", twoBoneIK.minPoleAngle);
				twoBoneIK.maxPoleAngle = EditorGUILayout.FloatField("Max Pole Angle:", twoBoneIK.maxPoleAngle);
				EditorGUILayout.MinMaxSlider(ref twoBoneIK.minPoleAngle, ref twoBoneIK.maxPoleAngle, minAngleLimit, maxAngleLimit);

				twoBoneIK.minAngleTagetOffset = EditorGUILayout.FloatField("Min Angle Target Offset:", twoBoneIK.minAngleTagetOffset);
				twoBoneIK.maxAngleTargetOffset = EditorGUILayout.FloatField("Max Angle Target Offset:", twoBoneIK.maxAngleTargetOffset);
			}
		}
	}
#endif
}