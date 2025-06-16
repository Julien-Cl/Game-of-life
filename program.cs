using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.InteropServices;






public static class Program 
{

  [DllImport("kernel32.dll", SetLastError = true)]
  [return: MarshalAs(UnmanagedType.Bool)]
  static extern bool AllocConsole();

  [STAThread] 

  public static void Main(string[] args) 
  {
    try
    {
      AllocConsole(); // Affichage de la console


      using (var game = new Game1())
      {
        game.Run();


        // On maintient la console ouverte
        Console.WriteLine("Appuyez sur une touche pour fermer...");
        Console.ReadKey();


      }
    }
    catch (Exception e)
    {
      Console.WriteLine($"Une erreur est survenue : {e.Message}");
    }
  }
}









public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private Camera _camera;
  private Rectangle _worldBounds;



  private GameOfLife gameOfLife;

  private Texture2D _blankTexture;

  private MouseState _previousMouseState;

  private int resolutionWidth = 1000;
  private int resolutionHeight = 1000;

  private int simulationWidth = 2000;
  private int simulationHeight = 2000;

  private int terrainWidth;
  private int terrainHeight;

  private int terrainBorderAroundSimulation = 2;




  private float zoomLevel;

  // Constructor
  // -----------
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);

    // Set game loop update speed
    IsFixedTimeStep = false;
    float updatesPerSecond = 10.0f;
    TargetElapsedTime = TimeSpan.FromSeconds(1f / updatesPerSecond);

    Content.RootDirectory = "Content";
    IsMouseVisible = false;
  }

  // Initialization
  // --------------
  protected override void Initialize()
  {

    terrainWidth = simulationWidth + terrainBorderAroundSimulation * 2;
    terrainHeight = simulationHeight + terrainBorderAroundSimulation * 2;


    gameOfLife = new GameOfLife(simulationWidth, simulationHeight, terrainBorderAroundSimulation); // Important pour appeler le constructeur. Sinon l'objet est du bon type mais le constructeur n'a jamais été appelé. 

    // Set resolution
    _graphics.PreferredBackBufferWidth = resolutionWidth;
    _graphics.PreferredBackBufferHeight = resolutionHeight;
    _graphics.ApplyChanges();


    _camera = new Camera(GraphicsDevice.Viewport);
    _worldBounds = new Rectangle(
      terrainBorderAroundSimulation, 
      terrainBorderAroundSimulation,
      terrainWidth + terrainBorderAroundSimulation,
      terrainWidth + terrainBorderAroundSimulation);


    base.Initialize();
  }

  // Loading content
  // ---------------
  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);


    // Créer une texture blanche de 1x1 pixel
    _blankTexture = new Texture2D(GraphicsDevice, 1, 1);
    _blankTexture.SetData(new[] { Color.White });


  }


  // Game loop
  // ---------
  protected override void Update(GameTime gameTime)
  {
    // Exit si la fenêtre est fermée
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();


    // Mouse input
    // -----------
    MouseState currentMouseState = Mouse.GetState();

    if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
    {
      int cellSize = gameOfLife.cellSize;

      // Convertir les coordonnées de la souris en indices de la grille
      int gridX = currentMouseState.X / cellSize; 
      int gridY = currentMouseState.Y / cellSize;


      // Vérifier si le clic est dans les limites de la grille
      if (gridX >= 0 && gridX < resolutionWidth && gridY >= 0 && gridY < resolutionHeight)
      {
        //gameOfLife.SetCellValue(true, gridX, gridY);

        gameOfLife.RandomFeed(); 

      }
    }


    // Zoom
    // ----
    float zoomSpeed = 0.001f;
    float zoomFarthest = 0.1f;
    float zoomNearest = 5;

    int scrollDifference = currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;

    zoomSpeed += (Math.Abs(scrollDifference) * 0.00001f);

    zoomLevel += scrollDifference * zoomSpeed;

    zoomLevel = MathHelper.Clamp(zoomLevel, zoomFarthest, zoomNearest);

    _previousMouseState = currentMouseState;

    _camera.Zoom = zoomLevel;




    // Camera slide
    // ------------
    float movementSpeed = 0.1f * (zoomNearest - zoomLevel) * (zoomNearest - zoomLevel);

    movementSpeed = Math.Max(movementSpeed, 4.0f); 

    int scrollBorder = 1; 

    // Gestion du défilement de l'écran
    Vector2 movement = Vector2.Zero;

    if (currentMouseState.X <= scrollBorder)
      movement.X -= movementSpeed;
    else if (currentMouseState.X >= GraphicsDevice.Viewport.Width - scrollBorder)
      movement.X += movementSpeed;

    if (currentMouseState.Y <= scrollBorder)
      movement.Y -= movementSpeed;
    else if (currentMouseState.Y >= GraphicsDevice.Viewport.Height - scrollBorder)
      movement.Y += movementSpeed;


    _camera.Move(movement);

    // Limiter la caméra aux limites du monde
    Vector2 min = new Vector2(_camera.VisibleArea.Width / 2, _camera.VisibleArea.Height / 2);
    Vector2 max = new Vector2(_worldBounds.Width - _camera.VisibleArea.Width / 2,
                              _worldBounds.Height - _camera.VisibleArea.Height / 2);
    _camera.ClampPosition(min, max);

    _camera.Update();



    // Simulation update
    // -----------------

    Benchmark.Begin();

    gameOfLife.Update();

    Benchmark.End("Game logic update");



    base.Update(gameTime);
  }



  // Rendering loop
  // --------------
  // Called 60 times per second.
  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.CornflowerBlue);


    Benchmark.Begin();

    _spriteBatch.Begin(transformMatrix: _camera.Transform);
    gameOfLife.Draw(_spriteBatch, _blankTexture);
    _spriteBatch.End();

    Benchmark.End("Rendering");



    base.Draw(gameTime);
  }
}

