/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, January 2024
*/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Oculus.Editor
{
	[CustomEditor(typeof(OVRToMetaPersonSkeletonSync))]
	public class OVRToMetaPersonSkeletonSyncEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			var skeletonSync = (OVRToMetaPersonSkeletonSync)target;
			if (skeletonSync.sourceBonesModel != null)
			{
				if (GUILayout.Button("Take All Bones From Source Model"))
				{
					skeletonSync.TakeAllBonesToSyncFromSourceModel();
				}
			}
		}
	}
}
