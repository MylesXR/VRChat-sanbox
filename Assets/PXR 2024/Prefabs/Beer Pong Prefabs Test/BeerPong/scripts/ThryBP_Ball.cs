﻿
using System;
using Thry.Clapper;
using Thry.General;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace Thry.BeerPong
{
    public class ThryBP_Ball : UdonSharpBehaviour
    {
        //Rimming
        const float RIMMING_TOTAL_AGULAR_ROTATION = 3000;
        const float RIMMING_SPEED = 1300;
        float rimming = 0;

        //Desktop strength
        const float MAX_MOUSE_DOWN_THROW_TIME = 1;
        const float MAX_THROW_STRENGTH = 5f;
        float rightMouseDownStart;
        float throwStrengthPercent = 0;

        //Phsyiscs
        const float BOUND_DRAG = 0.9f;

        //References and Settings
        public Transform throwIndicator;
        public float throwVelocityMultiplierVR = 1.5f;
        public float throwVelocityMultiplierDekstop = 1f;



        public VRC_Pickup _pickup;
        Rigidbody _rigidbody;
        Renderer _renderer;

        public ParticleSystem _splashParticles;
        private AudioSource _splashAudio;

        public Renderer _strengthIndicatorDesktop;



        //State
        [UdonSynced]
        byte[] state = new byte[] { 0, 0, 0, 0, 0, 0 };
        byte localState = 0;
        float stateStartTime = 0;
        float physicsSimulationStartTime = 0;
        bool _countedHit;

        const byte STATE_IDLE = 0;
        const byte STATE_IN_HAND = 1;
        const byte STATE_LOCKED = 2;
        const byte STATE_FYING = 3;
        const byte STATE_RIMING = 4;
        const byte STATE_IN_CUP = 5;

        [UdonSynced]
        public int currentPlayer = 0;

        [UdonSynced]
        Vector3 start_position;
        [UdonSynced]
        Quaternion start_rotation;
        [UdonSynced]
        bool isRightHand;
        [UdonSynced]
        Vector3 start_velocity;

        Vector3 startPositionLocal;
        Vector3 startVelocityLocal;

        //other
        float timeStartBeingStill = 0;
        float _radius = 1;
        float tableHeight;

        //References set by main script
        [HideInInspector]
        public Transform respawnHeight;
        [HideInInspector]
        public ThryBP_Main MainScript;

        //Velocity tacking
        Vector3 _lastPosition;
        Quaternion _lastRotation;
        const int VELOCITY_BUFFER_LENGTH = 10;
        int _lastvelociesHead = 0;
        Vector3[] _lastvelocies = new Vector3[VELOCITY_BUFFER_LENGTH];
        float[] _lastAngularVelocities = new float[VELOCITY_BUFFER_LENGTH];

        //Sounds
        [Header("Sounds")]
        public AudioClip audio_collision_table;
        public AudioClip audio_collision_glass;
        public AudioClip audio_water_splash;
        private AudioSource _audioSource;

        public void Init()
        {
            if(Networking.LocalPlayer == null) return;

            _rigidbody = GetComponent<Rigidbody>();
            _renderer = GetComponent<Renderer>();
            _audioSource = GetComponent<AudioSource>();
            _splashAudio = _splashParticles.transform.GetComponent<AudioSource>();

            _rigidbody.isKinematic = true;
            _radius = (_renderer.bounds.extents.x + _renderer.bounds.extents.y + _renderer.bounds.extents.z) / 3;

            _isDev = Array.IndexOf(_devUsers, Networking.LocalPlayer.displayName) != -1;

            _SetColor();
            if (Networking.IsOwner(gameObject))
            {
                start_position = transform.position;
                start_rotation = transform.rotation;
                RequestSerialization();
            }
            GetComponent<Collider>().enabled = true;

            tableHeight = MainScript.tableHeight.position.y;
        }

        private void PlayAudio(AudioClip clip, float strength)
        {
            if(clip != null) _audioSource.PlayOneShot(clip, Mathf.Clamp01(strength));
        }

        public override void OnDeserialization()
        {
            _pickup.pickupable = state[0] == STATE_IDLE || (state[0] == STATE_IN_CUP && !MainScript.DoAutoRespawnBalls);
            _SetColor();

            if (state[0] == STATE_IDLE)
            {
                SetStatic();
                transform.position = start_position;
                transform.rotation = start_rotation;
                startVelocityLocal = Vector3.zero;
            }
            else if (state[0] == STATE_IN_HAND)
            {
                SetStatic();
                throwIndicator.gameObject.SetActive(false);
            }
            else if (state[0] == STATE_LOCKED)
            {
                transform.position = start_position;
                transform.rotation = start_rotation;
                throwIndicator.SetPositionAndRotation(transform.position, transform.rotation);
                throwIndicator.gameObject.SetActive(true);
            }
            else if (state[0] == STATE_FYING)
            {
                transform.position = start_position;
                transform.rotation = start_rotation;
                startPositionLocal = start_position;
                startVelocityLocal = start_velocity;
                physicsSimulationStartTime = Time.time;
                SetStaticCollisions();
                throwIndicator.gameObject.SetActive(false);
            }else if(state[0] == STATE_RIMING)
            {
                rimming = state[4] * 2;
            }else if(state[0] == STATE_IN_CUP)
            {
                ThryBP_Glass cup = MainScript.GetCup(state[1], state[2], state[3]);
                if (Utilities.IsValid(cup))
                {
                    cup.colliderForThrow.enabled = false;
                    cup.colliderInside.SetActive(true);
                }
            }

            if (state[0] != localState)
            {
                localState = state[0];
                stateStartTime = Time.time;

                if(localState == STATE_IDLE || localState == STATE_FYING)
                {
                    ResetLastCupsColliders();
                }
                if(localState == STATE_IN_CUP)
                {
                    _TriggerSplash();
                }
            }
        }

        private void _TriggerSplash()
        {
            _splashParticles.transform.position = transform.position;
            _splashParticles.Emit(20);
            PlayAudio(audio_water_splash, 1);
        }

        public override void PostLateUpdate()
        {
            switch (localState)
            {
                case STATE_IN_HAND:
                    if (Networking.IsOwner(gameObject)) ThrowStrengthCheck();
                    else UpdateBallInHandPositionIfRemote();
                    break;
                case STATE_LOCKED:
                    if (Networking.IsOwner(gameObject)) ThrowStrengthCheck();
                    break;
                case STATE_FYING:
                    UpdateVelocityTracking();
                    if (_isStatic) transform.position = PositionAtTime(startPositionLocal, startVelocityLocal, Time.time - physicsSimulationStartTime);
                    // if (_isStatic) transform.position = PositionAtTime(startPositionLocal, startVelocityLocal, (Time.time - physicsSimulationStartTime) * 0.2f); // for testing
                    if (Networking.IsOwner(gameObject)) BallRespawnChecks();
                    break;
                case STATE_RIMING:
                    DoRimming();
                    break;
                case STATE_IN_CUP:
                    DoInCup();
                    break;
            }
        }

        void UpdateVelocityTracking()
        {
            _lastvelociesHead = (++_lastvelociesHead) % VELOCITY_BUFFER_LENGTH;
            _lastvelocies[_lastvelociesHead] = (transform.position - _lastPosition) / Time.deltaTime;
            _lastAngularVelocities[_lastvelociesHead] = Quaternion.Angle(transform.rotation,_lastRotation) / Time.deltaTime;
            _lastPosition = transform.position;
            _lastRotation = transform.rotation;
        }

        void ThrowStrengthCheck()
        {
            //For vr
            UpdateVelocityTracking();
            //For desktop
            if (!Input.GetMouseButton(1)) rightMouseDownStart = Time.time;
            else
            {
                throwStrengthPercent = (Time.time - rightMouseDownStart) / MAX_MOUSE_DOWN_THROW_TIME;
                _strengthIndicatorDesktop.material.SetFloat("_Strength", throwStrengthPercent);
            }
        }

        void UpdateBallInHandPositionIfRemote()
        {
            if (isRightHand)
            {
                Quaternion q = Networking.GetOwner(gameObject).GetBoneRotation(HumanBodyBones.RightHand);
                transform.SetPositionAndRotation(Networking.GetOwner(gameObject).GetBonePosition(HumanBodyBones.RightHand) + q * start_position,
                     q * start_rotation);
            }
            else
            {
                Quaternion q = Networking.GetOwner(gameObject).GetBoneRotation(HumanBodyBones.LeftHand);
                transform.SetPositionAndRotation(Networking.GetOwner(gameObject).GetBonePosition(HumanBodyBones.LeftHand) + q * start_position,
                    q * start_rotation);
            }
        }

        void BallRespawnChecks()
        {
            //respawn ball for other team if falls off table
            if (transform.position.y < respawnHeight.position.y)
            {
                Debug.Log("[ThryBP] Ball below respawn point. Respawning for other team.");
                NextTeam();
                Respawn();
            }
            if (_lastvelocies[_lastvelociesHead].sqrMagnitude > 0.5f)
            {
                timeStartBeingStill = Time.time;
            }
            else if (Time.time - timeStartBeingStill > 1.0f && Time.time - stateStartTime > 1.0f)
            {
                Debug.Log("[ThryBP] Ball idle for more than 1 second. Respawning for other team.");
                NextTeam();
                Respawn();
            }
            else if (Time.time - stateStartTime > 15f)
            {
                Debug.Log("[ThryBP] Ball Timeout. Respawning for other team.");
                NextTeam();
                Respawn();
            }
        }

        void DoRimming()
        {
            if (rimming < RIMMING_TOTAL_AGULAR_ROTATION)
            {
                ThryBP_Glass cup = MainScript.GetCup(state[1], state[2], state[3]);
                if (Utilities.IsValid(cup))
                {
                    rimming += RIMMING_SPEED * Time.deltaTime;

                    float inside = 0.9f - rimming / RIMMING_TOTAL_AGULAR_ROTATION * 0.15f;
                    float downwards = 1 - rimming / RIMMING_TOTAL_AGULAR_ROTATION * 0.3f;
                    float angle = (rimming - Mathf.Pow(rimming * 0.009f, 2)) / cup.GlobalCircumfrence * 0.4f;
                    transform.position = cup.transform.position + Vector3.up * (cup.GlobalHeight * downwards)
                        + Quaternion.Euler(0, angle, 0) * Vector3.forward * (cup.GlobalRadius * cup.transform.lossyScale.x * inside - _radius);
                }
                else
                {
                    Respawn();
                }
            }
            else
            {
                ThryBP_Glass cup = MainScript.GetCup(state[1], state[2], state[3]);
                if (Utilities.IsValid(cup))
                {
                    cup.colliderInside.SetActive(true);
                    cup.colliderForThrow.enabled = false;
                    _rigidbody.velocity = Quaternion.Euler(0, 90 + rimming, 0) * Vector3.forward * 0.3f; //in circle
                    _rigidbody.velocity += Quaternion.Euler(0, rimming + 180, 0) * Vector3.forward * 0.1f; //inwards
                    _rigidbody.velocity += Vector3.down * 0.1f; //downwards
                    SetDynamic();
                    localState = STATE_IN_CUP;
                    stateStartTime = Time.time;
                    _TriggerSplash();
                    _pickup.pickupable = !MainScript.DoAutoRespawnBalls;
                }
                else
                {
                    Respawn();
                }
            }
        }

        void DoInCup()
        {
            ThryBP_Glass cup = MainScript.GetCup(state[1], state[2], state[3]);
            if (Utilities.IsValid(cup))
            {

                //if ball is not in cup teleport ball in cup
                Collider c = cup.colliderForThrow;
                c.enabled = true;
                Vector3 cPoint = c.ClosestPoint(transform.position);
                c.enabled = false;
                Bounds b = cup.GetBounds();
                b.size = b.size += Vector3.up;
                if (Vector3.Distance(cPoint, transform.position) > 0.01f && ((_rigidbody.velocity.sqrMagnitude == 0) || (b.Contains(transform.position) == false)))
                {
                    //Debug.Log($"cPoint: {cPoint.ToString("F3")} point: {transform.position.ToString("F3")}");
                    //Debug.Log($"[ThryBP] Teleported ball into cup ({state[1]},{state[2]},{state[3]})");
                    transform.position = cup.GetBounds().center;
                }
                else if (Networking.IsOwner(gameObject))
                {
                    if (MainScript.Gamemode == ThryBP_Main.GM_NORMAL)
                    {
                        if(!_countedHit)
                        {
                            _countedHit = true;
                            MainScript.CountCupHit(cup.PlayerCupOwner.PlayerIndex, currentPlayer, state[0] == STATE_RIMING ? 1 : 0);
                            if(!MainScript.DoAutoRespawnBalls)
                            {
                                NextTeam();
                                SendCustomEventDelayedSeconds(nameof(SendItsYourTurn), 2);
                            } 
                        }
                        if (Time.time - stateStartTime > 2)
                        {
                            if(MainScript.DoAutoRespawnBalls || Time.time - stateStartTime > 20)
                            {
                                MainScript.RemoveCup(cup, currentPlayer);
                                Respawn();
                            }
                        }
                    }
                    else if (MainScript.Gamemode == ThryBP_Main.GM_KING_HILL || MainScript.Gamemode == ThryBP_Main.GM_MAYHEM)
                    {
                        if (Time.time - stateStartTime > 1)
                        {
                            MainScript.CountCupHit(cup.PlayerCupOwner.PlayerIndex, currentPlayer, state[0] == STATE_RIMING ? 1 : 0);
                            MainScript.RemoveCup(cup, currentPlayer);
                            Respawn();
                        }
                    }
                    else if (MainScript.Gamemode == ThryBP_Main.GM_BIG_PONG)
                    {
                        if (Time.time - stateStartTime > 3)
                        {
                            MainScript.CountCupHit(cup.PlayerCupOwner.PlayerIndex, currentPlayer, state[0] == STATE_RIMING ? 1 : 0);
                            Respawn();
                        }
                    }
                }
            }
            else
            {
                Respawn();
            }
        }

        void SetState(byte s)
        {
            state[0] = s;
            localState = s;
            stateStartTime = Time.time;
        }

        public void SetPositionRotation(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            start_position = position;
            start_rotation = rotation;
            RequestSerialization();
        }

        //Set ownership
        public override void OnPickup()
        {
            _countedHit = false;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            SendCustomEventDelayedFrames(nameof(OnPickupDelayed), 1);

            if (localState == STATE_IN_CUP)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                SetStatic();

                ThryBP_Glass cup = MainScript.GetCup(state[1], state[2], state[3]);
                MainScript.RemoveCup(cup, currentPlayer);
            }

            SetState(STATE_IN_HAND);
        }

        public void OnPickupAI()
        {
            _countedHit = false;
            Networking.SetOwner(Networking.LocalPlayer, gameObject);

            if (localState == STATE_IN_CUP)
            {
                _rigidbody.velocity = Vector3.zero;
                _rigidbody.angularVelocity = Vector3.zero;
                SetStatic();

                ThryBP_Glass cup = MainScript.GetCup(state[1], state[2], state[3]);
                MainScript.RemoveCup(cup, currentPlayer);
            }
            SetState(STATE_IDLE);
        }

        public void OnPickupDelayed()
        {
            SerializePositionRelativeToHand();
        }

        private void SerializePositionRelativeToHand()
        {
            SendCustomEventDelayedFrames(nameof(SerializePositionRelativeToHandConcrete), 1);
            SendCustomEventDelayedSeconds(nameof(SerializePositionRelativeToHandConcrete), 1);
        }

        public void SerializePositionRelativeToHandConcrete()
        {
            if (state[0] != STATE_IN_HAND) return;
            isRightHand = _pickup.currentHand == VRC_Pickup.PickupHand.Right;
            if (isRightHand)
            {
                start_position = transform.position - Networking.LocalPlayer.GetBonePosition(HumanBodyBones.RightHand);
                Quaternion q = Quaternion.Inverse(Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.RightHand));
                start_rotation = q * transform.rotation;
                start_position = q * start_position;
            }
            else
            {
                start_position = transform.position - Networking.LocalPlayer.GetBonePosition(HumanBodyBones.LeftHand);
                Quaternion q = Quaternion.Inverse(Networking.LocalPlayer.GetBoneRotation(HumanBodyBones.LeftHand));
                start_rotation = q * transform.rotation;
                start_position = q * start_position;
            }
            RequestSerialization();
        }


        bool _isDev;
        int _special;

        public override void OnPickupUseDown()
        {
            _special++;
        }

        public override void OnPickupUseUp()
        {
            if ((int)InputManager.GetLastUsedInputMethod() == 10) //if index, release
            {
                _pickup.Drop();
            }
        }

        Vector3 dropVelocity;
        public override void OnDrop()
        {
            //Drop velocity is avg velocity over last few frames
            dropVelocity = Vector3.zero;
            int count = 0;
            float angularAddition = 0;
            for (int i = 0; i < VELOCITY_BUFFER_LENGTH; i++)
            {
                dropVelocity += _lastvelocies[i];
                if (_lastvelocies[i] != Vector3.zero) count++;
                if (_lastAngularVelocities[i] > angularAddition) angularAddition = _lastAngularVelocities[i];
            }
            dropVelocity = dropVelocity / count;

            angularAddition = Mathf.Min(1.5f, angularAddition / 2000);
            dropVelocity = (dropVelocity.magnitude + angularAddition) * dropVelocity.normalized;

            
            SendCustomEventDelayedFrames(nameof(OnDropDelayed), 1);
        }

        private string[] _devUsers = new string[] { "Thryrallo", "Thry", "Katy" };
        public void OnDropDelayed()
        {
            if (Networking.LocalPlayer.IsUserInVR())
            {
                Debug.Log("[Thry][BP] DropVelocity: " + dropVelocity);
                //Transfer the velocity to the selected vector
                if (state[0] == STATE_LOCKED)
                {
                    transform.SetPositionAndRotation(throwIndicator.position, throwIndicator.rotation);
                    start_velocity = throwIndicator.rotation * Vector3.forward * dropVelocity.magnitude * throwVelocityMultiplierVR;
                }
                //throw normally
                else
                {
                    start_velocity = dropVelocity * throwVelocityMultiplierVR;
                }
            }
            else
            {
                float strength = Mathf.Min(MAX_THROW_STRENGTH, throwStrengthPercent * MAX_THROW_STRENGTH);
                _strengthIndicatorDesktop.material.SetFloat("_Strength", 0);
                //Transfer the velocity to the selected vector
                if (state[0] == STATE_LOCKED)
                {
                    transform.SetPositionAndRotation(throwIndicator.position, throwIndicator.rotation);
                    start_velocity = throwIndicator.rotation * Vector3.forward * strength * throwVelocityMultiplierDekstop;
                }
                //throw normally
                else
                {
                    start_velocity = transform.rotation * Vector3.forward * strength * throwVelocityMultiplierDekstop;
                }
            }

            start_position = transform.position;
            start_rotation = transform.rotation;
            startPositionLocal = start_position;
            startVelocityLocal = start_velocity;
            physicsSimulationStartTime = Time.time;

            _pickup.pickupable = false;
            SetStaticCollisions();
            throwIndicator.gameObject.SetActive(false);

            //Aim assist
            bool doFull = _isDev && (_special >= 3 || (Networking.LocalPlayer.IsUserInVR() && Input.GetAxis("Oculus_CrossPlatform_PrimaryIndexTrigger") > 0.9f));
            _special = 0;
            if (MainScript.AimAssist > 0 || doFull)
            {
                start_velocity = DoAimAssistVectoring(start_velocity, transform.position, doFull?1000:MainScript.AimAssist);
                startVelocityLocal = start_velocity;
            }
            MainScript.LastAimAssistPublsher.SetFloat(MainScript.AimAssist);

            SetState(STATE_FYING);
            RequestSerialization();

            MainScript.CountThrow(currentPlayer);
        }

        public void ShootAI(float skill)
        {
            // calculate rough velocity to throw ball in that direction
            start_velocity = MainScript.GetAIThrowVector(transform.position, currentPlayer) * MAX_THROW_STRENGTH * throwVelocityMultiplierDekstop;

            start_position = transform.position;
            start_rotation = transform.rotation;
            startPositionLocal = start_position;
            startVelocityLocal = start_velocity;
            physicsSimulationStartTime = Time.time;

            _pickup.pickupable = false;
            SetStaticCollisions();
            throwIndicator.gameObject.SetActive(false);

            start_velocity = DoAIVectoring(start_velocity, transform.position, skill);
            startVelocityLocal = start_velocity;

            SetState(STATE_FYING);
            RequestSerialization();
        }

        bool _isStatic = true;
        void SetStatic()
        {
            _rigidbody.isKinematic = true;
            _isStatic = true;
        }

        void SetStaticCollisions()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.drag = float.MaxValue;
            _rigidbody.angularDrag = float.MaxValue;
            _rigidbody.useGravity = false;
            _isStatic = true;
        }

        void SetDynamic()
        {
            _rigidbody.isKinematic = false;
            _rigidbody.drag = 0;
            _rigidbody.angularDrag = 0.5f;
            _rigidbody.useGravity = true;
            _isStatic = false;
        }

        //=========Prediction==========

        private Vector3 PositionAtTime(Vector3 start, Vector3 startVelocity, float time)
        {
            float y = start.y + Physics.gravity.y * time * time / 2 + startVelocity.y * time;
            start = start + startVelocity * time;
            start.y = y;
            return start;
        }

        private Vector3 VelocityAtTime(Vector3 startVelocity, float time)
        {
            startVelocity.y = Physics.gravity.y * time + startVelocity.y;
            return startVelocity;
        }

        private Vector3 DoAimAssistVectoring(Vector3 velocity, Vector3 position, float strength)
        {
            if (strength == 0) return velocity;
            //Get Point where ball will hit table
            float predictedTableHitTime = PredictTableHitTime(velocity, position, tableHeight);
            Vector3 predictionTableHit = GetTableHitPosition(velocity, position, tableHeight, predictedTableHitTime);
            ThryBP_Glass cup = GetClosestGlassToPredictedTableCollision(predictionTableHit);
            if (cup == null) return velocity;
            Vector3 cupOpening = cup.transform.position + Vector3.up * (cup.GlobalHeight + _radius);
            Vector3 hitOnCupPlane = PredictTableHit(velocity, position, cupOpening.y);
            // If hit not in range, check if ball will bounce close to cup
            if (Vector3.Distance(cupOpening, hitOnCupPlane) > strength)
            {
                // Calculate the position of the first bounce
                Vector3 reflectVel = Vector3.Reflect(VelocityAtTime(velocity, predictedTableHitTime), Vector3.up) * BOUND_DRAG;
                float predictedCupHeightTime2 = PredictTableHitTime(reflectVel, predictionTableHit, cupOpening.y);
                Vector3 predictionCupHeightPosition2 = GetTableHitPosition(reflectVel, predictionTableHit, cupOpening.y, predictedCupHeightTime2);
                // Get new closest cup
                cup = GetClosestGlassToPredictedTableCollision(predictionCupHeightPosition2);
                cupOpening = cup.transform.position + Vector3.up * (cup.GlobalHeight + _radius);
                // If first bounce is close to cup, aim for that
                if (Vector3.Distance(cupOpening, predictionCupHeightPosition2) < strength)
                {
                    Vector3 correctDirection = OptimalVectorJustChangeY(velocity, position, cup);
                    // Adjust xz velocity to hit cup
                    float xzThrowDistance = XZDistance(position, predictionCupHeightPosition2);
                    float targetXZDistance = XZDistance(position, cupOpening);
                    float multiplier = targetXZDistance / xzThrowDistance;
                    correctDirection.x *= multiplier;
                    correctDirection.z *= multiplier;
                    return correctDirection;
                }
                //Check if ball close to going past, if so adjust angle for bounce aim assist
                float distance = Vector3.Cross(new Vector3(velocity.x,0, velocity.z).normalized, new Vector3(cupOpening.x,0,cupOpening.z) - new Vector3(position.x, 0, position.z)).magnitude;
                if (distance < strength) return OptimalVectorJustChangeY(velocity, position, cup);
                return velocity;
            }
            return OptimalVectorChangeStrength(velocity, position, cup);
        }

        private float XZDistance(Vector3 a, Vector3 b)
        {
            a.y = 0;
            b.y = 0;
            return Vector3.Distance(a, b);
        }

        private Vector3 DoAIVectoring(Vector3 velocity, Vector3 position, float skill)
        {
            if (skill == 0) return velocity;
            //Get Point where ball will hit table
            Vector3 predictionTableHit = PredictTableHit(velocity, position, tableHeight);
            ThryBP_Glass cup = GetClosestGlassToPredictedTableCollision(predictionTableHit);
            if (cup == null) return velocity;
            Vector3 optimalVector = OptimalVectorChangeStrength(velocity, position, cup);

            float lerping = UnityEngine.Random.Range(0f, 1f);
            if (lerping < skill) lerping = 1; // hit
            else lerping = (1 - lerping) * 0.85f; // miss
            velocity = Vector3.Lerp(velocity, optimalVector, lerping);

            return velocity;
        }

        private Vector3 OptimalVectorChangeStrength(Vector3 velocity, Vector3 position, ThryBP_Glass cup)
        {
            Vector3 cupOpening = cup.transform.position + Vector3.up * (cup.GlobalHeight + _radius);
            // Debug.DrawLine(position, cupOpening, Color.red, 10);
            Vector3 horizonzalVector = cupOpening - position;
            horizonzalVector.y = 0;
            Vector3 horizonzalVelocity = velocity;
            horizonzalVelocity.y = 0;
            //https://www.youtube.com/watch?v=mOYJKv22qeQ&list=PLX2gX-ftPVXUUlf-9Eo_6ut4kP6wKaSWh&index=7&ab_channel=MichelvanBiezen
            // d = v0 * cos(w) * t
            // => t = d / (v0 * cos(w))
            // y = y0 + v0 * sin(w) * t - 0.5 * g * t2
            // y = y0 + v0 * sin(w) * ( d / (v0 * cos(w)) ) - 0.5 * g * ( d / (v0 * cos(w)) )2
            // y = y0 + tan(w) * d - 0.5 * g * ( d / (v0 * cos(w)) )2
            // y0 - y + tan(w) * d = 0.5 * g * ( d / (v0 * cos(w)) )2
            // 2(y0 - y + tan(w) * d) / g = ( d / (v0 * cos(w)) )2
            // sqrt( 2(y0 - y + tan(w) * d) / g) = d / (v0 * cos(w))
            // v0 * cos(w) = d / sqrt( 2(y0 - y + tan(w) * d) / g)
            // v0 = d / sqrt( 2(y0 - y + tan(w) * d) / g) / cos(w)
            float d = horizonzalVector.magnitude;
            float y0 = position.y;
            float y = cupOpening.y;
            float w = Mathf.Deg2Rad * Vector3.Angle(horizonzalVelocity, velocity);
            float g = -Physics.gravity.y;

            float v0 = d / Mathf.Sqrt(2 * (y0 - y + Mathf.Tan(w) * d) / g) / Mathf.Cos(w);
            if (float.IsNaN(v0)) return velocity;

            Vector3 newDirection = horizonzalVector.normalized * horizonzalVelocity.magnitude;
            newDirection.y = velocity.y;

            return newDirection.normalized * v0;
        }

        private Vector3 OptimalVectorJustChangeY(Vector3 velocity, Vector3 position, ThryBP_Glass cup)
        {
            Vector3 horizonzalVector = cup.transform.position - position;
            horizonzalVector.y = 0;
            Vector3 horizonzalVelocity = velocity;
            horizonzalVelocity.y = 0;
            Vector3 newDirection = horizonzalVector.normalized * horizonzalVelocity.magnitude;
            newDirection.y = velocity.y;
            return newDirection;
        }

        private Vector3 OptimalVectorChangeAngle(Vector3 velocity, Vector3 position, float tableHeight, ThryBP_Glass aimedCup)
        {
            //Calculate trajectory to that cup
            Vector3 horizonzalDistance = aimedCup.transform.position - position;
            horizonzalDistance.y = 0;
            //https://www.youtube.com/watch?v=bqYtNrhdDAY&ab_channel=MichelvanBiezen
            //https://www.youtube.com/watch?v=pQ23Eb-bXvQ&ab_channel=MichelvanBiezen
            float g = -Physics.gravity.y;
            float v2 = velocity.sqrMagnitude;
            float x = horizonzalDistance.magnitude;
            float h = position.y - (tableHeight + aimedCup.GlobalHeight);
            float phi = Mathf.Atan(x / h);
            float theta = (Mathf.Acos(((g * x * x / v2) - h) / Mathf.Sqrt(h * h + x * x)) + phi) / 2;

            Vector3 newVel = (horizonzalDistance.normalized * Mathf.Cos(theta) + Vector3.up * Mathf.Sin(theta)) * velocity.magnitude;
            if (float.IsNaN(theta))
            {
                //cant reacht cup with current velocity
                //just aim in it's direction instead
                Vector3 horizontalVel = velocity;
                horizontalVel.y = 0;
                newVel = horizonzalDistance.normalized * horizontalVel.magnitude;
                newVel.y = velocity.y;
            }
            return newVel;
        }

        private ThryBP_Glass GetClosestGlassToPredictedTableCollision(Vector3 prediction)
        {
            //Get Cup closest to that point
            ThryBP_Glass aimedCup = null;
            float closestDistance = float.MaxValue;
            bool disallowOwnCups = MainScript.ShouldAimAssistAimForOwnCups() == false;
            bool disallowOwnSide = MainScript.ShouldAimAssistAimAtOwnSide() == false;
            for (int p = 0; p < MainScript.playerCountSlider.local_float; p++)
            {
                float d = 0;
                int length = MainScript.players[p].cups.ActiveCupsCount;
                ThryBP_Glass[] cups = MainScript.players[p].cups.ActiveCupsList;
                for(int i = 0; i < length; i++)
                {
                    ThryBP_Glass cup = cups[i];
                    if (cup == null) continue;
                    if (disallowOwnCups && cup.PlayerCupOwner.PlayerIndex == currentPlayer) continue;
                    if (disallowOwnSide && cup.PlayerAnchorSide.PlayerIndex == currentPlayer) continue;
                    d = Vector3.Distance(cup.transform.position, prediction);
                    if (d < closestDistance)
                    {
                        closestDistance = d;
                        aimedCup = cup;
                    }
                }
            }
            return aimedCup;
        }

        private Vector3 PredictTableHit(Vector3 orignVelocity, Vector3 orignPosition, float tableHeight)
        {
            float t = PredictTableHitTime(orignVelocity, orignPosition, tableHeight);
            return GetTableHitPosition(orignVelocity, orignPosition, tableHeight, t);
        }

        private Vector3 GetTableHitPosition(Vector3 orignVelocity, Vector3 orignPosition, float tableHeight, float t)
        {
            return new Vector3(orignPosition.x + orignVelocity.x * t, tableHeight + _radius, orignPosition.z + orignVelocity.z * t);
        }

        private float PredictTableHitTime(Vector3 orignVelocity, Vector3 orignPosition, float tableHeight)
        {
            // Solve quadratic equation: c0*x^2 + c1*x + c2. 
            float c0 = Physics.gravity.y / 2;
            float c1 = orignVelocity.y;
            float c2 = orignPosition.y - (tableHeight + _radius);
            float[] quadraticOut = new float[2];
            int quadratic = SolveQuadric(c0, c1, c2, quadraticOut);
            float t = 0;
            if (quadratic == 1) t = quadraticOut[0];
            else if(quadratic == 2 && quadraticOut[0] > 0 && quadraticOut[0] > quadraticOut[1]) t = quadraticOut[0];
            else if(quadratic == 2 && quadraticOut[1] > 0) t = quadraticOut[1];
            return t;
        }

        public int SolveQuadric(float c0, float c1, float c2, float[] s)
        {
            float p, q, D;

            /* normal form: x^2 + px + q = 0 */
            p = c1 / (2 * c0);
            q = c2 / c0;

            D = p * p - q;

            if (D == 0)
            {
                s[0] = -p;
                return 1;
            }
            else if (D < 0)
            {
                return 0;
            }
            else /* if (D > 0) */
            {
                float sqrt_D = Mathf.Sqrt(D);

                s[0] = sqrt_D - p;
                s[1] = -sqrt_D - p;
                return 2;
            }
        }

        //=========Collision Code======

        private void ResetLastCupsColliders()
        {
            ThryBP_Glass cup = MainScript.GetCup(state[1], state[2], state[3]);
            if (Utilities.IsValid(cup))
            {
                cup.colliderForThrow.enabled = true;
                cup.colliderInside.SetActive(false);
            }
        }

        public void Respawn()
        {
            if(Networking.IsOwner(gameObject)) MainScript.UpdateLocalPlayerAdaptiveAimAssist();
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
            SetStatic();
            SetState(STATE_IDLE);
            transform.position = MainScript.GetBallSpawn(currentPlayer).position;
            transform.rotation = MainScript.GetBallSpawn(currentPlayer).rotation;
            start_position = transform.position;
            start_rotation = transform.rotation;
            _pickup.pickupable = true;
            MainScript.players[currentPlayer].ItsYourTurn(this);
            ResetLastCupsColliders();
            RequestSerialization();
        }

        public void SendItsYourTurn()
        {
            MainScript.players[currentPlayer].ItsYourTurn(this);
        }

        private void NextTeam()
        {
            currentPlayer = MainScript.GetNextPlayer(currentPlayer);
            if(currentPlayer >= MainScript.playerCountSlider.local_float)
            {
                currentPlayer = 0;
            }
            _SetColor();
            RequestSerialization();
        }

        public void _SetColor()
        {
            _renderer.material.color = MainScript.GetPlayerColor(currentPlayer);
            MainScript.SetActivePlayerColor(_renderer.material.color);
        }

        const float COLLISION_MAXIMUM_DISTANCE_FROM_CENTER = 0.95f;

        private void BallGlassCollision(Collision collision, ThryBP_Glass hitGlass)
        {
            Bounds b = hitGlass.GetBounds();
            bool isCollisionOnTop = (b.max.y < transform.position.y);

            SetDynamic();

            if (isCollisionOnTop)
            {
                Vector3 p1 = transform.position;
                p1.y = 0;
                Vector3 p2 = hitGlass.transform.position;
                p2.y = 0;
                float distanceFromCenter = Vector3.Distance(p1, p2) / hitGlass.GlobalRadius;

                if (distanceFromCenter < COLLISION_MAXIMUM_DISTANCE_FROM_CENTER)
                {
                    float angle = Vector3.Angle(_lastvelocies[_lastvelociesHead], Vector3.down);
                    //bool tripOnEdge = distanceFromCenter > Random.Range(0, 0.85f);
                    bool tripOnEdge = distanceFromCenter > UnityEngine.Random.Range(0.4f,0.7f) && angle > UnityEngine.Random.Range(45, 80); //if it lands really flat chances are higher for it to ride edge

                    if (tripOnEdge)
                    {
                        SetState(STATE_RIMING);
                        state[1] = (byte)hitGlass.PlayerAnchorSide.PlayerIndex;
                        state[2] = (byte)hitGlass.Row;
                        state[3] = (byte)hitGlass.Column;
                        rimming = Vector3.Angle(hitGlass.transform.rotation * Vector3.forward, p2 - p1);
                        if ((p2 - p1).x > 0) rimming = 360-rimming;
                        rimming = rimming % 360;
                        state[4] = (byte)(rimming / 2);
                        state[5] = (byte)(distanceFromCenter * 100);
                    }
                    else
                    {
                        hitGlass.colliderForThrow.enabled = false;
                        hitGlass.colliderInside.SetActive(true);
                        _pickup.pickupable = !MainScript.DoAutoRespawnBalls;
                        SetState(STATE_IN_CUP);
                        state[1] = (byte)hitGlass.PlayerAnchorSide.PlayerIndex;
                        state[2] = (byte)hitGlass.Row;
                        state[3] = (byte)hitGlass.Column;

                        _TriggerSplash();
                        _rigidbody.velocity = Vector3.down * startVelocityLocal.magnitude;
                    }
                    RequestSerialization();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider == null || collision.collider.gameObject == null) return;

            bool hasHitGlass = collision.collider.gameObject.GetComponent<ThryBP_Glass>() != null;
            if (hasHitGlass)
            {
                PlayAudio(audio_collision_glass, startVelocityLocal.sqrMagnitude / 10);
            }
            else
            {
                PlayAudio(audio_collision_table, startVelocityLocal.sqrMagnitude / 10);
            }

            startPositionLocal = transform.position;
            startVelocityLocal = Vector3.Reflect(VelocityAtTime(startVelocityLocal, Time.time - physicsSimulationStartTime), Vector3.up) * BOUND_DRAG;
            physicsSimulationStartTime = Time.time;

            if (Networking.IsOwner(gameObject))
            {
                if(state[0] == STATE_FYING)
                {
                    if (hasHitGlass)
                    {
                        ThryBP_Glass hitGlass = collision.collider.gameObject.GetComponent<ThryBP_Glass>();
                        //check if collision with own glass and if friendly fire is turned on
                        if (MainScript.AllowCollision(currentPlayer, hitGlass.PlayerCupOwner, hitGlass.PlayerAnchorSide))
                        {
                            BallGlassCollision(collision, hitGlass);
                        }
                    }
                    //none beer pong object => miss
                    else if (collision.collider.gameObject.name.StartsWith("[ThryBP]") == false)
                    {
                        Debug.Log("[ThryBP] Ball hit non game object. Respawning for other team.");
                        NextTeam();
                        Respawn();
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (collider == null || collider.gameObject == null) return;

            if (localState == STATE_RIMING && collider.gameObject.GetComponent<HandCollider>() != null)
            {
                HandCollider hand = collider.gameObject.GetComponent<HandCollider>();
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                SetState(STATE_FYING);
                _rigidbody.velocity = hand._velocity;

                start_velocity = _rigidbody.velocity;
                start_position = transform.position;
                start_rotation = transform.rotation;
                RequestSerialization();
            }
        }
    }
}