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
	public enum HandType
	{
		LeftHand,
		RightHand
	}

	public class OVRToMetaPersonArmAligner : MonoBehaviour
	{
		public HandType handType;

		public GameObject ovrModel;
		public GameObject metaPersonModel;

		public BonePair upperArmPair;
		public BonePair lowerArmPair;
		public BonePair handPair;

		public BonePair indexProximalPair;
		public BonePair indexIntermediatePair;
		public BonePair indexDistalPair;
		public BonePair indexTipPair;

		public BonePair middleProximalPair;
		public BonePair middleIntermediatePair;
		public BonePair middleDistalPair;
		public BonePair middleTipPair;

		public BonePair ringProximalPair;
		public BonePair ringIntermediatePair;
		public BonePair ringDistalPair;
		public BonePair ringTipPair;

		public BonePair pinkyMetaPair;
		public BonePair pinkyProximalPair;
		public BonePair pinkyIntermediatePair;
		public BonePair pinkyDistalPair;
		public BonePair pinkyTipPair;

		public BonePair thumbMetaPair;
		public BonePair thumbProximalPair;
		public BonePair thumbDistalPair;
		public BonePair thumbTipPair;

		public void MapBones()
		{
			if (ovrModel == null || metaPersonModel == null)
				return;

			var ovrTransforms = ovrModel.GetComponentsInChildren<Transform>();
			var mpTransforms = metaPersonModel.GetComponentsInChildren<Transform>();

			upperArmPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftArmUpper]));
			upperArmPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftArmUpper]));

			lowerArmPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftArmLower]));
			lowerArmPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftArmLower]));

			handPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandWrist]));
			handPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandWrist]));

			//index
			indexProximalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandIndexProximal]));
			indexProximalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandIndexProximal]));

			indexIntermediatePair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandIndexIntermediate]));
			indexIntermediatePair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandIndexIntermediate]));

			indexDistalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandIndexDistal]));
			indexDistalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandIndexDistal]));

			indexTipPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandIndexTip]));
			indexTipPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandIndexTip]));

			//middle
			middleProximalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandMiddleProximal]));
			middleProximalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandMiddleProximal]));

			middleIntermediatePair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandMiddleIntermediate]));
			middleIntermediatePair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandMiddleIntermediate]));

			middleDistalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandMiddleDistal]));
			middleDistalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandMiddleDistal]));

			middleTipPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandMiddleTip]));
			middleTipPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandMiddleTip]));

			//ring
			ringProximalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandRingProximal]));
			ringProximalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandRingProximal]));

			ringIntermediatePair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandRingIntermediate]));
			ringIntermediatePair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandRingIntermediate]));

			ringDistalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandRingDistal]));
			ringDistalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandRingDistal]));

			ringTipPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandRingTip]));
			ringTipPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandRingTip]));

			//pinky
			pinkyMetaPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandLittleMetacarpal]));
			pinkyMetaPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandLittleMetacarpal]));

			pinkyProximalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandLittleProximal]));
			pinkyProximalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandLittleProximal]));

			pinkyIntermediatePair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandLittleIntermediate]));
			pinkyIntermediatePair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandLittleIntermediate]));

			pinkyDistalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandLittleDistal]));
			pinkyDistalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandLittleDistal]));

			pinkyTipPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandLittleTip]));
			pinkyTipPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandLittleTip]));

			//thumb
			thumbMetaPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandThumbMetacarpal]));
			thumbMetaPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandThumbMetacarpal]));

			thumbProximalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandThumbProximal]));
			thumbProximalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandThumbProximal]));

			thumbDistalPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandThumbDistal]));
			thumbDistalPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandThumbDistal]));

			thumbTipPair.ovrBone = GetByName(ovrTransforms, RenameByHandType(BonesMapping.boneIdToOVRBones[OVRPlugin.BoneId.Body_LeftHandThumbTip]));
			thumbTipPair.mpBone = GetByName(mpTransforms, RenameByHandType(BonesMapping.boneIdToMetaPersonBones[OVRPlugin.BoneId.Body_LeftHandThumbTip]));
		}

		public void Align()
		{
			upperArmPair.ovrBone.position = upperArmPair.mpBone.position;

			RotateParnetToAlignChild(upperArmPair, lowerArmPair);
			lowerArmPair.ovrBone.position = lowerArmPair.mpBone.position;

			RotateParnetToAlignChild(lowerArmPair, handPair);
			handPair.ovrBone.position = handPair.mpBone.position;

			RotateHand();

			AlignFinger(indexProximalPair, indexIntermediatePair, indexDistalPair, indexTipPair);
			AlignFinger(middleProximalPair, middleIntermediatePair, middleDistalPair, middleTipPair);
			AlignFinger(ringProximalPair, ringIntermediatePair, ringDistalPair, ringTipPair);

			//AlignPinkyMeta();
			AlignFinger(pinkyProximalPair, pinkyIntermediatePair, pinkyDistalPair, pinkyTipPair);

			//AlignThumbMeta();
			AlignFinger(thumbMetaPair, thumbProximalPair, thumbDistalPair, thumbTipPair);
		}

		private void RotateParnetToAlignChild(BonePair parentPair, BonePair childPair)
		{
			Vector3 targetDirection = childPair.ovrBone.position - parentPair.ovrBone.position;
			Vector3 srcDirection = childPair.mpBone.position - parentPair.mpBone.position;
			Quaternion rot = Quaternion.FromToRotation(targetDirection, srcDirection);
			parentPair.ovrBone.rotation = rot * parentPair.ovrBone.rotation;
		}

		private void RotateHand()
		{
			Vector3 targetDirection = pinkyTipPair.ovrBone.position - indexTipPair.ovrBone.position;
			Vector3 srcDirection = pinkyTipPair.mpBone.position - indexTipPair.mpBone.position;
			handPair.ovrBone.rotation = Quaternion.FromToRotation(targetDirection, srcDirection) * handPair.ovrBone.rotation;

			targetDirection = middleTipPair.ovrBone.position - handPair.ovrBone.position;
			srcDirection = middleTipPair.mpBone.position - handPair.mpBone.position;
			handPair.ovrBone.rotation = Quaternion.FromToRotation(targetDirection, srcDirection) * handPair.ovrBone.rotation;
		}

		private void AlignFinger(BonePair proximalPair, BonePair intermediatePair, BonePair distalPair, BonePair tipPair)
		{
			//proximalPair.targetBone.position = proximalPair.srcBone.position;
			RotateParnetToAlignChild(proximalPair, intermediatePair);

			//intermediatePair.targetBone.position = intermediatePair.srcBone.position;
			RotateParnetToAlignChild(intermediatePair, distalPair);

			//distalPair.targetBone.position = distalPair.srcBone.position;
			//RotateParnetToAlignChild(distalPair, tipPair);

			//tipPair.targetBone.position = tipPair.srcBone.position;
		}

		private void AlignPinkyMeta()
		{
			pinkyMetaPair.ovrBone.position = handPair.ovrBone.position;
			Vector3 targetDirection = pinkyProximalPair.ovrBone.position - pinkyMetaPair.ovrBone.position;
			Vector3 srcDirection = pinkyProximalPair.mpBone.position - handPair.mpBone.position;
			pinkyMetaPair.ovrBone.rotation = Quaternion.FromToRotation(targetDirection, srcDirection) * pinkyMetaPair.ovrBone.rotation;
		}

		private void AlignThumbMeta()
		{
			thumbMetaPair.ovrBone.position = handPair.ovrBone.position;
			Vector3 targetDirection = thumbProximalPair.ovrBone.position - thumbMetaPair.ovrBone.position;
			Vector3 srcDirection = thumbProximalPair.mpBone.position - handPair.mpBone.position;
			thumbMetaPair.ovrBone.rotation = Quaternion.FromToRotation(targetDirection, srcDirection) * thumbMetaPair.ovrBone.rotation;
		}

		private Transform GetByName(Transform[] transforms, string name)
		{
			return transforms.FirstOrDefault(t => t.name == name);
		}

		private string RenameByHandType(string boneName)
		{
			if (handType == HandType.LeftHand)
			{
				boneName = boneName.Replace("Right", "Left");
			}
			else
			{
				boneName = boneName.Replace("Left", "Right");
			}
			return boneName;
		}
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(OVRToMetaPersonArmAligner))]
	public class OVRToMetaPersonArmAlignerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.Space(10);

			var armAligner = (OVRToMetaPersonArmAligner)target;
			if (GUILayout.Button("Auto Map Bones"))
			{
				armAligner.MapBones();
			}
		}
	}
#endif
}
