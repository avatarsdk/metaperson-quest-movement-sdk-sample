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
	public class BonesMapping
	{
		public static readonly string leftForeArmTwist1Name = "LeftForeArm1";
		public static readonly string leftForeArmTwist2Name = "LeftForeArm2";
		public static readonly string leftHandName = "LeftHand";
		public static readonly string leftForeArmName = "LeftForeArm";

		public static readonly string rightForeArmTwist1Name = "RightForeArm1";
		public static readonly string rightForeArmTwist2Name = "RightForeArm2";
		public static readonly string rightHandName = "RightHand";
		public static readonly string rightForeArmName = "RightForeArm";

		public static readonly Dictionary<OVRBodyBoneId, string> boneIdToMetaPersonBones = new Dictionary<OVRBodyBoneId, string>()
		{
			{ OVRBodyBoneId.Body_Root, "" },
			{ OVRBodyBoneId.Body_Hips, "Hips" },
			{ OVRBodyBoneId.Body_SpineLower, "" },
			{ OVRBodyBoneId.Body_SpineMiddle, "Spine" },
			{ OVRBodyBoneId.Body_SpineUpper, "Spine1" },
			{ OVRBodyBoneId.Body_Chest, "Spine2" },
			{ OVRBodyBoneId.Body_Neck, "Neck2" },
			{ OVRBodyBoneId.Body_Head, "Head" },
			{ OVRBodyBoneId.Body_LeftShoulder, "LeftShoulder" },
			{ OVRBodyBoneId.Body_LeftScapula, "" },
			{ OVRBodyBoneId.Body_LeftArmUpper, "LeftArm" },
			{ OVRBodyBoneId.Body_LeftArmLower, "LeftForeArm" },
			{ OVRBodyBoneId.Body_LeftHandWristTwist, "" },
			{ OVRBodyBoneId.Body_RightShoulder, "RightShoulder" },
			{ OVRBodyBoneId.Body_RightScapula, "" },
			{ OVRBodyBoneId.Body_RightArmUpper, "RightArm" },
			{ OVRBodyBoneId.Body_RightArmLower, "RightForeArm" },
			{ OVRBodyBoneId.Body_RightHandWristTwist, "" },
			{ OVRBodyBoneId.Body_LeftHandPalm, "" },
			{ OVRBodyBoneId.Body_LeftHandWrist, "LeftHand" },
			{ OVRBodyBoneId.Body_LeftHandThumbMetacarpal, "LeftHandThumb1" },
			{ OVRBodyBoneId.Body_LeftHandThumbProximal, "LeftHandThumb2" },
			{ OVRBodyBoneId.Body_LeftHandThumbDistal, "LeftHandThumb3" },
			{ OVRBodyBoneId.Body_LeftHandThumbTip, "LeftHandThumb4" },
			{ OVRBodyBoneId.Body_LeftHandIndexMetacarpal, "" },
			{ OVRBodyBoneId.Body_LeftHandIndexProximal, "LeftHandIndex1" },
			{ OVRBodyBoneId.Body_LeftHandIndexIntermediate, "LeftHandIndex2" },
			{ OVRBodyBoneId.Body_LeftHandIndexDistal, "LeftHandIndex3" },
			{ OVRBodyBoneId.Body_LeftHandIndexTip, "LeftHandIndex4" },
			{ OVRBodyBoneId.Body_LeftHandMiddleMetacarpal, "" },
			{ OVRBodyBoneId.Body_LeftHandMiddleProximal, "LeftHandMiddle1" },
			{ OVRBodyBoneId.Body_LeftHandMiddleIntermediate, "LeftHandMiddle2" },
			{ OVRBodyBoneId.Body_LeftHandMiddleDistal, "LeftHandMiddle3" },
			{ OVRBodyBoneId.Body_LeftHandMiddleTip, "LeftHandMiddle4" },
			{ OVRBodyBoneId.Body_LeftHandRingMetacarpal, "" },
			{ OVRBodyBoneId.Body_LeftHandRingProximal, "LeftHandRing1" },
			{ OVRBodyBoneId.Body_LeftHandRingIntermediate, "LeftHandRing2" },
			{ OVRBodyBoneId.Body_LeftHandRingDistal, "LeftHandRing3" },
			{ OVRBodyBoneId.Body_LeftHandRingTip, "LeftHandRing4" },
			{ OVRBodyBoneId.Body_LeftHandLittleMetacarpal, "" },
			{ OVRBodyBoneId.Body_LeftHandLittleProximal, "LeftHandPinky1" },
			{ OVRBodyBoneId.Body_LeftHandLittleIntermediate, "LeftHandPinky2" },
			{ OVRBodyBoneId.Body_LeftHandLittleDistal, "LeftHandPinky3" },
			{ OVRBodyBoneId.Body_LeftHandLittleTip, "LeftHandPinky4" },
			{ OVRBodyBoneId.Body_RightHandPalm, "" },
			{ OVRBodyBoneId.Body_RightHandWrist, "RightHand" },
			{ OVRBodyBoneId.Body_RightHandThumbMetacarpal, "RightHandThumb1" },
			{ OVRBodyBoneId.Body_RightHandThumbProximal, "RightHandThumb2" },
			{ OVRBodyBoneId.Body_RightHandThumbDistal, "RightHandThumb3" },
			{ OVRBodyBoneId.Body_RightHandThumbTip, "RightHandThumb4" },
			{ OVRBodyBoneId.Body_RightHandIndexMetacarpal, "" },
			{ OVRBodyBoneId.Body_RightHandIndexProximal, "RightHandIndex1" },
			{ OVRBodyBoneId.Body_RightHandIndexIntermediate, "RightHandIndex2" },
			{ OVRBodyBoneId.Body_RightHandIndexDistal, "RightHandIndex3" },
			{ OVRBodyBoneId.Body_RightHandIndexTip, "RightHandIndex4" },
			{ OVRBodyBoneId.Body_RightHandMiddleMetacarpal, "" },
			{ OVRBodyBoneId.Body_RightHandMiddleProximal, "RightHandMiddle1" },
			{ OVRBodyBoneId.Body_RightHandMiddleIntermediate, "RightHandMiddle2" },
			{ OVRBodyBoneId.Body_RightHandMiddleDistal, "RightHandMiddle3" },
			{ OVRBodyBoneId.Body_RightHandMiddleTip, "RightHandMiddle4" },
			{ OVRBodyBoneId.Body_RightHandRingMetacarpal, "" },
			{ OVRBodyBoneId.Body_RightHandRingProximal, "RightHandRing1" },
			{ OVRBodyBoneId.Body_RightHandRingIntermediate, "RightHandRing2" },
			{ OVRBodyBoneId.Body_RightHandRingDistal, "RightHandRing3" },
			{ OVRBodyBoneId.Body_RightHandRingTip, "RightHandRing4" },
			{ OVRBodyBoneId.Body_RightHandLittleMetacarpal, "" },
			{ OVRBodyBoneId.Body_RightHandLittleProximal, "RightHandPinky1" },
			{ OVRBodyBoneId.Body_RightHandLittleIntermediate, "RightHandPinky2" },
			{ OVRBodyBoneId.Body_RightHandLittleDistal, "RightHandPinky3" },
			{ OVRBodyBoneId.Body_RightHandLittleTip, "RightHandPinky4" },

			{ OVRBodyBoneId.LowerBody_LeftLegUpper, "LeftUpLeg" },
			{ OVRBodyBoneId.LowerBody_LeftLegLower, "LeftLeg" },
			{ OVRBodyBoneId.LowerBody_LeftFoot, "LeftFoot" },
			{ OVRBodyBoneId.LowerBody_RightLegUpper, "RightUpLeg" },
			{ OVRBodyBoneId.LowerBody_RightLegLower, "RightLeg" },
			{ OVRBodyBoneId.LowerBody_RightFoot, "RightFoot" }
		};

		public static readonly Dictionary<OVRBodyBoneId, string> boneIdToOVRBones = new Dictionary<OVRBodyBoneId, string>()
		{
			{ OVRBodyBoneId.Body_Root, "Root" },
			{ OVRBodyBoneId.Body_Hips, "Hips" },
			{ OVRBodyBoneId.Body_SpineLower, "SpineLower" },
			{ OVRBodyBoneId.Body_SpineMiddle, "SpineMiddle" },
			{ OVRBodyBoneId.Body_SpineUpper, "SpineUpper" },
			{ OVRBodyBoneId.Body_Chest, "Chest" },
			{ OVRBodyBoneId.Body_Neck, "Neck" },
			{ OVRBodyBoneId.Body_Head, "Head" },
			{ OVRBodyBoneId.Body_LeftShoulder, "LeftShoulder" },
			{ OVRBodyBoneId.Body_LeftScapula, "LeftScapula" },
			{ OVRBodyBoneId.Body_LeftArmUpper, "LeftArmUpper" },
			{ OVRBodyBoneId.Body_LeftArmLower, "LeftArmLower" },
			{ OVRBodyBoneId.Body_LeftHandWristTwist, "LeftHandWristTwist" },
			{ OVRBodyBoneId.Body_RightShoulder, "RightShoulder" },
			{ OVRBodyBoneId.Body_RightScapula, "RightScapula" },
			{ OVRBodyBoneId.Body_RightArmUpper, "RightArmUpper" },
			{ OVRBodyBoneId.Body_RightArmLower, "RightArmLower" },
			{ OVRBodyBoneId.Body_RightHandWristTwist, "RightHandWristTwist" },
			{ OVRBodyBoneId.Body_LeftHandPalm, "LeftHandPalm" },
			{ OVRBodyBoneId.Body_LeftHandWrist, "LeftHandWrist" },
			{ OVRBodyBoneId.Body_LeftHandThumbMetacarpal, "LeftHandThumbMeta" },
			{ OVRBodyBoneId.Body_LeftHandThumbProximal, "LeftHandThumbProximal" },
			{ OVRBodyBoneId.Body_LeftHandThumbDistal, "LeftHandThumbDistal" },
			{ OVRBodyBoneId.Body_LeftHandThumbTip, "LeftHandThumbTip" },
			{ OVRBodyBoneId.Body_LeftHandIndexMetacarpal, "LeftHandIndexMetacarpal" },
			{ OVRBodyBoneId.Body_LeftHandIndexProximal, "LeftHandIndexProximal" },
			{ OVRBodyBoneId.Body_LeftHandIndexIntermediate, "LeftHandIndexIntermediate" },
			{ OVRBodyBoneId.Body_LeftHandIndexDistal, "LeftHandIndexDistal" },
			{ OVRBodyBoneId.Body_LeftHandIndexTip, "LeftHandIndexTip" },
			{ OVRBodyBoneId.Body_LeftHandMiddleMetacarpal, "LeftHandMiddleMetacarpal" },
			{ OVRBodyBoneId.Body_LeftHandMiddleProximal, "LeftHandMiddleProximal" },
			{ OVRBodyBoneId.Body_LeftHandMiddleIntermediate, "LeftHandMiddleIntermediate" },
			{ OVRBodyBoneId.Body_LeftHandMiddleDistal, "LeftHandMiddleDistal" },
			{ OVRBodyBoneId.Body_LeftHandMiddleTip, "LeftHandMiddleTip" },
			{ OVRBodyBoneId.Body_LeftHandRingMetacarpal, "LeftHandRingMetacarpal" },
			{ OVRBodyBoneId.Body_LeftHandRingProximal, "LeftHandRingProximal" },
			{ OVRBodyBoneId.Body_LeftHandRingIntermediate, "LeftHandRingIntermediate" },
			{ OVRBodyBoneId.Body_LeftHandRingDistal, "LeftHandRingDistal" },
			{ OVRBodyBoneId.Body_LeftHandRingTip, "LeftHandRingTip" },
			{ OVRBodyBoneId.Body_LeftHandLittleMetacarpal, "LeftHandPinkyMeta" },
			{ OVRBodyBoneId.Body_LeftHandLittleProximal, "LeftHandPinkyProximal" },
			{ OVRBodyBoneId.Body_LeftHandLittleIntermediate, "LeftHandPinkyIntermediate" },
			{ OVRBodyBoneId.Body_LeftHandLittleDistal, "LeftHandPinkyDistal" },
			{ OVRBodyBoneId.Body_LeftHandLittleTip, "LeftHandPinkyTip" },
			{ OVRBodyBoneId.Body_RightHandPalm, "RightHandPalm" },
			{ OVRBodyBoneId.Body_RightHandWrist, "RightHandWrist" },
			{ OVRBodyBoneId.Body_RightHandThumbMetacarpal, "RightHandThumbMeta" },
			{ OVRBodyBoneId.Body_RightHandThumbProximal, "RightHandThumbProximal" },
			{ OVRBodyBoneId.Body_RightHandThumbDistal, "RightHandThumbDistal" },
			{ OVRBodyBoneId.Body_RightHandThumbTip, "RightHandThumbTip" },
			{ OVRBodyBoneId.Body_RightHandIndexMetacarpal, "RightHandIndexMetacarpal" },
			{ OVRBodyBoneId.Body_RightHandIndexProximal, "RightHandIndexProximal" },
			{ OVRBodyBoneId.Body_RightHandIndexIntermediate, "RightHandIndexIntermediate" },
			{ OVRBodyBoneId.Body_RightHandIndexDistal, "RightHandIndexDistal" },
			{ OVRBodyBoneId.Body_RightHandIndexTip, "RightHandIndexTip" },
			{ OVRBodyBoneId.Body_RightHandMiddleMetacarpal, "RightHandMiddleMetacarpal" },
			{ OVRBodyBoneId.Body_RightHandMiddleProximal, "RightHandMiddleProximal" },
			{ OVRBodyBoneId.Body_RightHandMiddleIntermediate, "RightHandMiddleIntermediate" },
			{ OVRBodyBoneId.Body_RightHandMiddleDistal, "RightHandMiddleDistal" },
			{ OVRBodyBoneId.Body_RightHandMiddleTip, "RightHandMiddleTip" },
			{ OVRBodyBoneId.Body_RightHandRingMetacarpal, "RightHandRingMetacarpal" },
			{ OVRBodyBoneId.Body_RightHandRingProximal, "RightHandRingProximal" },
			{ OVRBodyBoneId.Body_RightHandRingIntermediate, "RightHandRingIntermediate" },
			{ OVRBodyBoneId.Body_RightHandRingDistal, "RightHandRingDistal" },
			{ OVRBodyBoneId.Body_RightHandRingTip, "RightHandRingTip" },
			{ OVRBodyBoneId.Body_RightHandLittleMetacarpal, "RightHandPinkyMeta" },
			{ OVRBodyBoneId.Body_RightHandLittleProximal, "RightHandPinkyProximal" },
			{ OVRBodyBoneId.Body_RightHandLittleIntermediate, "RightHandPinkyIntermediate" },
			{ OVRBodyBoneId.Body_RightHandLittleDistal, "RightHandPinkyDistal" },
			{ OVRBodyBoneId.Body_RightHandLittleTip, "RightHandPinkyTip" },

			{ OVRBodyBoneId.LowerBody_LeftLegUpper, "LeftLegUpper" },
			{ OVRBodyBoneId.LowerBody_LeftLegLower, "LeftLegLower" },
			{ OVRBodyBoneId.LowerBody_LeftFoot, "LeftFootAnkle" },
			{ OVRBodyBoneId.LowerBody_RightLegUpper, "RightLegUpper" },
			{ OVRBodyBoneId.LowerBody_RightLegLower, "RightLegLower" },
			{ OVRBodyBoneId.LowerBody_RightFoot, "RightFootAnkle" }
		};
	}
}
