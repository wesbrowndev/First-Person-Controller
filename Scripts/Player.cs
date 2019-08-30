using Godot;
using System;

public class Player : KinematicBody {
    [Export] NodePath HeadObj;

    [Export] float MouseSensitivity = 0.3f;
    [Export] float MaxCameraAngle = 90;
    [Export] float MinCameraAngle = -90;

    Spatial head;
    Camera mainCamera;

    float cameraAngle;
    Vector2 cameraChange;

    Vector3 velocity;
    Vector3 direction;

    // Walk
    const float Gravity = -9.8f * 3;
    const float MaxSpeed = 5;
    const float MaxRunningSpeed = 10;
    const float Accel = 2;
    const float DeAccel = 6;

    // Jump
    const float JumpHeight = 15;

    // Flight
    const float FlySpeed = 2;
    const float FlyAccel = 4;
    bool flying;

    public override void _Ready () {
        head = GetNode <Spatial> (HeadObj);
        mainCamera = head.GetChild <Camera> (0);
    }

    public override void _Input (InputEvent @event) {
        if (@event is InputEventMouseMotion motion) {
            cameraChange = motion.Relative;
        }
    }

    public override void _PhysicsProcess (float delta) {
        Aim ();

        if (flying) {
            Fly (delta);
        }
        else {
            Walk (delta);
        }
    }


    void Fly (float delta) {
        // Reset the direction
        direction = new Vector3 ();

        // Get camera rotation
        var aim = mainCamera.GlobalTransform.basis;

        // Receive Input
        if (Input.IsActionPressed ("move_forward")) {
            direction -= aim.z;
        }

        if (Input.IsActionPressed ("move_backward")) {
            direction += aim.z;
        }

        if (Input.IsActionPressed ("move_left")) {
            direction -= aim.x;
        }

        if (Input.IsActionPressed ("move_right")) {
            direction += aim.x;
        }

        direction = direction.Normalized ();

        // Where the player goes at max speed
        var target = direction * FlySpeed;

        // Calculate a portion of the distance to start
        velocity = velocity.LinearInterpolate (target, FlyAccel * delta);

        // Move
        MoveAndSlide (velocity);
    }

    void Walk (float delta) {
        // Reset the direction
        direction = new Vector3 ();

        // Get camera rotation
        var aim = mainCamera.GlobalTransform.basis;

        // Receive Input
        if (Input.IsActionPressed ("move_forward")) {
            direction -= aim.z;
        }

        if (Input.IsActionPressed ("move_backward")) {
            direction += aim.z;
        }

        if (Input.IsActionPressed ("move_left")) {
            direction -= aim.x;
        }

        if (Input.IsActionPressed ("move_right")) {
            direction += aim.x;
        }

        direction = direction.Normalized ();

        // Gravity
        velocity.y += Gravity * delta;

        var tempVelocity = velocity;
        tempVelocity.y = 0;

        var speed = Input.IsActionPressed ("move_sprint") ? MaxRunningSpeed : MaxSpeed;

        // Where the player goes at max speed
        var target = direction * speed;

        var acceleration = direction.Dot (tempVelocity) > 0 ? Accel : DeAccel;

        // Calculate a portion of the distance to start
        tempVelocity = tempVelocity.LinearInterpolate (target, acceleration * delta);

        velocity.x = tempVelocity.x;
        velocity.z = tempVelocity.z;

        // Move
        velocity = MoveAndSlide (velocity, Vector3.Up);

        if (Input.IsActionJustPressed ("jump") && IsOnFloor ()) {
            velocity.y = JumpHeight;
        }
    }

    void Aim () {
        if (cameraChange.Length () > 0) {
            head.RotateY (Mathf.Deg2Rad (-cameraChange.x * MouseSensitivity));

            var change = -cameraChange.y * MouseSensitivity;

            if (change + cameraAngle < MaxCameraAngle && change + cameraAngle > MinCameraAngle) {
                mainCamera.RotateX (Mathf.Deg2Rad (change));

                cameraAngle += change;
            }

            cameraChange = new Vector2 ();
        }
    }

    void _on_Area_body_entered (object body) {
        if (body.ToString () == "Player") {
            flying = true;
        }
    }
    
    void _on_Area_body_exited(object body)
    {
        if (body.ToString () == "Player") {
            flying = false;
        }
    }
}
