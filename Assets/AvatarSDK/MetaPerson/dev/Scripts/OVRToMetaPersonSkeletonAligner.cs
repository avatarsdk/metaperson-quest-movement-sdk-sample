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
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AvatarSDK.MetaPerson.Oculus.Dev
{
	public class OVRToMetaPersonSkeletonAligner : MonoBehaviour
	{
		public GameObject ovrReferenceModel;
		public GameObject metaPersonReferenceModel;

		public OVRToMetaPersonArmAligner leftArmAligner;
		public OVRToMetaPersonArmAligner rightArmAligner;

		public void AlignOVRWithMetaPersonArms()
		{
			leftArmAligner.Align();
			rightArmAligner.Align();
		}

		public void ResetOVRBonesPositions()
		{
			GameObject targetModel = leftArmAligner.ovrModel;
			var targetTransforms = targetModel.GetComponentsInChildren<Transform>();
			var referenceTransforms = ovrReferenceModel.GetComponentsInChildren<Transform>();
			foreach(Transform t in targetTransforms)
			{
				var referenceTransform = referenceTransforms.FirstOrDefault(rt => rt.name == t.name);
				if (referenceTransform != null)
				{
					t.rotation = referenceTransform.rotation;
					t.position = referenceTransform.position;
				}
			}
		}

		public void  ResetMetaPersonBonesPositions()
		{
			GameObject targetModel = leftArmAligner.metaPersonModel;
			var targetTransforms = targetModel.GetComponentsInChildren<Transform>();
			var referenceTransforms = metaPersonReferenceModel.GetComponentsInChildren<Transform>();
			foreach (Transform t in targetTransforms)
			{
				var referenceTransform = referenceTransforms.FirstOrDefault(rt => rt.name == t.name);
				if (referenceTransform != null)
				{
					t.rotation = referenceTransform.rotation;
					t.position = referenceTransform.position;
				}
			}
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(OVRToMetaPersonSkeletonAligner))]
	public class OVRToMetaPersonSkeletonAlignerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.Space(10);

			var skeletonAligner = (OVRToMetaPersonSkeletonAligner)target;
			if (GUILayout.Button("Align OVR With MetaPerson Skeleton"))
			{
				skeletonAligner.AlignOVRWithMetaPersonArms();
			}
			if (GUILayout.Button("Reset OVR Bones Positions"))
			{
				skeletonAligner.ResetOVRBonesPositions();
			}
			if (GUILayout.Button("Reset MetaPerson Bones Positions"))
			{
				skeletonAligner.ResetMetaPersonBonesPositions();
			}
		}
	}
#endif
}
