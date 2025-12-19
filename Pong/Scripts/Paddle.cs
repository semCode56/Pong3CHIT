using Godot;
using System;
using System.Reflection.Metadata;
using static Godot.TextServer;

public partial class Paddle : Area2D
{
    // Auf welcher Seite des Spielfeldes das Paddle steht.
    // Wird im Inspector gesetzt.
    public enum SideEnum
    {
        Left,
        Right
    }

    // Welche Input-Actions das Paddle steuern.
    // Die Enum-Namen entsprechen exakt den Input-Action-Namen.
    public enum ControlsEnum
    {
        left_move_up,
        left_move_down,
        right_move_up,
        right_move_down
    }

    // Konstante Geschwindigkeit des Paddles in Pixel pro Sekunde
    const int PAD_SPEED = 150;

    // Steuerungstaste für nach oben
    [Export]
    ControlsEnum controlUp;

    // Steuerungstaste für nach unten
    [Export]
    ControlsEnum controlDown;

    // Größe des Bildschirms (Viewport)
    Vector2 screenSize;

    // Referenz auf den Ball (optional über Inspector gesetzt)
    [Export]
    Ball ball;

    // Höhe des Paddles, wird in _Ready() aus dem Sprite gelesen
    float paddleSize;

    // Wird aufgerufen, sobald das Paddle in die Szene geladen wurde
    public override void _Ready()
    {
        // Bildschirmgröße lesen (damit Paddle nicht hinausfährt)
        screenSize = GetViewportRect().Size;

        // Paddle-Höhe direkt aus der Texture des Sprite2D holen
        paddleSize = GetNode<Sprite2D>("Sprite2D").Texture.GetSize().Y;
    }

    // Wird jeden Frame aufgerufen
    public override void _Process(double delta)
    {
        // Aktuelle Position des Paddles zwischenspeichern
        Vector2 pos = Position;

        // Paddle nach oben bewegen:
        // Nur wenn wir oberhalb von Y > 0 bleiben
        // und die entsprechende Input-Action gedrückt wird
        if (pos.Y - paddleSize / 2 > 0 && Input.IsActionPressed(controlUp.ToString()))
            pos.Y += -PAD_SPEED * (float)delta;

        // Paddle nach unten bewegen:
        // Nur wenn wir innerhalb des Bildschirms bleiben
        if (pos.Y + paddleSize / 2 < screenSize.Y && Input.IsActionPressed(controlDown.ToString()))
            pos.Y += PAD_SPEED * (float)delta;

        // Neue Position setzen
        Position = pos;
    }

    // Signal-Handler: Wird aufgerufen, wenn der Ball
    // in das CollisionShape2D des Paddles eindringt (Area2D → Area2D)
    private void OnPaddleAreaEntered(Area2D area)
    {
        // Prüfen, ob das kollidierende Area ein Ball ist
        if (area is Ball ball)
        {
            // relative beschreibt, WO der Ball am Paddle getroffen hat:
            // -1 → ganz oben, 0 → Mitte, +1 → ganz unten
            float relative = (ball.Position.Y - Position.Y) / (paddleSize * 0.5f);

            // Begrenze den Wert sicher auf [-1, +1]
            relative = Mathf.Clamp(relative, -1f, 1f);

            // Treffer sehr nahe an der Mitte → spinFactor auf 0 setzen,
            // damit der Ball relativ neutral zurückkommt
            if (relative > -0.1 && relative < 0.1)
                relative = 0;

            // Ball informieren, dass er getroffen wurde → Spin einbauen
            ball.HitPaddle(relative);
        }
    }
}