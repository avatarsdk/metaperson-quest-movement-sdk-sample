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

		public static readonly Dictionary<OVRPlugin.BoneId, string> boneIdToMetaPersonBones = new Dictionary<OVRPlugin.BoneId, string>()
		{
			{ OVRPlugin.BoneId.Body_Root, "" },
			{ OVRPlugin.BoneId.Body_Hips, "Hips" },
			{ OVRPlugin.BoneId.Body_SpineLower, "" },
			{ OVRPlugin.BoneId.Body_SpineMiddle, "Spine" },
			{ OVRPlugin.BoneId.Body_SpineUpper, "Spine1" },
			{ OVRPlugin.BoneId.Body_Chest, "Spine2" },
			{ OVRPlugin.BoneId.Body_Neck, "Neck2" },
			{ OVRPlugin.BoneId.Body_Head, "Head" },
			{ OVRPlugin.BoneId.Body_LeftShoulder, "LeftShoulder" },
			{ OVRPlugin.BoneId.Body_LeftScapula, "" },
			{ OVRPlugin.BoneId.Body_LeftArmUpper, "LeftArm" },
			{ OVRPlugin.BoneId.Body_LeftArmLower, "LeftForeArm" },
			{ OVRPlugin.BoneId.Body_LeftHandWristTwist, "" },
			{ OVRPlugin.BoneId.Body_RightShoulder, "RightShoulder" },
			{ OVRPlugin.BoneId.Body_RightScapula, "" },
			{ OVRPlugin.BoneId.Body_RightArmUpper, "RightArm" },
			{ OVRPlugin.BoneId.Body_RightArmLower, "RightForeArm" },
			{ OVRPlugin.BoneId.Body_RightHandWristTwist, "" },
			{ OVRPlugin.BoneId.Body_LeftHandPalm, "" },
			{ OVRPlugin.BoneId.Body_LeftHandWrist, "LeftHand" },
			{ OVRPlugin.BoneId.Body_LeftHandThumbMetacarpal, "LeftHandThumb1" },
			{ OVRPlugin.BoneId.Body_LeftHandThumbProximal, "LeftHandThumb2" },
			{ OVRPlugin.BoneId.Body_LeftHandThumbDistal, "LeftHandThumb3" },
			{ OVRPlugin.BoneId.Body_LeftHandThumbTip, "LeftHandThumb4" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexMetacarpal, "" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexProximal, "LeftHandIndex1" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexIntermediate, "LeftHandIndex2" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexDistal, "LeftHandIndex3" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexTip, "LeftHandIndex4" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleMetacarpal, "" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleProximal, "LeftHandMiddle1" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleIntermediate, "LeftHandMiddle2" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleDistal, "LeftHandMiddle3" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleTip, "LeftHandMiddle4" },
			{ OVRPlugin.BoneId.Body_LeftHandRingMetacarpal, "" },
			{ OVRPlugin.BoneId.Body_LeftHandRingProximal, "LeftHandRing1" },
			{ OVRPlugin.BoneId.Body_LeftHandRingIntermediate, "LeftHandRing2" },
			{ OVRPlugin.BoneId.Body_LeftHandRingDistal, "LeftHandRing3" },
			{ OVRPlugin.BoneId.Body_LeftHandRingTip, "LeftHandRing4" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleMetacarpal, "" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleProximal, "LeftHandPinky1" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleIntermediate, "LeftHandPinky2" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleDistal, "LeftHandPinky3" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleTip, "LeftHandPinky4" },
			{ OVRPlugin.BoneId.Body_RightHandPalm, "" },
			{ OVRPlugin.BoneId.Body_RightHandWrist, "RightHand" },
			{ OVRPlugin.BoneId.Body_RightHandThumbMetacarpal, "RightHandThumb1" },
			{ OVRPlugin.BoneId.Body_RightHandThumbProximal, "RightHandThumb2" },
			{ OVRPlugin.BoneId.Body_RightHandThumbDistal, "RightHandThumb3" },
			{ OVRPlugin.BoneId.Body_RightHandThumbTip, "RightHandThumb4" },
			{ OVRPlugin.BoneId.Body_RightHandIndexMetacarpal, "" },
			{ OVRPlugin.BoneId.Body_RightHandIndexProximal, "RightHandIndex1" },
			{ OVRPlugin.BoneId.Body_RightHandIndexIntermediate, "RightHandIndex2" },
			{ OVRPlugin.BoneId.Body_RightHandIndexDistal, "RightHandIndex3" },
			{ OVRPlugin.BoneId.Body_RightHandIndexTip, "RightHandIndex4" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleMetacarpal, "" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleProximal, "RightHandMiddle1" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleIntermediate, "RightHandMiddle2" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleDistal, "RightHandMiddle3" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleTip, "RightHandMiddle4" },
			{ OVRPlugin.BoneId.Body_RightHandRingMetacarpal, "" },
			{ OVRPlugin.BoneId.Body_RightHandRingProximal, "RightHandRing1" },
			{ OVRPlugin.BoneId.Body_RightHandRingIntermediate, "RightHandRing2" },
			{ OVRPlugin.BoneId.Body_RightHandRingDistal, "RightHandRing3" },
			{ OVRPlugin.BoneId.Body_RightHandRingTip, "RightHandRing4" },
			{ OVRPlugin.BoneId.Body_RightHandLittleMetacarpal, "" },
			{ OVRPlugin.BoneId.Body_RightHandLittleProximal, "RightHandPinky1" },
			{ OVRPlugin.BoneId.Body_RightHandLittleIntermediate, "RightHandPinky2" },
			{ OVRPlugin.BoneId.Body_RightHandLittleDistal, "RightHandPinky3" },
			{ OVRPlugin.BoneId.Body_RightHandLittleTip, "RightHandPinky4" },
			{ OVRPlugin.BoneId.Body_End, "" }
		};

		public static readonly Dictionary<OVRPlugin.BoneId, string> boneIdToOVRBones = new Dictionary<OVRPlugin.BoneId, string>()
		{
			{ OVRPlugin.BoneId.Body_Root, "Root" },
			{ OVRPlugin.BoneId.Body_Hips, "Hips" },
			{ OVRPlugin.BoneId.Body_SpineLower, "SpineLower" },
			{ OVRPlugin.BoneId.Body_SpineMiddle, "SpineMiddle" },
			{ OVRPlugin.BoneId.Body_SpineUpper, "SpineUpper" },
			{ OVRPlugin.BoneId.Body_Chest, "Chest" },
			{ OVRPlugin.BoneId.Body_Neck, "Neck" },
			{ OVRPlugin.BoneId.Body_Head, "Head" },
			{ OVRPlugin.BoneId.Body_LeftShoulder, "LeftShoulder" },
			{ OVRPlugin.BoneId.Body_LeftScapula, "LeftScapula" },
			{ OVRPlugin.BoneId.Body_LeftArmUpper, "LeftArmUpper" },
			{ OVRPlugin.BoneId.Body_LeftArmLower, "LeftArmLower" },
			{ OVRPlugin.BoneId.Body_LeftHandWristTwist, "LeftHandWristTwist" },
			{ OVRPlugin.BoneId.Body_RightShoulder, "RightShoulder" },
			{ OVRPlugin.BoneId.Body_RightScapula, "RightScapula" },
			{ OVRPlugin.BoneId.Body_RightArmUpper, "RightArmUpper" },
			{ OVRPlugin.BoneId.Body_RightArmLower, "RightArmLower" },
			{ OVRPlugin.BoneId.Body_RightHandWristTwist, "RightHandWristTwist" },
			{ OVRPlugin.BoneId.Body_LeftHandPalm, "LeftHandPalm" },
			{ OVRPlugin.BoneId.Body_LeftHandWrist, "LeftHandWrist" },
			{ OVRPlugin.BoneId.Body_LeftHandThumbMetacarpal, "LeftHandThumbMeta" },
			{ OVRPlugin.BoneId.Body_LeftHandThumbProximal, "LeftHandThumbProximal" },
			{ OVRPlugin.BoneId.Body_LeftHandThumbDistal, "LeftHandThumbDistal" },
			{ OVRPlugin.BoneId.Body_LeftHandThumbTip, "LeftHandThumbTip" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexMetacarpal, "LeftHandIndexMetacarpal" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexProximal, "LeftHandIndexProximal" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexIntermediate, "LeftHandIndexIntermediate" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexDistal, "LeftHandIndexDistal" },
			{ OVRPlugin.BoneId.Body_LeftHandIndexTip, "LeftHandIndexTip" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleMetacarpal, "LeftHandMiddleMetacarpal" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleProximal, "LeftHandMiddleProximal" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleIntermediate, "LeftHandMiddleIntermediate" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleDistal, "LeftHandMiddleDistal" },
			{ OVRPlugin.BoneId.Body_LeftHandMiddleTip, "LeftHandMiddleTip" },
			{ OVRPlugin.BoneId.Body_LeftHandRingMetacarpal, "LeftHandRingMetacarpal" },
			{ OVRPlugin.BoneId.Body_LeftHandRingProximal, "LeftHandRingProximal" },
			{ OVRPlugin.BoneId.Body_LeftHandRingIntermediate, "LeftHandRingIntermediate" },
			{ OVRPlugin.BoneId.Body_LeftHandRingDistal, "LeftHandRingDistal" },
			{ OVRPlugin.BoneId.Body_LeftHandRingTip, "LeftHandRingTip" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleMetacarpal, "LeftHandPinkyMeta" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleProximal, "LeftHandPinkyProximal" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleIntermediate, "LeftHandPinkyIntermediate" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleDistal, "LeftHandPinkyDistal" },
			{ OVRPlugin.BoneId.Body_LeftHandLittleTip, "LeftHandPinkyTip" },
			{ OVRPlugin.BoneId.Body_RightHandPalm, "RightHandPalm" },
			{ OVRPlugin.BoneId.Body_RightHandWrist, "RightHandWrist" },
			{ OVRPlugin.BoneId.Body_RightHandThumbMetacarpal, "RightHandThumbMeta" },
			{ OVRPlugin.BoneId.Body_RightHandThumbProximal, "RightHandThumbProximal" },
			{ OVRPlugin.BoneId.Body_RightHandThumbDistal, "RightHandThumbDistal" },
			{ OVRPlugin.BoneId.Body_RightHandThumbTip, "RightHandThumbTip" },
			{ OVRPlugin.BoneId.Body_RightHandIndexMetacarpal, "RightHandIndexMetacarpal" },
			{ OVRPlugin.BoneId.Body_RightHandIndexProximal, "RightHandIndexProximal" },
			{ OVRPlugin.BoneId.Body_RightHandIndexIntermediate, "RightHandIndexIntermediate" },
			{ OVRPlugin.BoneId.Body_RightHandIndexDistal, "RightHandIndexDistal" },
			{ OVRPlugin.BoneId.Body_RightHandIndexTip, "RightHandIndexTip" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleMetacarpal, "RightHandMiddleMetacarpal" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleProximal, "RightHandMiddleProximal" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleIntermediate, "RightHandMiddleIntermediate" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleDistal, "RightHandMiddleDistal" },
			{ OVRPlugin.BoneId.Body_RightHandMiddleTip, "RightHandMiddleTip" },
			{ OVRPlugin.BoneId.Body_RightHandRingMetacarpal, "RightHandRingMetacarpal" },
			{ OVRPlugin.BoneId.Body_RightHandRingProximal, "RightHandRingProximal" },
			{ OVRPlugin.BoneId.Body_RightHandRingIntermediate, "RightHandRingIntermediate" },
			{ OVRPlugin.BoneId.Body_RightHandRingDistal, "RightHandRingDistal" },
			{ OVRPlugin.BoneId.Body_RightHandRingTip, "RightHandRingTip" },
			{ OVRPlugin.BoneId.Body_RightHandLittleMetacarpal, "RightHandPinkyMeta" },
			{ OVRPlugin.BoneId.Body_RightHandLittleProximal, "RightHandPinkyProximal" },
			{ OVRPlugin.BoneId.Body_RightHandLittleIntermediate, "RightHandPinkyIntermediate" },
			{ OVRPlugin.BoneId.Body_RightHandLittleDistal, "RightHandPinkyDistal" },
			{ OVRPlugin.BoneId.Body_RightHandLittleTip, "RightHandPinkyTip" },
			{ OVRPlugin.BoneId.Body_End, "" }
		};
	}
}
