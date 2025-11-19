using Godot;
using System;

namespace World;

/// <summary>
/// Camera2D class with zooming and panning capabilities.
/// Zooming is done with the mouse wheel, panning with right mouse button drag.
/// </summary>
public partial class WorldCamera : Camera2D
{
    /// <summary>
    /// Minimum zoom level.
    /// </summary>
    [Export]
    public float MinZoom { get; private set; } = 0.1f;

    /// <summary>
    /// Maximum zoom level.
    /// </summary>
    [Export]
    public float MaxZoom { get; private set; } = 3.0f;

    /// <summary>
    /// Zoom step for each mouse wheel scroll.
    /// </summary>
    [Export]
    public float ZoomStep { get; private set; } = 0.1f;

    /// <summary>
    /// Current zoom level.
    /// </summary>
    private float _currentZoom = 1.0f;

    /// <summary>
    /// Indicates whether the camera is currently being panned.
    /// </summary>
    private bool _isPanning = false;

    /// <summary>
    /// Handles unhandled input events for zooming and panning.
    /// </summary>
    /// <param name="event">The input event</param>
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.WheelUp)
                SetZoom(_currentZoom + ZoomStep);
            else if (mouseEvent.ButtonIndex == MouseButton.WheelDown)
                SetZoom(_currentZoom - ZoomStep);

            if (mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
            {
                _isPanning = true;
            }
            else if (mouseEvent.ButtonIndex == MouseButton.Right && !mouseEvent.Pressed)
            {
                _isPanning = false;
            }
        }

        if (@event is InputEventMouseMotion mouseMotionEvent)
        {
            if (_isPanning)
            {
                var mouseMotion = mouseMotionEvent.Relative;
                GlobalPosition -= mouseMotion / Zoom.X;
            }
        }
    }

    /// <summary>
    /// Sets the zoom level, clamped between MinZoom and MaxZoom.
    /// Should keep the point under the mouse cursor stationary, but is not working right now.
    /// The second commented approach works a little bit, but not perfectly.
    /// </summary>
    /// <param name="newZoom">The new zoom level to set.</param>
    private void SetZoom(float newZoom)
    {
        // var oldZoom = currentZoom;
        // var oldMousePos = GetGlobalMousePosition();
        // currentZoom = Mathf.Clamp(newZoom, MinZoom, MaxZoom);
        // Zoom = new Vector2(currentZoom, currentZoom);
        // var newMousePos = GetGlobalMousePosition();
        // var difference = newMousePos - oldMousePos;
        // GD.Print($"Old mouse pos: {oldMousePos}, New mouse pos: {newMousePos}, Difference: {difference}");
        // GlobalPosition -= difference;
        // //GlobalPosition += new Vector2(10, 10);

        // var oldZoom = currentZoom;
        // currentZoom = Mathf.Clamp(newZoom, MinZoom, MaxZoom);
        // var distance = GetGlobalMousePosition() - GlobalPosition;
        // var newPosition = GlobalPosition + distance * (1 - oldZoom / currentZoom);
        // Zoom = new Vector2(currentZoom, currentZoom);
        // GlobalPosition = newPosition;

        _currentZoom = Mathf.Clamp(newZoom, MinZoom, MaxZoom);
        Zoom = new Vector2(_currentZoom, _currentZoom);

        GD.Print($"Zoom set to: {_currentZoom}");
        GD.Print($"Camera position set to: {GlobalPosition}");
        GD.Print($"Mouse global position: {GetGlobalMousePosition()}");
    }
}
