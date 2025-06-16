using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;





public class Camera
{
  public Vector2 Position { get; set; }
  public float Zoom { get; set; }
  public Rectangle Bounds { get; private set; }
  public Rectangle VisibleArea { get; private set; }
  public Matrix Transform { get; private set; }

  public Camera(Viewport viewport)
  {
    Bounds = viewport.Bounds;
    Zoom = 1f;
    Position = Vector2.Zero;
  }

  public void Update()
  {
    Transform = Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                Matrix.CreateScale(Zoom) *
                Matrix.CreateTranslation(new Vector3(Bounds.Width * 0.5f, Bounds.Height * 0.5f, 0));

    VisibleArea = new Rectangle(
        (int)(Position.X - Bounds.Width / 2 / Zoom),
        (int)(Position.Y - Bounds.Height / 2 / Zoom),
        (int)(Bounds.Width / Zoom),
        (int)(Bounds.Height / Zoom));
  }


  public void Move(Vector2 amount)
  {
    Position += amount;
  }

  public void ClampPosition(Vector2 min, Vector2 max)
  {
    Position = Vector2.Clamp(Position, min, max);
  }





}