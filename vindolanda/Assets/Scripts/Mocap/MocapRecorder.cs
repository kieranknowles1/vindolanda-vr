using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace Vindolanda.Mocap
{
    /// <summary>
    /// Record a motion-captured animation. Editor only
    /// </summary>
    public class MocapRecorder : MonoBehaviour
    {
        public XRHandTrackingEvents leftHand;
        public NearFarInteractor leftInteractor;
        public XRHandTrackingEvents rightHand;
        public NearFarInteractor rightInteractor;
        public Transform head;

        public const string outputPath = "Assets/Animations/Mocap/";
        public string outputFileName = "output";
        public string AssetPath => $"{outputPath}/{outputFileName}.asset";
        Clip output;

        public InputActionReference startRecording;
        public bool recordingActive = false;
        float startTime = float.NaN;

        Clip.HandState? leftState;
        Clip.HandState? rightState;

#if UNITY_EDITOR
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            leftHand.jointsUpdated.AddListener(OnPoseUpdated);
            rightHand.jointsUpdated.AddListener(OnPoseUpdated);

            startRecording.action.performed += OnStartPressed;
        }

        void OnStartPressed(InputAction.CallbackContext _)
        {
            recordingActive = !recordingActive;

            if (recordingActive)
            {
                output = ScriptableObject.CreateInstance<Clip>();
                startTime = Time.realtimeSinceStartup;

                GameConstants.Instance.Player.Subtitles.Show("System", "Recording");
            }
            else
            {
                AssetDatabase.CreateAsset(output, AssetPath);
                AssetDatabase.SaveAssetIfDirty(output);
                output = null;

                GameConstants.Instance.Player.Subtitles.Hide();
            }
        }

        void OnPoseUpdated(XRHandJointsUpdatedEventArgs evnt)
        {
            float GetFingerValue(XRHandFingerID id)
            {
                var shape = evnt.hand.CalculateFingerShape(id, XRFingerShapeTypes.FullCurl);
                shape.TryGetFullCurl(out var result);
                return result;
            }
            var hand = evnt.hand.handedness == Handedness.Left ? leftHand : rightHand;
            var interactor = evnt.hand.handedness == Handedness.Left ? leftInteractor : rightInteractor;

            Clip.HandState state = new()
            {
                transform = new()
                {
                    position = evnt.hand.rootPose.position,
                    rotation = evnt.hand.rootPose.rotation,
                },
                thumb = GetFingerValue(XRHandFingerID.Thumb),
                index = GetFingerValue(XRHandFingerID.Index),
                middle = GetFingerValue(XRHandFingerID.Middle),
                ring = GetFingerValue(XRHandFingerID.Ring),
                pinky = GetFingerValue(XRHandFingerID.Little),
                hasItem = interactor.interactablesSelected.Count > 0,
            };

            if (hand == leftHand)
                leftState = state;
            else
                rightState = state;
        }

        private void FixedUpdate()
        {
            if (!recordingActive) return;

            // Don't add a keyframe if we have no data
            if (leftState == null || rightState == null) return;

            output.keyframes.Add(new Clip.Keyframe()
            {
                startTime = Time.realtimeSinceStartup - startTime,
                leftHand = leftState.Value,
                rightHand = rightState.Value,
                head = new()
                {
                    position = head.localPosition,
                    rotation = head.localRotation,
                }
            });

            // Don't reuse keyframes
            leftState = null; rightState = null;
        }
#endif
    }
}