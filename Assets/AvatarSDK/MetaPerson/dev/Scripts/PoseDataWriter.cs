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
using System.IO;
using UnityEngine;
using static OVRSkeleton;

namespace AvatarSDK.MetaPerson.Oculus.Dev
{
	public class PoseDataWriter : MonoBehaviour
	{
		public GameObject skeletonProviderObject;

		private PoseDataSerializer poseSerializer = new PoseDataSerializer();

		private IOVRSkeletonDataProvider skeletonDataProvider = null;

		private string posesDirPath = string.Empty;

		private int poseNumber = 0;

		private float savingDelayInSec = 5.0f;

		private DateTime lastSaveTime = DateTime.MinValue;

		private void Start()
		{
			skeletonDataProvider = FindSkeletonDataProvider();

			posesDirPath = Path.Combine(Application.persistentDataPath, "poses");
			if (Directory.Exists(posesDirPath))
				Directory.Delete(posesDirPath, true);
			Directory.CreateDirectory(posesDirPath);

			poseNumber = 0;
		}

		private void Update()
		{
			if ((DateTime.Now - lastSaveTime).TotalSeconds > savingDelayInSec)
			{
				lastSaveTime = DateTime.Now;

				if (skeletonDataProvider == null)
				{
					Debug.LogError("IOVRSkeletonDataProvider was not found!");
					return;
				}

				var poseData = skeletonDataProvider.GetSkeletonPoseData();
				if (poseData.IsDataValid)
					WritePose(poseData);
			}
		}
		
		private void WritePose(OVRSkeleton.SkeletonPoseData skeletonPoseData)
		{
			string fileName = Path.Combine(posesDirPath, string.Format("{0}.json", poseNumber++));
			poseSerializer.WritePoseToFile(fileName, skeletonPoseData);
		}

		private IOVRSkeletonDataProvider FindSkeletonDataProvider()
		{
			var dataProviders = skeletonProviderObject.GetComponentsInParent<IOVRSkeletonDataProvider>();
			foreach (var dataProvider in dataProviders)
			{
				if (dataProvider.GetSkeletonType() == SkeletonType.Body)
				{
					return dataProvider;
				}
			}

			return null;
		}
	}
}
