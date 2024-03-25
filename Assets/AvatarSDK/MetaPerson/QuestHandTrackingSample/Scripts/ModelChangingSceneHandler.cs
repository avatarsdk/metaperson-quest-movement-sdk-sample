/* Copyright (C) Itseez3D, Inc. - All Rights Reserved
* You may not use this file except in compliance with an authorized license
* Unauthorized copying of this file, via any medium is strictly prohibited
* Proprietary and confidential
* UNLESS REQUIRED BY APPLICABLE LAW OR AGREED BY ITSEEZ3D, INC. IN WRITING, SOFTWARE DISTRIBUTED UNDER THE LICENSE IS DISTRIBUTED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OR
* CONDITIONS OF ANY KIND, EITHER EXPRESS OR IMPLIED
* See the License for the specific language governing permissions and limitations under the License.
* Written by Itseez3D, Inc. <support@avatarsdk.com>, March 2024
*/

using AvatarSDK.MetaPerson.Loader;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AvatarSDK.MetaPerson.Oculus
{
	[Serializable]
	public class MetaPersonModelsConfig
	{
		public string modelUrl;

		public GameObject avatarObject;

		public GameObject mirroredAvatarObject;
	}

	public class ModelChangingSceneHandler : MonoBehaviour
	{
		public MetaPersonMaterialGenerator metaPersonMaterialGenerator;

		public MetaPersonSkeletonType skeletonType;

		public MetaPersonModelsConfig maleModelsConfig;

		public MetaPersonModelsConfig femaleModelsConfig;

		private void Start()
		{
			StartAvatarLoading();
		}

		private void StartAvatarLoading()
		{
			MetaPersonModelsConfig modelsConfig = skeletonType == MetaPersonSkeletonType.Male ? maleModelsConfig : femaleModelsConfig;

			modelsConfig.avatarObject.SetActive(true);
			modelsConfig.mirroredAvatarObject.SetActive(true);

			if (string.IsNullOrEmpty(modelsConfig.modelUrl))
				return;

			LoadAndReplaceAvatar(modelsConfig.modelUrl, modelsConfig.avatarObject);
			LoadAndReplaceAvatar(modelsConfig.modelUrl, modelsConfig.mirroredAvatarObject);
		}

		private async void LoadAndReplaceAvatar(string modelUrl, GameObject targetAvatar)
		{
			GameObject metaPersonLoaderObject = new GameObject("MetaPersonLoader");
			MetaPersonLoader metaPersonLoader = metaPersonLoaderObject.AddComponent<MetaPersonLoader>();
			metaPersonLoader.cacheModels = true;
			metaPersonLoader.materialGenerator = metaPersonMaterialGenerator;

			bool isAvatarLoaded = await metaPersonLoader.LoadModelAsync(modelUrl);
			if (isAvatarLoaded)
			{
				MetaPersonUtils.ReplaceAvatar(metaPersonLoader.avatarObject, targetAvatar);
			}
			else
				Debug.LogError("Unable to load avatar");
		}
	}
}
