using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aircraft : MonoBehaviour
{
	public bool isplaying;
	public Transform sun;
	public Vector2 sunOffset;
	public float sunSpeed;
	public TMPro.TMP_Text scoreText;
	public GameObject blackScreen;
	public GameObject noController;

	[Header("Startup Settings")]
	public float startElevationT;

	bool worldIsSpherical = true;

	public float worldRadius;

	[Header("Elevation")]
	public float minElevation;

	public float maxElevation;
	public float currentElevation;

	[Header("Movement")]
	public float turnSpeedInTopDownView;

	public float turnSpeedInBehindView;
	public float minSpeed = 4;
	public float maxSpeed = 12;
	public float accelerateDuration = 2;
	public float boostSpeed = 25;
	public float speedSmoothing = 0.1f;
	public float maxBoostTime = 30;
	public float boostTimeAtStart = 5;

	[HideInInspector]
	public float totalTurnAngle;

	public float smoothRollTime;
	public float rollAngle;
	public float smoothPitchTime;
	public float maxPitchAngle;
	public float changeElevationSpeed;
	public float turnSmoothTime;

	[Header("Graphics")]
	public Transform model;

	public Transform[] navigationLights;
	public Transform[] ailerons;
	public float aileronAngle = 20;
	public Transform propeller;
	public float propellerSpeed;


	[Header("Debug")]
	public float currentSpeed;

	public bool debug_TestInitPos;
	public bool debug_lockMovement;

	// Private stuff

	GameCamera gameCamera;
	float smoothedTurnSpeed;
	float turnSmoothV;
	float pitchSmoothV;
	float rollSmoothV;

	public float currentPitchAngle { get; private set; }
	public float currentRollAngle { get; private set; }
	public float turnInput { get; private set; }
	bool boosting;
	float boostTimeRemaining;
	float boostTimeToAdd;
	float baseTargetSpeed;


	float pitchInput;


	float nextNavigationLightUpdateTime;
	bool navigationLightsOn;


	int numHoopsHit;
	float winNotifyTime = -1;
	[HideInInspector] public bool hasWon;
	const int numHoops = 11;

	void OnTriggerEnter(Collider other)
	{
		if (!isplaying) return;
		if (other.CompareTag("Hoop"))
		{
			other.GetComponent<MeshRenderer>().material.color = Color.green;
			other.GetComponent<BoxCollider>().enabled = false;
			numHoopsHit++;
			UpdateScore();
		}
		else if (other.CompareTag("Terrain"))
		{
		}

		Debug.Log(other.tag);
	}

	void UpdateScore()
	{
		scoreText.text = $"{numHoopsHit} / {numHoops}";

		if (numHoopsHit >= numHoops && winNotifyTime <= 0)
		{
			winNotifyTime = Time.time + 1;
		}
	}

	void Awake()
	{
		gameCamera = FindFirstObjectByType<GameCamera>();

		SetStartPos();

		baseTargetSpeed = Mathf.Lerp(minSpeed, maxSpeed, 0.35f);
		currentSpeed = baseTargetSpeed;
		SetNavigationLightScale(0);
		boostTimeRemaining = boostTimeAtStart;

		sun.rotation = GetSunTargetRotation();
		UpdateScore();
		blackScreen.SetActive(true);
	}

	public void StartGame(bool hasController)
	{
		isplaying = hasController;
		blackScreen.SetActive(!hasController);
		noController.SetActive(!hasController);
	}

	public void StopGame()
	{
		if (!hasWon)
		{
			isplaying = false;
			noController.SetActive(false);
			blackScreen.SetActive(true);
			SetStartPos();
		}
	}

	void Update()
	{
		if (!isplaying) return;

		if (Time.time > winNotifyTime && winNotifyTime > 0)
		{
			hasWon = true;
		}

		//if (GameController.IsState(GameState.Playing))
		{
			HandleInput();
			HandleMovement();
			UpdateBoostTimer();
		}

		UpdateGraphics();


		sun.rotation = Quaternion.Slerp(sun.rotation, GetSunTargetRotation(), Time.deltaTime * sunSpeed);
	}

	Quaternion GetSunTargetRotation()
	{
		Vector3 sunPos = transform.position + transform.position.normalized * 2;
		Vector3 targSun = transform.position + transform.forward * sunOffset.y + transform.right * sunOffset.x;
		Quaternion sunTargRot = Quaternion.LookRotation(targSun - sunPos);
		return sunTargRot;
	}

	public void UpdateMovementInput(Vector2 moveInput, float accelerateDir, bool boosting)
	{
		// Turning
		turnInput = moveInput.x;
		pitchInput = moveInput.y;

		// Speed
		baseTargetSpeed += (maxSpeed - minSpeed) / accelerateDuration * accelerateDir * Time.deltaTime;
		baseTargetSpeed = Mathf.Clamp(baseTargetSpeed, minSpeed, maxSpeed);

		this.boosting = boosting && boostTimeRemaining > 0;
	}


	void HandleInput()
	{
		if (hasWon) return;
		
		Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		float accelerateDir = movementInput.y;
		bool boosting = false;
		UpdateMovementInput(movementInput, accelerateDir, boosting);
	}


	void HandleMovement()
	{
		// Turn
		float turnSpeed = turnSpeedInBehindView;
		smoothedTurnSpeed = Mathf.SmoothDamp(smoothedTurnSpeed, turnInput * turnSpeed, ref turnSmoothV, turnSmoothTime);
		float turnAmount = smoothedTurnSpeed * Time.deltaTime;
		totalTurnAngle += turnAmount;


		// Update speed
		float targetSpeed = (boosting) ? boostSpeed : baseTargetSpeed;
		currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 1 - Mathf.Pow(speedSmoothing, Time.deltaTime));


		// Calculate forward and vertical components of the speed based on pitch angle of the plane
		float forwardSpeed = Mathf.Cos(Mathf.Abs(currentPitchAngle) * Mathf.Deg2Rad) * currentSpeed;
		float verticalVelocity = -Mathf.Sin(currentPitchAngle * Mathf.Deg2Rad) * currentSpeed;

		// Update elevation
		currentElevation += verticalVelocity * Time.deltaTime;
		currentElevation = Mathf.Clamp(currentElevation, minElevation, maxElevation);


		UpdatePosition(forwardSpeed);
		UpdateRotation(turnAmount);

		// --- Update pitch and roll ---
		float targetPitch = pitchInput * maxPitchAngle;
		// Automatically stop pitching the plane when reaching min/max elevation
		float dstToPitchLimit = (targetPitch > 0) ? currentElevation - minElevation : maxElevation - currentElevation;
		float pitchLimitSmoothDst = 3;
		targetPitch *= Mathf.Clamp01(dstToPitchLimit / pitchLimitSmoothDst);

		currentPitchAngle = Mathf.SmoothDampAngle(currentPitchAngle, targetPitch, ref pitchSmoothV, smoothPitchTime);


		float targetRoll = turnInput * rollAngle;
		currentRollAngle = Mathf.SmoothDampAngle(currentRollAngle, targetRoll, ref rollSmoothV, smoothRollTime);
	}

	void UpdateBoostTimer()
	{
		// Decrease boost timer when boosting
		if (boosting)
		{
			boostTimeRemaining = Mathf.Max(0, boostTimeRemaining - Time.deltaTime);
		}

		// Increase boost timer gradually when time has been gained
		if (boostTimeToAdd > 0)
		{
			const float boostAddSpeed = 4;
			float boostTimeToAddThisFrame = Mathf.Min(Time.deltaTime * boostAddSpeed, boostTimeToAdd);
			boostTimeRemaining += boostTimeToAddThisFrame;
			boostTimeToAdd -= boostTimeToAddThisFrame;
			boostTimeRemaining = Mathf.Min(boostTimeRemaining, maxBoostTime);
		}
	}

	void UpdatePosition(float forwardSpeed)
	{
		// Update position
		Vector3 newPos = transform.position + transform.forward * forwardSpeed * Time.deltaTime;
		if (worldIsSpherical)
		{
			newPos = newPos.normalized * (worldRadius + currentElevation);
		}
		else
		{
			newPos = new Vector3(newPos.x, currentElevation, newPos.z);
		}

		transform.position = newPos;
	}

	void UpdateRotation(float turnAmount)
	{
		if (worldIsSpherical)
		{
			Vector3 gravityUp = transform.position.normalized;
			transform.RotateAround(transform.position, gravityUp, turnAmount);
			transform.LookAt((transform.position + transform.forward * 10).normalized * (worldRadius + currentElevation), gravityUp);
			transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp) * transform.rotation;
		}
		else
		{
			transform.RotateAround(transform.position, Vector3.up, turnAmount);
		}
	}


	void UpdateGraphics()
	{
		// Rotate ailerons when turning
		UpdateAileronGraphic(ailerons[0], -turnInput * aileronAngle);
		UpdateAileronGraphic(ailerons[1], turnInput * aileronAngle);

		// Set pitch/roll rotation
		SetPlaneRotation();

		// Rotate propeller
		propeller.localEulerAngles += Vector3.forward * propellerSpeed * Time.deltaTime;

		// Turn on navigation lights at night (even if should technically be on always I think...)
		if (Time.time > nextNavigationLightUpdateTime)
		{
			bool isDark = true;
			navigationLightsOn = isDark;
			nextNavigationLightUpdateTime = Time.time + 3;
		}

		SetNavigationLightScale((navigationLightsOn) ? 1 : 0, true);

		void UpdateAileronGraphic(Transform aileron, float targetAngle)
		{
			Vector3 rot = aileron.localEulerAngles;
			float smoothAngle = Mathf.LerpAngle(rot.x, targetAngle, Time.deltaTime * 5);
			aileron.localEulerAngles = new Vector3(smoothAngle, rot.y, rot.z);
		}
	}

	void SetPlaneRotation()
	{
		model.localEulerAngles = new Vector3(currentPitchAngle, 0, currentRollAngle);
	}


	public void SetStartPos()
	{
		currentElevation = Mathf.Lerp(minElevation, maxElevation, startElevationT);
		transform.position = transform.position.normalized * (worldRadius + currentElevation);

		// Needs to be called twice to settle (Todo: fix this nonsense)
		SetStartRot();
		SetStartRot();


		void SetStartRot()
		{
			Vector3 gravityUp = transform.position.normalized;
			transform.rotation = Quaternion.FromToRotation(transform.up, gravityUp);
			transform.LookAt((transform.position + transform.forward * 10).normalized * (worldRadius + currentElevation), gravityUp);
			transform.Rotate(transform.position.normalized, 0, Space.World);

			UpdateRotation(0);
		}
	}


	// Allow navigation lights to be turned on and off by scaling them (crude way to allow brightness to fade in/out)
	void SetNavigationLightScale(float scale, bool smooth = false)
	{
		for (int i = 0; i < navigationLights.Length; i++)
		{
			if (smooth)
			{
				float currentScale = navigationLights[i].localScale.x;
				navigationLights[i].localScale = Vector3.one * Mathf.Lerp(currentScale, scale, Time.deltaTime);
			}
			else
			{
				navigationLights[i].localScale = Vector3.one * scale;
			}
		}
	}

	// ---- Public functions ----

	public void AddBoost(float time)
	{
		boostTimeToAdd += time;
	}


	public void SetPitch(float pitch)
	{
		currentPitchAngle = pitch;
		SetPlaneRotation();
	}

	// ---- Properties ----
	public Vector3 GravityUp
	{
		get { return (worldIsSpherical) ? transform.position.normalized : Vector3.up; }
	}

	public float Height
	{
		get { return worldRadius + currentElevation; }
	}

	public bool IsBoosting
	{
		get { return boosting; }
	}

	public bool BoosterIsCharging
	{
		get { return boostTimeToAdd > 0; }
	}

	// Get speed value remapped to [0,1] (0 at min speed, 1 at max speed. Note: still 1 when boosting).
	public float SpeedT
	{
		get { return Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed); }
	}

	public float TargetSpeedT
	{
		get
		{
			if (IsBoosting)
			{
				return 1;
			}

			return Mathf.InverseLerp(minSpeed, maxSpeed, baseTargetSpeed);
		}
	}

	public float BoostRemainingT
	{
		get { return Mathf.Clamp01(boostTimeRemaining / maxBoostTime); }
	}
}