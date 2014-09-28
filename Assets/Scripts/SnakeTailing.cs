using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Handles tail motion for snake.
/// </summary>
public class SnakeTailing : MonoBehaviour
{
	public int NumbNeckBones = 30;
	public float NeckBoneSamplingInterval = 2.5f;


	private List<Transform> neckBones = new List<Transform>();
	private List<Vector3> targetPoses = new List<Vector3>();
	private List<Vector3> toNext = new List<Vector3>();

	private Vector3 prevHeadPos;
	private float distanceSinceNewNode;

	void Awake()
	{
		//Get the neck bones.
		for (int i = 0; i < NumbNeckBones; ++i)
		{
			GameObject obj = GameObject.Find("Bone_Neck" + (i + 1).ToString());
			if (obj == null)
			{
				Debug.LogError("Couldn't find object 'Bone_Neck" + (i + 1).ToString() + "'");
				return;
			}

			neckBones.Add(obj.transform);
			if (i > 0)
			{
				targetPoses.Add(neckBones[i - 1].position);
				toNext.Add((targetPoses[i - 1] - neckBones[i].position).normalized);
			}
		}

		distanceSinceNewNode = 0.0f;
		prevHeadPos = neckBones[0].position;
	}

	void FixedUpdate()
	{
		//Get the amount of movement this past frame.
		Vector3 newPos = neckBones[0].position,
				delta = newPos - prevHeadPos;
		float dist = delta.magnitude;
		distanceSinceNewNode += dist;

		//If no movement has happened, don't bother doing any computations.
		if (dist == 0.0f)
		{
			prevHeadPos = newPos;
			return;
		}

		//If tail bones should have reached their current goal positions, update goal positions.
		if (distanceSinceNewNode >= NeckBoneSamplingInterval)
		{
			//See how may intervals the head skipped over in its movement last frame.
			float div = distanceSinceNewNode / NeckBoneSamplingInterval;
			int skips = (int)div;
			float extraMovement = div - skips;

			//Calculate the new target positions that the head skipped through.
			List<Vector3> skipTargetPoses = new List<Vector3>();
			for (int i = 0; i < skips; ++i)
			{
				float lerpVal = (skips == 1 ? 0.0f : ((float)i / (float)(skips - 1)));
				skipTargetPoses.Add(Vector3.Lerp(newPos, prevHeadPos, lerpVal));
			}

			//Make the new target positions. First pull from the new skipped-over positions.
			List<Vector3> newTargetPoses = new List<Vector3>();
			int oldTargIndex = 0;
			for (int i = 0; i < targetPoses.Count; ++i)
			{
				if (i < skipTargetPoses.Count)
				{
					newTargetPoses.Add(skipTargetPoses[i]);
				}
				else
				{
					newTargetPoses.Add(targetPoses[oldTargIndex]);
					++oldTargIndex;
				}
			}
			Vector3 lastTailPos = (oldTargIndex < targetPoses.Count) ? targetPoses[oldTargIndex] : targetPoses[targetPoses.Count - 1];
			targetPoses = newTargetPoses;

			//Move each bone up to the target position before its current target.
			for (int i = 1; i < neckBones.Count - 1; ++i)
				neckBones[i].position = targetPoses[i];
			neckBones[neckBones.Count - 1].position = lastTailPos;

			//Update the movement vector for each bone.
			for (int i = 0; i < toNext.Count; ++i)
				toNext[i] = (targetPoses[i] - neckBones[i + 1].position).normalized;

			//Push the bones towards their new target a bit.
			if (extraMovement > 0.0f)
			{
				for (int i = 1; i < neckBones.Count; ++i)
					neckBones[i].position += toNext[i - 1] * extraMovement;
			}

			distanceSinceNewNode = 0.0f;
		}
		else
		{
			for (int i = 1; i < neckBones.Count; ++i)
				neckBones[i].position += toNext[i - 1] * dist;
		}
		
		prevHeadPos = newPos;
	}
}