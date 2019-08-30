using Godot;
using System;

public class Main : Spatial {
    
    public override void _Ready () {
        Input.SetMouseMode (Input.MouseMode.Captured);
    }

    public override void _Process (float delta) {
        if (Input.IsActionJustReleased ("ui_cancel")) {
            Input.SetMouseMode (Input.MouseMode.Visible);
            
            GetTree().Quit ();
        }

        if (Input.IsActionJustReleased ("restart")) {
            GetTree ().ReloadCurrentScene ();
        }
    }
}