using Godot;
using System;
using static Godot.Image;
using static Godot.TextServer;

public partial class Ball : Area2D
{
    // Konstante für die Anfangsgeschwindigkeit des Balls (Pixel pro Sekunde)
    const int INITIAL_BALL_SPEED = 100;

    // Aktuelle Geschwindigkeit des Balls (Pixel pro Sekunde)
    float ballSpeed = INITIAL_BALL_SPEED;

    // Größe des Viewports (Bildschirmbereich), in dem sich der Ball bewegt
    Vector2 screenSize;

    // Bewegungsrichtung des Balls (normierter Vektor)
    Vector2 direction;

    // Zufallsgenerator für kleine Zufallsabweichungen der Richtung
    Random random = new Random();

    // Wird einmal aufgerufen, wenn der Node in die Szene eingefügt wird
    public override void _Ready()
    {
        // Aktuelle Größe des Viewports holen (Breite/Höhe)
        screenSize = GetViewportRect().Size;

        // Startrichtung: nach rechts (X=1) mit leicht zufälliger vertikaler Komponente (Y)
        direction = new Vector2(1f, random.NextSingle() * 0.1f);

        // Richtung auf Länge 1 normalisieren (damit nur ballSpeed die tatsächliche Geschwindigkeit bestimmt)
        direction = direction.Normalized();
    }

    // Wird in jedem Frame aufgerufen. 'delta' ist die vergangene Zeit seit dem letzten Frame.
    public override void _Process(double delta)
    {
        // Aktuelle Position des Balls zwischenspeichern
        Vector2 ballPos = Position;

        // Neue Position berechnen: alte Position + Richtung * Geschwindigkeit * Zeit
        ballPos += direction * ballSpeed * (float)delta;

        // Sicher stellen, dass der Ball im Spielfeld bleibt
        ballPos.X = Mathf.Clamp(ballPos.X, 0, screenSize.X);
        ballPos.Y = Mathf.Clamp(ballPos.Y, 0, screenSize.Y);

        // Berechnete Position zurück auf den Node anwenden
        Position = ballPos;
    }

    // Wird aufgerufen, wenn der Ball ein Paddle trifft.
    // spinFactor beschreibt, wo der Ball auf dem Paddle getroffen hat
    // (z.B. -1 oben, 0 Mitte, +1 unten) und beeinflusst den Abprallwinkel.
    public void HitPaddle(float spinFactor)
    {
        // Neue Richtung:
        // X-Komponente wird umgedreht (Ball fliegt in die andere horizontale Richtung),
        // Y-Komponente wird aus dem spinFactor abgeleitet
        // -> Trifft der Ball das Paddle im oberen Bereich, dann wird der Ball nach oben abgelenkt
        // -> Trifft der Ball das Paddle im unteren Bereich, dann wird der Ball nach unten abgelenkt
        // -> Trifft der Ball das Paddle im mittleren Bereich, dann fliegt der Ball gerade
        Vector2 reflected = new Vector2(-direction.X, spinFactor * 0.8f);

        // Neue Richtung normalisieren, damit wir wieder einen Einheitsvektor haben
        direction = reflected.Normalized();

        // Ballgeschwindigkeit leicht erhöhen, damit das Spiel mit der Zeit schneller wird
        ballSpeed *= 1.1f;
    }

    public void Bounce(bool isHorizontalWall)
    {
        if (isHorizontalWall)
        {
            direction.Y = -direction.Y;
        }
        else
        {
            float x = direction.X;

            if (x > 0)
            {
                GameManager.AddPointsForPlayer(1, 1);
            }
            else if (x < 0)
            {
                GameManager.AddPointsForPlayer(0, 1);
            }

            Reset();
        }
    }

    public void Reset()
    {
        // Ball zurück in die Bildschirmmitte setzen
        Vector2 ballPos = screenSize * 0.5f;

        // Geschwindigkeit auf Anfangswert zurücksetzen
        ballSpeed = INITIAL_BALL_SPEED;

        // Startrichtung: alte horizontale Richtung mit leicht zufälliger vertikaler Komponente (Y)
        direction = new Vector2(direction.X, random.NextSingle() * 0.15f);

        // Richtung auf Länge 1 normalisieren (damit nur ballSpeed die tatsächliche Geschwindigkeit bestimmt)
        direction = direction.Normalized();

        Position = ballPos;
    }
}