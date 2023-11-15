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
	public class MetaPersonBindPoseBuilder : MonoBehaviour
	{
		public MPBindPose bindPose;

		public GameObject ovrModel;

		public GameObject metaPersonModel;

		public List<BonePair> bonePairs = new List<BonePair>();

		public void AutoMapBonePairs()
		{
			var ovrTransforms = ovrModel.GetComponentsInChildren<Transform>();
			var metaPersonTransforms = metaPersonModel.GetComponentsInChildren<Transform>();

			bonePairs.Clear();
			foreach(var mpPair in BonesMapping.boneIdToMetaPersonBones)
			{
				OVRPlugin.BoneId boneId = mpPair.Key;
				string mpBoneName = mpPair.Value;
				string ovrBoneName = BonesMapping.boneIdToOVRBones[boneId];
				if (!string.IsNullOrEmpty(mpBoneName) && !string.IsNullOrEmpty(ovrBoneName))
				{
					Transform mpTransform = metaPersonTransforms.FirstOrDefault(t => t.name == mpBoneName);
					Transform ovrTransform = ovrTransforms.FirstOrDefault(t => t.name == ovrBoneName);
					bonePairs.Add(new BonePair() { mpBone = mpTransform, ovrBone = ovrTransform });
				}
			}
		}

		public void AlignRotationsAndSaveBindPose()
		{
			if (bindPose == null)
			{
				Debug.LogError("BindPose isn't provided!");
				return;
			}

			Dictionary<Transform, Vector3> positions = new Dictionary<Transform, Vector3>();
			foreach (Transform t in GetComponentsInChildren<Transform>())
				positions.Add(t, t.position);

			for (int i = 0; i < bonePairs.Count; i++)
			{
				bonePairs[i].mpBone.rotation = bonePairs[i].ovrBone.rotation;
			}

			foreach (var pair in positions)
				pair.Key.position = pair.Value;

			bindPose.transformPoses.Clear();
			foreach(Transform t in metaPersonModel.GetComponentsInChildren<Transform>())
				bindPose.AddTransformPose(t, t.worldToLocalMatrix);

#if UNITY_EDITOR
			if (bindPose != null)
			{
				EditorUtility.SetDirty(bindPose);
				AssetDatabase.SaveAssetIfDirty(bindPose);
			}
#endif
		}

		public void ApplyBindPose()
		{
			if (bindPose == null)
			{
				Debug.LogError("BindPose isn't provided!");
				return;
			}

			bindPose.ApplyBindPose(gameObject);
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(MetaPersonBindPoseBuilder))]
	public class MetaPersonBindPoseBuilderEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.Space(10);

			var bindPoseBuilder = (MetaPersonBindPoseBuilder)target;
			if (GUILayout.Button("Auto Map Bones"))
			{
				bindPoseBuilder.AutoMapBonePairs();
			}
			if (GUILayout.Button("Align Rotations And Save Bind Pose"))
			{
				bindPoseBuilder.AlignRotationsAndSaveBindPose();
			}
			if (GUILayout.Button("Apply Bind Pose"))
			{
				bindPoseBuilder.ApplyBindPose();
			}
		}
	}
#endif
}
