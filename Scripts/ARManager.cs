using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ARMirror
{
    public class ARManager : MonoBehaviour
    {

        [SerializeField]
        private float startScale;
        private float currentScale = 0;
        public float CurrentScale { get { return currentScale == 0 ? startScale : currentScale; } }

        public enum State { PlaceObject, EditObject, None }

        private State state = State.PlaceObject;

        public State CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                if (arController != null && state != value)
                {
                    switch (value)
                    {
                        case State.PlaceObject:
                            arController.MayChangePlaceholderPosition = true;
                            arController.ShowPlanes = true;
                            break;
                        case State.None:
                        case State.EditObject:
                            arController.MayChangePlaceholderPosition = false;
                            arController.ShowPlanes = false;
                            break;
                    }

                    objStartPos = placeholder.transform.position;
                    isTouching = false;

                    state = value;
                }
            }
        }

        private Vector2 touch1StartPos;
        private Vector2 touch2StartPos;
        private Vector3 objStartPos;
        private Vector3 objStartScale;
        private float objStartRotation;
        private float startAngle;
        [SerializeField]
        private float moveFactor;
        [SerializeField]
        private float scaleFactor;
        [SerializeField]
        private float rotationFactor;

        private GameObject placeholder;

        private ARController arController;

        private bool mayPlace = true;

        private bool hasPlaced = false;

        private bool isTouching = false;

        // Use this for initialization
        void Start()
        {
            placeholder = GameObject.Find("MirrorPlaceholder");
            objStartPos = placeholder.transform.position;
            arController = GameObject.Find("ARController").GetComponent<ARController>();
        }

        // Update is called once per frame
        void Update()
        {
            if (hasPlaced)
            {
                if (Input.touchCount == 1)
                {
                    if (mayPlace)
                    {
                        CurrentState = State.PlaceObject;
                    }
                }
                else if (Input.touchCount >= 2)
                {
                    CurrentState = State.EditObject;
                }
                else
                {
                    CurrentState = State.None;
                }
                switch (state)
                {
                    case State.EditObject:
                        EditObject();
                        break;
                }
            }
            else
            {
                PlaceObject();
            }
        }

        private void PlaceObject()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        isTouching = true;
                        break;
                    case TouchPhase.Ended:
                        if (isTouching)
                        {
                            isTouching = false;
                            if (placeholder.transform.position != objStartPos)
                            {
                                hasPlaced = true;
                            }
                        }
                        break;
                }
            }
        }
        private void EditObject()
        {
            if (Input.touchCount >= 2)
            {
                //Scale or Rotate
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);
                if (touch2.phase == TouchPhase.Began)
                {
                    touch1StartPos = touch1.position;
                    touch2StartPos = touch2.position;
                    objStartPos = placeholder.transform.position;
                    objStartScale = placeholder.transform.localScale;
                    objStartRotation = placeholder.transform.localEulerAngles.y;
                    Vector2 newVec = touch2.position - touch1.position;
                    startAngle = Vector2.Angle(Vector2.zero, newVec);
                    if (touch1.position.x > touch2.position.x) startAngle *= -1;
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    //Scale
                    float dif = Vector2.Distance(touch2.position, touch1.position) / Vector2.Distance(touch2StartPos, touch1StartPos);
                    placeholder.transform.localScale = new Vector3(objStartScale.x * dif * scaleFactor, objStartScale.y * dif * scaleFactor, objStartScale.z * dif * scaleFactor);
                    currentScale = placeholder.transform.localScale.x * startScale;
                    //Rotation
                    // Vector2 newV = touch2.position - touch1.position;
                    // float newAngle = Vector2.Angle(touch2StartPos - touch1StartPos, newV);
                    // Vector2 dirV = RotateVector(newV, startAngle);
                    // if (dirV.x < 0)
                    // {
                    //     placeholder.transform.localRotation = Quaternion.Euler(0f, objStartRotation + newAngle, 0f);
                    // }
                    // else
                    // {
                    //     placeholder.transform.localRotation = Quaternion.Euler(0f, objStartRotation - newAngle, 0f);
                    // }
                }
                if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
                {
                    mayPlace = false;
                    Invoke("ResetMayPlace", 0.5f);
                }
            }
        }

        private void ResetMayPlace()
        {
            mayPlace = true;
        }

        private Vector2 RotateVector(Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }
    }
}