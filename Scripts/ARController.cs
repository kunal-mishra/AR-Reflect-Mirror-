namespace ARMirror
{
    using System.Collections.Generic;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.Rendering;

    public class ARController : MonoBehaviour
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab for tracking and visualizing detected planes.
        /// </summary>
        public GameObject TrackedPlanePrefab;

        public GameObject MirrorPrefab;

        /// <summary>
        /// A gameobject parenting UI for displaying the "searching for planes" snackbar.
        /// </summary>
        public GameObject SearchingForPlaneUI;

        /// <summary>
        /// A list to hold new planes ARCore began tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_NewPlanes = new List<TrackedPlane>();

        /// <summary>
        /// A list to hold all planes ARCore is tracking in the current frame. This object is used across
        /// the application to avoid per-frame allocations.
        /// </summary>
        private List<TrackedPlane> m_AllPlanes = new List<TrackedPlane>();

        /// <summary>
        /// True if the app is in the process of quitting due to an ARCore connection error, otherwise false.
        /// </summary>
        private bool m_IsQuitting = false;

        private GameObject placeholder;

        private bool mayChangePlaceholderPosition = true;

        public bool MayChangePlaceholderPosition { get { return mayChangePlaceholderPosition; } set { mayChangePlaceholderPosition = value; } }

        private bool showPlanes = true;

        /// <summary>
        /// If true, tracked planes will be visualized
        /// </summary>
        public bool ShowPlanes
        {
            get
            {
                return showPlanes;
            }
            set
            {
                if (showPlanes != value)
                {
                    if (value)
                    {
                        CreatePlaneVisualization();
                    }
                    else
                    {
                        DestroyPlaneVisualization();
                    }
                }
                showPlanes = value;
            }
        }

        private List<GameObject> trackedPlaneVisualizers = new List<GameObject>();

        // private GameObject mirrorObject;

        private void DestroyPlaneVisualization()
        {
            foreach (var plane in trackedPlaneVisualizers)
            {
                Destroy(plane);
            }
            trackedPlaneVisualizers.Clear();
        }

        private void CreatePlaneVisualization()
        {
            DestroyPlaneVisualization();
            Session.GetTrackables<TrackedPlane>(m_AllPlanes);
            VisualizePlanes(m_AllPlanes);
        }

        private void VisualizePlanes(List<TrackedPlane> planes)
        {
            foreach (var plane in planes)
            {
                // Instantiate a plane visualization prefab and set it to track the plane. The transform is set to
                // the origin with an identity rotation since the mesh for our prefab is updated in Unity World
                // coordinates.
                GameObject planeObject = Instantiate(TrackedPlanePrefab, Vector3.zero, Quaternion.identity,
                    transform);
                planeObject.GetComponent<TrackedPlaneVisualizer>().Initialize(plane);
                trackedPlaneVisualizers.Add(planeObject);

            }
        }

        void Start()
        {
            placeholder = GameObject.Find("MirrorPlaceholder");
            placeholder.transform.localScale = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// The Unity Update() method.
        /// </summary>
        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                Application.Quit();
            }

            _QuitOnConnectionErrors();

            // Check that motion tracking is tracking.
            if (Session.Status != SessionStatus.Tracking)
            {
                const int lostTrackingSleepTimeout = 15;
                Screen.sleepTimeout = lostTrackingSleepTimeout;
                if (!m_IsQuitting && Session.Status.IsValid() && SearchingForPlaneUI != null)
                {
                    SearchingForPlaneUI.SetActive(true);
                }

                return;
            }

            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            // Iterate over planes found in this frame and instantiate corresponding GameObjects to visualize them.
            Session.GetTrackables<TrackedPlane>(m_NewPlanes, TrackableQueryFilter.New);
            if (showPlanes)
            {
                VisualizePlanes(m_NewPlanes);
            }

            // Disable the snackbar UI when no planes are valid.
            Session.GetTrackables<TrackedPlane>(m_AllPlanes);
            bool showSearchingUI = true;
            for (int i = 0; i < m_AllPlanes.Count; i++)
            {
                if (m_AllPlanes[i].TrackingState == TrackingState.Tracking)
                {
                    showSearchingUI = false;
                    break;
                }
            }

            if (SearchingForPlaneUI != null)
            {
                SearchingForPlaneUI.SetActive(showSearchingUI);
            }

            if (mayChangePlaceholderPosition)
            {
                RaycastAgainstLocation(Screen.width / 2, Screen.height / 2);
            }
        }

        private void RaycastAgainstLocation(float x, float y)
        {
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

            if (Frame.Raycast(x, y, raycastFilter, out hit))
            {
                // Create an anchor to allow ARCore to track the hitpoint as understanding of the physical
                // world evolves.
                var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                if (placeholder.transform.localScale == Vector3.zero)
                {
                    placeholder.transform.localScale = new Vector3(1, 1, 1);
                }
                placeholder.transform.position = hit.Pose.position;

                Vector3 cameraPositionSameY = FirstPersonCamera.transform.position;
                cameraPositionSameY.y = hit.Pose.position.y;
                placeholder.transform.LookAt(cameraPositionSameY, placeholder.transform.up);

                // Make Andy model a child of the anchor.
                placeholder.transform.parent = anchor.transform;
                placeholder.GetComponentInChildren<MirrorController>().Play();
            }
        }

        /// <summary>
        /// Quit the application if there was a connection error for the ARCore session.
        /// </summary>
        private void _QuitOnConnectionErrors()
        {
            if (m_IsQuitting)
            {
                return;
            }

            // Quit if ARCore was unable to connect and give Unity some time for the toast to appear.
            if (Session.Status == SessionStatus.ErrorPermissionNotGranted)
            {
                _ShowAndroidToastMessage("Camera permission is needed to run this application.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
            else if (Session.Status.IsError())
            {
                _ShowAndroidToastMessage("ARCore encountered a problem connecting.  Please start the app again.");
                m_IsQuitting = true;
                Invoke("_DoQuit", 0.5f);
            }
        }

        /// <summary>
        /// Actually quit the application.
        /// </summary>
        private void _DoQuit()
        {
            Application.Quit();
        }

        /// <summary>
        /// Show an Android toast message.
        /// </summary>
        /// <param name="message">Message string to show in the toast.</param>
        private void _ShowAndroidToastMessage(string message)
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            if (unityActivity != null)
            {
                AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
                unityActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
                {
                    AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", unityActivity,
                        message, 0);
                    toastObject.Call("show");
                }));
            }
        }
    }
}
