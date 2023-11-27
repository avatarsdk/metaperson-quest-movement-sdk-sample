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
	public enum OVRBodyBoneId
	{
		Body_Start = OVRPlugin.BoneId.Body_Start,
		Body_Root = OVRPlugin.BoneId.Body_Root,
		Body_Hips = OVRPlugin.BoneId.Body_Hips,
		Body_SpineLower = OVRPlugin.BoneId.Body_SpineLower,
		Body_SpineMiddle = OVRPlugin.BoneId.Body_SpineMiddle,
		Body_SpineUpper = OVRPlugin.BoneId.Body_SpineUpper,
		Body_Chest = OVRPlugin.BoneId.Body_Chest,
		Body_Neck = OVRPlugin.BoneId.Body_Neck,
		Body_Head = OVRPlugin.BoneId.Body_Head,
		Body_LeftShoulder = OVRPlugin.BoneId.Body_LeftShoulder,
		Body_LeftScapula = OVRPlugin.BoneId.Body_LeftScapula,
		Body_LeftArmUpper = OVRPlugin.BoneId.Body_LeftArmUpper,
		Body_LeftArmLower = OVRPlugin.BoneId.Body_LeftArmLower,
		Body_LeftHandWristTwist = OVRPlugin.BoneId.Body_LeftHandWristTwist,
		Body_RightShoulder = OVRPlugin.BoneId.Body_RightShoulder,
		Body_RightScapula = OVRPlugin.BoneId.Body_RightScapula,
		Body_RightArmUpper = OVRPlugin.BoneId.Body_RightArmUpper,
		Body_RightArmLower = OVRPlugin.BoneId.Body_RightArmLower,
		Body_RightHandWristTwist = OVRPlugin.BoneId.Body_RightHandWristTwist,
		Body_LeftHandPalm = OVRPlugin.BoneId.Body_LeftHandPalm,
		Body_LeftHandWrist = OVRPlugin.BoneId.Body_LeftHandWrist,
		Body_LeftHandThumbMetacarpal = OVRPlugin.BoneId.Body_LeftHandThumbMetacarpal,
		Body_LeftHandThumbProximal = OVRPlugin.BoneId.Body_LeftHandThumbProximal,
		Body_LeftHandThumbDistal = OVRPlugin.BoneId.Body_LeftHandThumbDistal,
		Body_LeftHandThumbTip = OVRPlugin.BoneId.Body_LeftHandThumbTip,
		Body_LeftHandIndexMetacarpal = OVRPlugin.BoneId.Body_LeftHandIndexMetacarpal,
		Body_LeftHandIndexProximal = OVRPlugin.BoneId.Body_LeftHandIndexProximal,
		Body_LeftHandIndexIntermediate = OVRPlugin.BoneId.Body_LeftHandIndexIntermediate,
		Body_LeftHandIndexDistal = OVRPlugin.BoneId.Body_LeftHandIndexDistal,
		Body_LeftHandIndexTip = OVRPlugin.BoneId.Body_LeftHandIndexTip,
		Body_LeftHandMiddleMetacarpal = OVRPlugin.BoneId.Body_LeftHandMiddleMetacarpal,
		Body_LeftHandMiddleProximal = OVRPlugin.BoneId.Body_LeftHandMiddleProximal,
		Body_LeftHandMiddleIntermediate = OVRPlugin.BoneId.Body_LeftHandMiddleIntermediate,
		Body_LeftHandMiddleDistal = OVRPlugin.BoneId.Body_LeftHandMiddleDistal,
		Body_LeftHandMiddleTip = OVRPlugin.BoneId.Body_LeftHandMiddleTip,
		Body_LeftHandRingMetacarpal = OVRPlugin.BoneId.Body_LeftHandRingMetacarpal,
		Body_LeftHandRingProximal = OVRPlugin.BoneId.Body_LeftHandRingProximal,
		Body_LeftHandRingIntermediate = OVRPlugin.BoneId.Body_LeftHandRingIntermediate,
		Body_LeftHandRingDistal = OVRPlugin.BoneId.Body_LeftHandRingDistal,
		Body_LeftHandRingTip = OVRPlugin.BoneId.Body_LeftHandRingTip,
		Body_LeftHandLittleMetacarpal = OVRPlugin.BoneId.Body_LeftHandLittleMetacarpal,
		Body_LeftHandLittleProximal = OVRPlugin.BoneId.Body_LeftHandLittleProximal,
		Body_LeftHandLittleIntermediate = OVRPlugin.BoneId.Body_LeftHandLittleIntermediate,
		Body_LeftHandLittleDistal = OVRPlugin.BoneId.Body_LeftHandLittleDistal,
		Body_LeftHandLittleTip = OVRPlugin.BoneId.Body_LeftHandLittleTip,
		Body_RightHandPalm = OVRPlugin.BoneId.Body_RightHandPalm,
		Body_RightHandWrist = OVRPlugin.BoneId.Body_RightHandWrist,
		Body_RightHandThumbMetacarpal = OVRPlugin.BoneId.Body_RightHandThumbMetacarpal,
		Body_RightHandThumbProximal = OVRPlugin.BoneId.Body_RightHandThumbProximal,
		Body_RightHandThumbDistal = OVRPlugin.BoneId.Body_RightHandThumbDistal,
		Body_RightHandThumbTip = OVRPlugin.BoneId.Body_RightHandThumbTip,
		Body_RightHandIndexMetacarpal = OVRPlugin.BoneId.Body_RightHandIndexMetacarpal,
		Body_RightHandIndexProximal = OVRPlugin.BoneId.Body_RightHandIndexProximal,
		Body_RightHandIndexIntermediate = OVRPlugin.BoneId.Body_RightHandIndexIntermediate,
		Body_RightHandIndexDistal = OVRPlugin.BoneId.Body_RightHandIndexDistal,
		Body_RightHandIndexTip = OVRPlugin.BoneId.Body_RightHandIndexTip,
		Body_RightHandMiddleMetacarpal = OVRPlugin.BoneId.Body_RightHandMiddleMetacarpal,
		Body_RightHandMiddleProximal = OVRPlugin.BoneId.Body_RightHandMiddleProximal,
		Body_RightHandMiddleIntermediate = OVRPlugin.BoneId.Body_RightHandMiddleIntermediate,
		Body_RightHandMiddleDistal = OVRPlugin.BoneId.Body_RightHandMiddleDistal,
		Body_RightHandMiddleTip = OVRPlugin.BoneId.Body_RightHandMiddleTip,
		Body_RightHandRingMetacarpal = OVRPlugin.BoneId.Body_RightHandRingMetacarpal,
		Body_RightHandRingProximal = OVRPlugin.BoneId.Body_RightHandRingProximal,
		Body_RightHandRingIntermediate = OVRPlugin.BoneId.Body_RightHandRingIntermediate,
		Body_RightHandRingDistal = OVRPlugin.BoneId.Body_RightHandRingDistal,
		Body_RightHandRingTip = OVRPlugin.BoneId.Body_RightHandRingTip,
		Body_RightHandLittleMetacarpal = OVRPlugin.BoneId.Body_RightHandLittleMetacarpal,
		Body_RightHandLittleProximal = OVRPlugin.BoneId.Body_RightHandLittleProximal,
		Body_RightHandLittleIntermediate = OVRPlugin.BoneId.Body_RightHandLittleIntermediate,
		Body_RightHandLittleDistal = OVRPlugin.BoneId.Body_RightHandLittleDistal,
		Body_RightHandLittleTip = OVRPlugin.BoneId.Body_RightHandLittleTip,
		Body_End = OVRPlugin.BoneId.Body_End,
	}
}
