using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DigitalRubyShared;



public class FingersControl : MonoBehaviour
{

    public Material mat;
    public Texture2D DefaultTexture;

    private TapGestureRecognizer tapGesture;
    private TapGestureRecognizer doubleTapGesture;
    private TapGestureRecognizer tripleTapGesture;
    private SwipeGestureRecognizer swipeGesture;
    private PanGestureRecognizer panGesture;
    private ScaleGestureRecognizer scaleGesture;
    private RotateGestureRecognizer rotateGesture;
    private LongPressGestureRecognizer longPressGesture;

    Vector2 zoom = Vector2.one;
    float speed = 0.1f;
    bool flipflop = false;



    void Start()
    {
        CreatePlatformSpecificViewTripleTapGesture();
        CreateDoubleTapGesture();
        CreateTapGesture();
        CreateSwipeGesture();
        CreatePanGesture();
        CreateScaleGesture();
        CreateRotateGesture();
        CreateLongPressGesture();

        // pan, scale and rotate can all happen simultaneously
        panGesture.AllowSimultaneousExecution(scaleGesture);
        panGesture.AllowSimultaneousExecution(rotateGesture);
        scaleGesture.AllowSimultaneousExecution(rotateGesture);
    }

    void LateUpdate()
    {

        int touchCount = Input.touchCount;
        if (FingersScript.Instance.TreatMousePointerAsFinger && Input.mousePresent)
        {
            touchCount += (Input.GetMouseButton(0) ? 1 : 0);
            touchCount += (Input.GetMouseButton(1) ? 1 : 0);
            touchCount += (Input.GetMouseButton(2) ? 1 : 0);
        }
        string touchIds = string.Empty;
        int gestureTouchCount = 0;
        foreach (GestureRecognizer g in FingersScript.Instance.Gestures)
        {
            gestureTouchCount += g.CurrentTrackedTouches.Count;
        }
        foreach (GestureTouch t in FingersScript.Instance.CurrentTouches)
        {
            touchIds += ":" + t.Id + ":";
        }
    }

    private void TapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            //DebugText("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
            if (flipflop)
            {
                mat.SetFloat("_Speed", speed);
            }
            else
            {
                mat.SetFloat("_Speed", 0);
            }

            flipflop = !flipflop;


        }
    }

    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }


    private void CreateDoubleTapGesture()
    {
        doubleTapGesture = new TapGestureRecognizer();
        doubleTapGesture.NumberOfTapsRequired = 2;
        doubleTapGesture.StateUpdated += DoubleTapGestureCallback;
        doubleTapGesture.RequireGestureRecognizerToFail = tripleTapGesture;
        FingersScript.Instance.AddGesture(doubleTapGesture);
    }

    private void DoubleTapGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            NativeGallery.GetImageFromGallery(LoadImagecallback);
        }
    }

    private void LoadImagecallback(string _path)
    {
        Debug.Log(_path);
        mat.SetTexture("_MainTexture", NativeGallery.LoadImageAtPath(_path, -1, false, false, false));

    }



    private void SwipeGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            //DebugText("Swiped from {0},{1} to {2},{3}; velocity: {4}, {5}", gesture.StartFocusX, gesture.StartFocusY, gesture.FocusX, gesture.FocusY, swipeGesture.VelocityX, swipeGesture.VelocityY);
            //Vector2 start = new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY);
            //Vector2 end = Camera.main.ScreenToWorldPoint(new Vector2(gesture.FocusX, gesture.FocusY));
            //float distance = Vector2.Distance(start, end);
            //mat.SetFloat("_Speed", distance * -0.001f);


            speed = swipeGesture.VelocityX * -0.0001f;
            mat.SetFloat("_Speed", speed);
        }
    }

    private void CreateSwipeGesture()
    {
        swipeGesture = new SwipeGestureRecognizer();
        swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
        swipeGesture.StateUpdated += SwipeGestureCallback;
        swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
        FingersScript.Instance.AddGesture(swipeGesture);
    }

    private void PanGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            //DebugText("Panned, Location: {0}, {1}, Delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);


            float deltaX = panGesture.DeltaX / 50.0f;
            float deltaY = panGesture.DeltaY / 50.0f;
            zoom.x += deltaX;
            zoom.y += deltaY;
            zoom.x = Mathf.Max(0, Mathf.Min(zoom.x, 20));
            zoom.y = Mathf.Max(0, Mathf.Min(zoom.y, 20));
            mat.SetVector("_Tiling", zoom);
        }
    }

    private void CreatePanGesture()
    {
        panGesture = new PanGestureRecognizer();
        panGesture.MinimumNumberOfTouchesToTrack = 2;
        panGesture.StateUpdated += PanGestureCallback;
        FingersScript.Instance.AddGesture(panGesture);
    }

    private void ScaleGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {
            //DebugText("Scaled: {0}, Focus: {1}, {2}", scaleGesture.ScaleMultiplier, scaleGesture.FocusX, scaleGesture.FocusY);
        }
    }

    private void CreateScaleGesture()
    {
        scaleGesture = new ScaleGestureRecognizer();
        scaleGesture.StateUpdated += ScaleGestureCallback;
        FingersScript.Instance.AddGesture(scaleGesture);
    }

    private void RotateGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Executing)
        {

            mat.SetFloat("_Rotate", rotateGesture.RotationRadians * Mathf.Rad2Deg);
        }
    }

    private void CreateRotateGesture()
    {
        rotateGesture = new RotateGestureRecognizer();
        rotateGesture.StateUpdated += RotateGestureCallback;
        FingersScript.Instance.AddGesture(rotateGesture);
    }

    private void LongPressGestureCallback(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began)
        {
            //DebugText("Long press began: {0}, {1}", gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            //DebugText("Long press moved: {0}, {1}", gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            //DebugText("Long press end: {0}, {1}, delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);
            mat.SetFloat("_Rotate", 0);
            mat.SetVector("_Tiling", new Vector2(3, 0.3f));
            //mat.SetTexture("_MainTexture", DefaultTexture);
            mat.SetFloat("_Speed", 0.1f);
        }
    }

    private void CreateLongPressGesture()
    {
        longPressGesture = new LongPressGestureRecognizer();
        longPressGesture.MaximumNumberOfTouchesToTrack = 1;
        longPressGesture.StateUpdated += LongPressGestureCallback;
        FingersScript.Instance.AddGesture(longPressGesture);
    }

    private void PlatformSpecificViewTapUpdated(GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            Debug.Log("You triple tapped the platform specific label!");
        }
    }

    private void CreatePlatformSpecificViewTripleTapGesture()
    {
        tripleTapGesture = new TapGestureRecognizer();
        tripleTapGesture.StateUpdated += PlatformSpecificViewTapUpdated;
        tripleTapGesture.NumberOfTapsRequired = 3;
        FingersScript.Instance.AddGesture(tripleTapGesture);
    }
}

