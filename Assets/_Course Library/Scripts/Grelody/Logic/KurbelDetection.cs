using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR.Hands.Gestures;

namespace UnityEngine.XR.Hands.Samples.GestureSample
{
    /// <summary>
    /// A gesture that detects when a hand is held in a static shape and is making a cranking motion.
    /// </summary>
    public class KurbelDetection : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The hand tracking events component to subscribe to receive updated joint data to be used for gesture detection.")]
        XRHandTrackingEvents m_HandTrackingEvents;

        [SerializeField]
        [Tooltip("The hand shape or pose that must be detected for the gesture to be performed.")]
        ScriptableObject m_HandShapeOrPose;

        [SerializeField]
        [Tooltip("The target Transform to user for target conditions in the hand shape or pose.")]
        Transform m_TargetTransform;

        [SerializeField]
        [Tooltip("Threshold to adjust the sensitivity of the cranking motion.")]
        float m_CircularMotionThreshold;

        [SerializeField]
        [Tooltip("The degree of movement that is needed to detect the hand movement.")]
        float m_DegreesToRotate;

        [SerializeField]
        [Tooltip("The minimum amount of time the hand must be held in the required shape and orientation for the gesture to be performed.")]
        float m_MinimumHoldTime = 0.2f;

        [SerializeField]
        [Tooltip("The interval at which the gesture detection is performed.")]
        float m_GestureDetectionInterval = 0.1f;

        [SerializeField]
        [Tooltip("The static gestures associated with this gestures handedness.")]
        StaticHandGesture[] m_StaticGestures;

        XRHandShape m_HandShape;
        XRHandPose m_HandPose;
        bool m_WasDetected;
        bool m_PerformedTriggered;
        float m_TimeOfLastConditionCheck;
        float m_HoldStartTime;

        private bool melodyInProgress = false;
        private Vector3 previousHandPosition;
        private float totalRotation = 0f;
        private bool isCranking = false;
        
        private XRHand trackedHand;

        private MelodyChordTest melodyChordTest;

        /// <summary>
        /// The hand tracking events component to subscribe to receive updated joint data to be used for gesture detection.
        /// </summary>
        public XRHandTrackingEvents handTrackingEvents
        {
            get => m_HandTrackingEvents;
            set => m_HandTrackingEvents = value;
        }

        /// <summary>
        /// The hand shape or pose that must be detected for the gesture to be performed.
        /// </summary>
        public ScriptableObject handShapeOrPose
        {
            get => m_HandShapeOrPose;
            set => m_HandShapeOrPose = value;
        }

        /// <summary>
        /// The target Transform to user for target conditions in the hand shape or pose.
        /// </summary>
        public Transform targetTransform
        {
            get => m_TargetTransform;
            set => m_TargetTransform = value;
        }

        /// <summary>
        /// The sensitivity of the cranking motion.
        /// </summary>
        public float circularMotionThreshold
        {
            get => m_CircularMotionThreshold;
            set => m_CircularMotionThreshold = value;
        }

        /// <summary>
        /// The needed degree of the movement to detect the movement
        /// </summary>
        public float degreesToRotate
        {
            get => m_DegreesToRotate;
            set => m_DegreesToRotate = value;
        }

        /// <summary>
        /// The minimum amount of time the hand must be held in the required shape and orientation for the gesture to be performed.
        /// </summary>
        public float minimumHoldTime
        {
            get => m_MinimumHoldTime;
            set => m_MinimumHoldTime = value;
        }

        /// <summary>
        /// The interval at which the gesture detection is performed.
        /// </summary>
        public float gestureDetectionInterval
        {
            get => m_GestureDetectionInterval;
            set => m_GestureDetectionInterval = value;
        }

        void Start()
        {
            melodyChordTest = (MelodyChordTest)FindFirstObjectByType<MelodyChordTest>();
        }

        void Awake()
        {
            melodyChordTest = (MelodyChordTest)FindFirstObjectByType<MelodyChordTest>();
        }

        void OnEnable()
        {
            m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);

            m_HandShape = m_HandShapeOrPose as XRHandShape;
            m_HandPose = m_HandShapeOrPose as XRHandPose;
            if (m_HandPose != null && m_HandPose.relativeOrientation != null)
                m_HandPose.relativeOrientation.targetTransform = m_TargetTransform;
        }

        void OnDisable() => m_HandTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

        void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
        {
            if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_TimeOfLastConditionCheck + m_GestureDetectionInterval) return;

            this.trackedHand = eventArgs.hand;
            var detected =
                m_HandTrackingEvents.handIsTracked &&
                m_HandShape != null && m_HandShape.CheckConditions(eventArgs) ||
                m_HandPose != null && m_HandPose.CheckConditions(eventArgs);

            if (!m_WasDetected && detected)
            {
                m_HoldStartTime = Time.timeSinceLevelLoad;
            }
            else if (m_WasDetected && !detected)
            {
                m_PerformedTriggered = false;
                melodyChordTest.PauseMusic();
                //m_GestureEnded?.Invoke();
            }

            m_WasDetected = detected;

            if (!m_PerformedTriggered && detected)
            {
                var holdTimer = Time.timeSinceLevelLoad - m_HoldStartTime;
                if (holdTimer > m_MinimumHoldTime)
                {
                    // if a melody is already being created then continue this melody
                    if (melodyChordTest.GetMelodyInProgress()) {
                        CheckForCrank();
                    }
                    // if no melody is currently being created, start a new melody
                    else {
                        melodyChordTest.StartMusic();
                    }
                    //m_GesturePerformed?.Invoke();
                    //m_PerformedTriggered = true;
                }
            }

            m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad;
        }

        private Vector3 GetRightHandPosition()
        {
            if (m_HandTrackingEvents != null && m_HandTrackingEvents.handIsTracked)
    {
                return this.trackedHand.rootPose.position;
            }

            // Return a default value if tracking fails
            return Vector3.zero;
        }

        void PerformCrankAction()
        {
            Debug.Log("Cranking Action Activated!");

            melodyChordTest.ContinueMusic();

            /*if (crankTarget != null)
            {
                crankTarget.Rotate(Vector3.up * 5f); // Example: Rotates the object
            }*/
        }

        void CheckForCrank()
        {
            Vector3 currentHandPosition = GetRightHandPosition();
            Vector3 movement = currentHandPosition - previousHandPosition;

            if (movement.magnitude > 0.01f) // Ensure movement is happening
            {
                Vector3 handPlaneNormal = Vector3.up; // Define crank plane
                Vector3 projectedMovement = Vector3.ProjectOnPlane(movement, handPlaneNormal);

                // Check rotational movement
                Vector3 handToPrevious = previousHandPosition - targetTransform.position;
                Vector3 handToCurrent = currentHandPosition - targetTransform.position;

                float angle = Vector3.SignedAngle(handToPrevious, handToCurrent, handPlaneNormal);

                if (Mathf.Abs(angle) > circularMotionThreshold) // Ignore small noise
                {
                    totalRotation += angle;
                    if (Mathf.Abs(totalRotation) >= degreesToRotate) // Movement of certain degree to see if the movement gets detected or is too small
                    {
                        isCranking = true;
                        totalRotation = 0f; // Reset counter
                    }
                    else {
                        isCranking = false;
                    }
                }
            }

            previousHandPosition = currentHandPosition;

            if (isCranking)
            {
                PerformCrankAction();
            }
            else
            {
                melodyChordTest.PauseMusic();
            }
        }

    }
}
