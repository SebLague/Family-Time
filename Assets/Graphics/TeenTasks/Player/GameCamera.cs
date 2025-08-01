using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	public event System.Action<Camera> gameCameraUpdateComplete;

	public enum ViewMode
	{
		LookingForward,
		LookingBehind,
	}

	public ViewMode activeView = ViewMode.LookingForward;

	public float fovSlow = 60;
	public float fovFast = 65;
	public float fovBoost = 75;
	public float fovSmoothTime;

	[Header("Top-Down Settings")]
	public float distanceAbove = 2;

	public float turnSpeed;
	public float startAngle;

	float angle;

	[Header("Alternate View Settings")]
	public ViewSettings lookingAheadView;

	public ViewSettings lookingBehindView;
	public ViewSettings menuView;

	[Header("References")]
	public Camera cam;


	// Other
	public Player player;
	Transform target;
	float smoothFovVelocity;

	float menuToGameViewTransitionT;


	void Start()
	{
		InitView();
	}

	public void SetActiveView(ViewMode viewMode)
	{
		activeView = viewMode;
	}


	public void InitView()
	{
		target = player.transform;
		UpdateView();
		cam.fieldOfView = CalculateFOV();
	}


	void LateUpdate()
	{
		UpdateView();

		cam.fieldOfView = Mathf.SmoothDamp(cam.fieldOfView, CalculateFOV(), ref smoothFovVelocity, fovSmoothTime);


		gameCameraUpdateComplete?.Invoke(cam);
	}

	void UpdateView()
	{
		// Set camera based on active view
		switch (activeView)
		{
			case ViewMode.LookingForward:
				UpdateAlternateView(lookingAheadView);
				break;
			case ViewMode.LookingBehind:
				UpdateAlternateView(lookingBehindView);
				break;
		}
	}

	float CalculateFOV()
	{
		return (player.IsBoosting) ? fovBoost : Mathf.Lerp(fovSlow, fovFast, player.SpeedT);
	}
	
	void UpdateAlternateView(ViewSettings view)
	{
		// Calculate new position
		Vector3 newPos = target.position + target.forward * view.offset.z + player.GravityUp * view.offset.y;

		//Calculate look target
		Vector3 lookTarget = target.position;
		lookTarget += target.right * view.lookTargetOffset.x;
		lookTarget += target.up * view.lookTargetOffset.y;
		lookTarget += target.forward * view.lookTargetOffset.z;

		transform.position = newPos;
		transform.LookAt(lookTarget, player.GravityUp);
	}
	

	[System.Serializable]
	public struct ViewSettings
	{
		public Vector3 offset;
		public Vector3 lookTargetOffset;

		public ViewSettings(Vector3 offset, Vector3 lookTargetOffset)
		{
			this.offset = offset;
			this.lookTargetOffset = lookTargetOffset;
		}

		public static ViewSettings Lerp(ViewSettings a, ViewSettings b, float t)
		{
			Vector3 offset = Vector3.Lerp(a.offset, b.offset, t);
			Vector3 lookTargetOffset = Vector3.Lerp(a.lookTargetOffset, b.lookTargetOffset, t);
			return new ViewSettings(offset, lookTargetOffset);
		}
	}
}