# PROJETOTDV_PLATFORMER2D
# Projeto_TDV_Platformer2d

# 2D Platformer

Este projeto é sobre um jogo 2D Platformer desenvolvido em monogame e em C#

----

**Grupo:**

Gonçalo Silva- a25970;

Gustavo Gonçalves- a25960;

Miguel Sousa- a25977;

---

## Estrutura do Código

Uma vez que o jogo foi desenvolvido em C#, o código está dividido em classes:

1. **Game1**

2. **Camera**

3. **Platform**

4. **MovingPlatform**

5. **Map**

6. **Ladder**

7. **BackgroundTile**

8. **State**

9. **MenuState**

10. **GameState**

11. **EndState**

12. **DeadState**

13. **Enemy**

14. **Player**

15. **Coin**

16. **Animation**

17. **Button**

 

----

## Análise das diversas classes

### Game1:

Esta classe é a principal classe do jogo e tem como objetivo manter o ciclo de vida do jogo e tudo o que o envolve.

```cs
 public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static int screenWidth = 1280;
        public static int screenHeight = 720;
        private State _currentState;
        private State _nextState;
        public static float TotalSeconds; // Används för att animationer ska uppdateras 
        public static List<int> scores = new List<int>(); // Highscore-lista

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = screenWidth; 
            _graphics.PreferredBackBufferHeight = screenHeight;  
            _graphics.ApplyChanges();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _currentState = new MenuState(this, GraphicsDevice, Content);
        }
        protected override void Update(GameTime gameTime)
        {
            TotalSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_nextState != null) // Om nextState har ett värde ska en ny state visas
            {
                _currentState = _nextState;
                _currentState.LoadContent(); // Laddar in nya staten
                _nextState = null;
            }
            _currentState.Update(gameTime); // Uppdaterar aktuella staten
            base.Update(gameTime);
        }
        public void ChangeState(State state)
        {
            _nextState = state;
        }
        public void Quit()
        {
            this.Exit();
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            _currentState.Draw(gameTime, _spriteBatch); // Drawar aktuella staten
            base.Draw(gameTime);
        }
    }
```

Esta classe utiliza um objeto `_graphics` cujo objetivo é gerir os gráficos e resolução.

Utiliza outro objeto `_spriteBatch` que é utilizado para desenhar texturas.

A `ScreenHeight` e a `ScreenWidth` são as dimensões da tela de jogo.

O `_currentState` representa o estado atual do jogo e o `_nextState` representa o estado de jogo que deve ser carregado.

A variável `TotalSeconds` é um timer que representa o tempo total de segundos desde que o jogo iniciou.Também é utilizado durante o jogo para as animações.

A lista `scores`  permite guardar as pontuações dos jogadores e exibi-las no menu high- scores.

O contrutor é o `Game1()` e nele é definido o diretório raiz de conteúdo como `Content` 

O método `Initialize()` é utilizado para realizar qualquer inicialização. Neste caso não existe nenhuma inicilização necessária, há exceção da inicialização padrão.

O método `LoadContent()` é utilizado para carregar todos os recursos do jogo. O `_spriteBatch`  é inicializado e o `_currentState` é atualizado para `MenuState`. 

O método `Update()` é utilizado para ir atualizando o jogo, tal como detetar colisões, atualizações de posições de objetos, entre outros. Começa por atualizar a variável `TotalGameSeconds` . Em seguida ele verifica se existe uma próxima state a ser carregada e se houver, ele atualiza a `_currentState` para a que está a ser carregada e atualiza a `_nextState` para NULL. Posteriormente ele da Update utilizando a `_currentState` de modo a ir atualizando o jogo. 

O método `ChangeState()` é utilizado para mudar o estado atual do jogo. Ele recebe um objeto State que representa o próximo estado a ser carregado.

O método `Quit()` é utilizado para fechar o jogo e utiliza o método `Exit()` para fechar o jogo.

O método `Draw()` é utilizado para renderizar a cena atual da tela. Recebe o parâmetro `gametime`  para sincrozinar a renderização com o tempo. O método começa por limpar a tela e meter um fundo azul. Após isso ele chama o método `Draw()` da `_currentState` , utilizando o `gametime` e o `_spriteBatch` como parâmetros de modo a renderizar a cena atual.

---

### Camera:

Esta classe é responsável pela movimentação da camera e pela posição dos objetos que serão renderizados no jogo.

```cs
        internal class Camera
    {
        public Matrix Transform { get; private set; }

        public void Follow(Player target)
        {
            var position = Matrix.CreateTranslation(
                MathHelper.Clamp(-target.position.X - (target.rectangle.Width / 2), -3420, -650),
                MathHelper.Clamp(-target.position.Y - (target.rectangle.Height / 2), -300, -3475738),
                0) ;
            var offset = Matrix.CreateTranslation(
                    Game1.screenWidth / 2,
                    Game1.screenHeight / 2 - 100,
                    0);
            Transform = position * offset;
        }
    }
```

Este código utiliza uma propriedade `Transform`  do tipo `Matrix` que permite guardar uma matriz 4x4.

O outro método presente nesta classe é o  `Follow(Player target)`  que recebe um parametro `target`  do `Player` . 

É criada uma matriz de translação `position` utilizando o `target` .

A outra matriz `offset` é criada com o objetivo de centrar a câmara.

Por fim a propriedade `Transform` assume o produto das matrizes `position`x`offset` 

Esta matriz é utilizada de modo a mudar a posição de objetos antes deles serem renderizados.

----

### Platform:

Esta classe é utilizada para criar plataformas com que o Player pode colidir.

```cs
 public class Platform
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rectangle, rectangleTop;
        public bool isDeadly, isOnlySolidTop, isBouncy;

        /*
         * isDeadly ger en plattform som spelaren dör av att kollidera med
         * 
         * isOnlySolidTop ger en plattform som spelaren kan gå igenom i sidled, hoppa igenom och kan stå på
         * 
         * isBouncy ger en plattform som spelaren studsar på om han rör den med fötterna
         */

        public Platform(Texture2D newTexture, Vector2 newPosition, bool deadly, bool onlySolidTop, bool bouncy)
        {
            texture = newTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);
            rectangleTop = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, 25);
            isDeadly = deadly;
            isOnlySolidTop = onlySolidTop;
            isBouncy = bouncy;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }

    }
```

Nesta classe as variáveis são as seguintes: textura `texture` , posição `position` , retângulo `rectangle` rectangleTop `rectangleTop`  e as variáveis do tipo boleano `isDeadly`, `isOnlySolidTop` , `isBouncy`.

Depois temos o construtor que inicializa as diversas propriedades. O primeiro parâmetro é a `texture` da plataforma, o segundo parâmetro é a `position` da mesma e os restantes parâmetros são boleanos que decidem o comportamento da plataforma.

O método `Draw()` desenha a textura da plataforma na tela utilizando o seu retângulo para ela ser colocada com o tamanho correto da terxtura e a posição da plataforma.

---

### MovingPlatform:

Esta classe é uma implementação básica de uma plataforma que se desloca e que contém colisões.

```cs
 public class MovingPlatform
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rectangle;
        public float velocity = 3f;
        public float stop = 0f;
        public float start = 3f;

        public MovingPlatform(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
        }

        public void Update()
        {
            position.X += velocity;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);
            foreach (Platform platform in GameState.platforms)
            {
                if (rectangle.Intersects(platform.rectangle)) 
                {
                    velocity = -velocity;
                    start = -start;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

    }
}
```

Nesta classe existem as variáveis texture textura `texture` , posição `position` , retângulo `rectangle` , velocidade `velocity` , stop `stop` e start `start` .
O construtor presente pega na textura e na posição como parâmetros e inicializa as variáveis com valores.
O método `Update()` faz com que a posição da plataforma seja atualizada ao somar a velocidade com o X da posição, atualizando o retângulo para estar na sua nova posição e verifica se existem colisões com outras plataformas. Se existir uma colisão a plataforma troca de direção ao inverter a sua velocidadee valor inicial.
O método `Draw()` utiliza um objeto `SpriteBatch` e desenha a textura da plataforma na sua posição atual no jogo.

----

### Map:

Esta classe trata da criação do mapa do jogo e de adicionar texturas ao nível.

*Nível:*

```cs
public static int[,] map_layout1 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7},
            { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 5, 0, 0, 1, 9, 0, 0, 0, 0, 0, 3, 1, 1 ,11,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,1, 1},
            { 0, 0, 0, 14,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 3, 1, 1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,0, 0, 0, 0, 0, 0, 4, 4, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,13, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 6, 0, 4, 4, 4, 4, 0, 0, 0, 0, 11,0, 0, 0, 0, 0, 12,12,12,12,12,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,13, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 10,0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 9, 1, 0, 0, 11,0, 0, 0, 0, 0, 12,12,12,12,12,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,13, 2},
            { 0, 0, 0, 0, 0, 5, 0, 0, 3, 1, 2, 2, 6, 6, 2, 0, 8, 0, 0, 0, 0, 0, 2, 6, 6, 6, 6, 6, 6, 2, 4, 4, 11,0, 6, 0, 0, 0, 12,12,12,12,12,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,13, 2},
            { 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 10,0, 0, 0, 0, 9, 0, 3, 1, 1, 1, 2},
            { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2},
            { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2},
            { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2},
        };

        //tiles som spelaren inte kan kollidera med, "bakgrunden".
        public static int[,] background_layout1 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 7, 4, 6, 0, 6, 7, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
       /**/ { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0},
        };



        public static int[,] map_layout2 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 4, 4, 0, 2, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            { 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 4, 4, 0, 4, 0, 0, 2, 0, 1, 4, 4, 4, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            { 11,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 10,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 1, 1, 10,0, 4, 3, 1, 2, 0, 2, 1, 1, 1, 10,0, 0, 3, 10,0, 0, 0, 0, 0, 2},
            { 11,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 14,14,0, 0, 4, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 0, 1, 0, 2},
            { 11,0, 4, 4, 4, 4, 4, 0, 5, 0, 0, 3, 1, 1, 10,0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 1, 2, 4, 2},
            { 1, 1, 1, 1, 1, 1, 1, 11,1, 1, 1, 2, 2, 2, 2, 0, 3, 1, 10,0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13,2, 4, 0, 0, 0, 8, 0, 0, 2, 8, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 4, 2},
            { 0, 0, 0, 0, 0, 0, 0, 11,2, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 0, 3, 1, 10,11,0, 0, 0, 0, 0, 0, 0, 4, 0, 4, 0, 4, 0, 4, 13,2, 1, 11,1, 1, 10,0, 0, 2, 1, 1, 1, 1, 1, 11,0, 0, 0, 0, 0, 0, 0, 2, 4, 2},
            { 0, 0, 0, 0, 0, 0, 0, 11,2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 11,0, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,0, 0, 0, 0, 0, 0, 0, 2, 4, 2},
            { 0, 0, 0, 0, 0, 3, 1, 1, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 11,1, 1, 10,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11,0, 0, 0, 0, 0, 0, 0, 2, 7, 2},
            { 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 1, 10,9, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},
            { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
            { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
            { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static int[,] background_layout2 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
       /**/ { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 3, 3, 3, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        };

        public static int[,] map_layout3 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            { 0, 4, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            { 1, 1, 1, 1, 1, 10,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 0, 0, 0, 0, 2, 0, 0, 3, 1, 1, 10,0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2},
            { 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 5, 0, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 1, 1, 1, 11,0, 0, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 3, 1, 1, 6, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12,0, 1, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 2, 2, 2, 11,4, 4, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 2, 1, 0, 2, 8, 4, 4, 4, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12,12,0, 0, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 1, 1, 10,0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 0, 0, 0, 2, 0, 2, 1, 1, 1, 1, 11,2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12,12,12,0, 0, 2},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 0, 0, 0, 2, 0, 0, 0, 8, 0, 0, 0, 0, 8, 0, 0, 0, 0, 8, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 11,2, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 12,12,12,12,0, 0, 2},
            { 0, 0, 0, 0, 0, 0, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 6, 6, 6, 2, 0, 0, 0, 0, 0, 0, 11,2, 6, 6, 6, 1, 0, 0, 5, 5, 5, 0, 12,12,12,12,12,6, 6, 2},
            { 1, 1, 1, 1, 1, 1, 10,0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 0, 0, 2, 6, 6, 6, 2, 6, 6, 6, 6, 2, 6, 6, 6, 6, 2, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
            { 2, 2, 2, 2, 2, 2, 2, 6, 6, 6, 6, 6, 6, 6, 6, 6, 1, 6, 6, 2, 1, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
            { 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
            { 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},
        };

        public static int[,] background_layout3 = new int[,] {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
       /**/ { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
```

* 1 = Relva 

* 2 = Terra

* 3 = Borda superior esquerda inclinada da relva

* 4 = Moeda

* 5 = Inimigo

* 6 = Prego

* 7 = Sinal

* 8 = "Executar"

* 9 = Plataforma móvel

* 10 = Borda superior direita inclinada da relva

* 11 = Escada

* 12 = Caixa

* 13 = Prego lado direito

* 14 = Pico superior

```cs
public static int[,] map_layout4 = new int[,]
        {
            {0,0}
        };
        public static int[,] background_layout4 = new int[,]
        {
            {0,0}
        };

        #endregion

        static int currentLevel = GameState.player.level;

        static int[][,] mapLayouts = { map_layout1, map_layout2, map_layout3, map_layout4 }; // Array av alla maps
        static int[][,] backgroundLayouts = { background_layout1, background_layout2, background_layout3, background_layout4}; // Array av alla bakgrunder

        static int[,] currentMapLayout = mapLayouts[currentLevel - 1]; // Anger korrekt map beroende på spelarens level
        static int[,] currentBackgroundLayout = backgroundLayouts[currentLevel - 1];

        public Map() { }
        public static void Generate()
        {
             currentLevel = GameState.player.level;
       currentMapLayout = mapLayouts[currentLevel - 1];
       currentBackgroundLayout = backgroundLayouts[currentLevel - 1]; ---
```

O código acima define e gera mapas de jogo com base no nível do jogador.
Os mapas são representados por matrizes de inteiros bidimensionais: `map_layout1`, `map_layout2`, `map_layout3` e `map_layout4`. Da mesma forma, os fundos são representados por `background_layout1`, `background_layout2`, `background_layout3` e `background_layout4`.
Esses layouts são armazenados em duas matrizes separadas, `mapLayouts` e `backgroundLayouts`, respectivamente. O nível atual é armazenado em `currentLevel`, que é inicializado como `GameState.player.level`.
Quando `Generate()` é chamado, `currentLevel` é atualizado para o nível atual do jogador, e `currentMapLayout` e `currentBackgroundLayout` são definidos como o layout correspondente em `mapLayouts` e `backgroundLayouts`.
É importante reparar que as matrizes `mapLayouts` e `backgroundLayouts` começam no índice 0, enquanto `currentLevel` começa em 1. Portanto, ao acessar `mapLayouts` e `backgroundLayouts`, é utilizado `currentLevel - 1` para obter o índice correto do nível.

```cs
// Generarar fÃ¶rgrunden
            for (int x = 0; x < currentMapLayout.GetLength(1); x++)
            {
                for (int y = 0; y < currentMapLayout.GetLength(0); y++)
                {
                    int number = currentMapLayout[y, x];

                    switch (number)
                    {
                        case 1:
                            GameState.platforms.Add(new Platform(GameState.grass_texture, new Vector2(x * GameState.grass_texture.Width, y * GameState.grass_texture.Width), false, false, false));
                            break;
                        case 2:
                            GameState.platforms.Add(new Platform(GameState.dirt_texture, new Vector2(x * GameState.dirt_texture.Width, y * GameState.dirt_texture.Width), false, false, false));
                            break;
                        case 3:
                            GameState.platforms.Add(new Platform(GameState.grass2_texture, new Vector2(x * GameState.grass2_texture.Width, y * GameState.grass2_texture.Width), false, false, false));
                            break;
                        case 4:
                            GameState.coins.Add(new Coin(GameState.coin_texture, new Vector2(x * 64 + 16, y * 64)));
                            break;
                        case 5:
                            GameState.enemies.Add(new Enemy(GameState.enemy_walking_texture, new Vector2(x * 64, y * 64 - 29)));
                            break;
                        case 6:
                            GameState.platforms.Add(new Platform(GameState.spike_texture, new Vector2(x * GameState.spike_texture.Width, y * GameState.spike_texture.Width + 16), true, false, false));
                            break;
                        case 7:
                            GameState.platforms.Add(new Platform(GameState.sign_texture, new Vector2(x * 64, y * 64 + 10), false, false, false));
                            break;
                        case 8:
                            GameState.platforms.Add(new Platform(GameState.spring_texture, new Vector2(x * GameState.spring_texture.Width, y * GameState.spring_texture.Height), false, false, true));
                            break;
                        case 9:
                            GameState.movingPlatforms.Add(new MovingPlatform(GameState.grass3_texture, new Vector2(x * GameState.grass3_texture.Width, y * GameState.grass3_texture.Height)));
                            break;
                        case 10:
                            GameState.platforms.Add(new Platform(GameState.grass4_texture, new Vector2(x * GameState.grass4_texture.Width, y * GameState.grass4_texture.Width), false, false, false));
                            break;
                        case 11:
                            GameState.ladders.Add(new Ladder(GameState.ladder_texture, new Vector2(x * GameState.ladder_texture.Width, y * GameState.ladder_texture.Width)));
                            break;
                        case 12:
                            GameState.platforms.Add(new Platform(GameState.box_texture, new Vector2(x * GameState.box_texture.Width, y * GameState.box_texture.Width), false, true, false));
                            break;
                        case 13:
                            GameState.platforms.Add(new Platform(GameState.spike_right_texture, new Vector2(x * 64 + 16, y * 64), true, false, false));
                            break;
                        case 14:
                            GameState.platforms.Add(new Platform(GameState.spike_top_texture, new Vector2(x * 64, y * 64), true, false, false));
                            break;
                    }
                }
            }

            // Genererar bakgrunden
            for (int x = 0; x < currentBackgroundLayout.GetLength(1); x++)
            {
                for (int y = 0; y < currentBackgroundLayout.GetLength(0); y++)
                {
                    int number = currentBackgroundLayout[y, x];

                    switch (number)
                    {
                        case 1:
                            GameState.backgroundTiles.Add(new BackgroundTile(GameState.cloud_texture, new Vector2(x * GameState.cloud_texture.Width, y * GameState.cloud_texture.Height)));
                            break;
                        case 2:
                            GameState.backgroundTiles.Add(new BackgroundTile(GameState.water1_texture, new Vector2(x * GameState.water1_texture.Width, y * 64 + 32)));
                            break;
                        case 3:
                            GameState.backgroundTiles.Add(new BackgroundTile(GameState.water2_texture, new Vector2(x * GameState.water2_texture.Width, y * GameState.water2_texture.Height)));
                            break;
                        case 4:
                            GameState.backgroundTiles.Add(new BackgroundTile(GameState.mushroom_texture, new Vector2(x * GameState.mushroom_texture.Width, y * GameState.mushroom_texture.Height)));
                            break;
                        case 5:
                            GameState.backgroundTiles.Add(new BackgroundTile(GameState.plant1_texture, new Vector2(x * GameState.plant1_texture.Width, y * GameState.plant1_texture.Height)));
                            break;
                        case 6:
                            GameState.backgroundTiles.Add(new BackgroundTile(GameState.plant2_texture, new Vector2(x * GameState.plant1_texture.Width, y * GameState.plant1_texture.Height)));
                            break;
                        case 7:
                            GameState.backgroundTiles.Add(new BackgroundTile(GameState.plant3_texture, new Vector2(x * GameState.plant1_texture.Width, y * GameState.plant1_texture.Height)));
```

O código acima cria a aparência do jogo com base num layout de mapa predefinido, adicionando diferentes elementos de jogo e objetos do plano de fundo com base nos números de célula do mapa.
O loop externo está repete as colunas do layout do mapa atual, enquanto o loop interno está repetir as linhas. Em seguida, o número atual da célula do mapa é recuperado e um switch case é usado para instanciar diferentes objetos de jogo com base nesse número.
Para a frente do jogo, dependendo do número da célula, uma plataforma, uma moeda, um inimigo, uma escada ou uma caixa são adicionados à lista de objetos de jogo, com base nas suas coordenadas x e y.
Para o plano de fundo do jogo, uma nuvem, água, cogumelo e plantas são adicionados à lista de objetos de plano de fundo com base nas coordenadas x e y.

---

### Ladder:

Esta classe permite armazenar as informações necessárias para desenhar o `ladder` no jogo.

```cs
 public class Ladder
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rectangle;

        public Ladder(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, Color.White);
        }
    }
```

As propriedades do tile `Ladder`  são, a textura deste tile `texture` , a sua posição `position` , e um retângulo `rectangle`  que representa os espaço ocupado pela escada.
Depois disso temos um construtor que recebe a textura e a posição da escada.
Por fim temos o método Draw, que vai utilizar o `rectangle` e a `texture` para desenhar a escada na posição correta.

----

### BackgroundTile:

Esta classe desenha o *background* do jogo.

```cs
 public class BackgroundTile
    {
        public Texture2D texture;
        public Vector2 position;

        public BackgroundTile(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }

    }
}
```

Na classe existem duas variáveis: Textura `Texture2D` e posição `Vector2` .
Existe ainda um contrutor que recebe dois parâmetros: `newTexture` de tipo `Texture2D` e `newPosition` de tipo `Vector2` . Este construtor inicializa as variáveis textura e posição com os parâmetros que lhes são atribuídos.
Também tem um método `Draw` que aceita uma `SpriteBtach` . Este método usa a `SpriteBatch` para sesenhar a textura na posição com a cor branca de `Color.White` .

----

### State:

Esta classe pretende definir uma classe base para criar diferentes estados ou telas do jogo. Cada estado pode herdar dessa classe e implementar o seu próprio comportamento para carregar conteúdo, criar gráficos e atualizar a lógica do jogo.

```cs
  public abstract class State // Abstrakt klass då varje State ska ärva samma egenskaper (ContentManager, GraphicsDevice, Game1, Konstruktor)
    {
        protected ContentManager _content; // ContentManager för att kunna ladda in och använda innehåll
        protected GraphicsDevice _graphics; // GraphicsDevice för att kunna rita saker på skärmen
        protected Game1 _game; // Tillgång till Game1 för att kunna byta GameState mm.

        public State(Game1 game, GraphicsDevice graphicsDevice, ContentManager content)
        {
            _content = content;
            _graphics = graphicsDevice;
            _game = game;
        }

        public abstract void LoadContent();
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public abstract void Update(GameTime gameTime);
    }
}
```

A classe `State` tem três variáveis protegidas: `_content` do tipo `ContentManager`, `_graphics` do tipo `GraphicsDevice` e `_game` do tipo `Game1`. Essas variáveis são usadas para carregar conteúdo, criar gráficos e mudar estados do jogo.
A classe `State` também possui um construtor que recebe três parâmetros: `game` do tipo `Game1`, `graphicsDevice` do tipo `GraphicsDevice` e `content` do tipo `ContentManager`. O construtor inicializa as variáveis correspondentes com os valores passados como parâmetros.
A classe `State` tem três métodos abstratos: `LoadContent()`, `Draw()` e `Update()`. Esses métodos são declarados como abstratos, o que significa que eles devem ser implementados por qualquer classe concreta que herde de `State`. Isso permite que cada estado defina o seu próprio comportamento ao carregar conteúdo, criar gráficos e atualizar a lógica do jogo.

---

### MenuState:

Este código implementa a funcionalidade do estado do menu. Esta classe herda da classe `State` e tem como objetivo o desenho e atualização da tela do menu do jogo e criar botões de opções para o jogador.

```cs
 public class MenuState : State
    {
        SpriteFont font, font_larger, font_smallest, font_smaller;
        Texture2D button_texture, background;
        SoundEffect click_sound;

        public static Button button_play;
        public static Button button_exit;
        public static List<Button> buttons;

        public MenuState(Game1 game,GraphicsDevice graphics, ContentManager content)
            : base(game, graphics, content)
        {
            font = _content.Load<SpriteFont>("Fonts/font");
            font_larger = _content.Load<SpriteFont>("Fonts/font_larger");
            font_smaller = _content.Load<SpriteFont>("Fonts/font_smaller");
            font_smallest = _content.Load<SpriteFont>("Fonts/font_smallest");

            button_texture = _content.Load<Texture2D>("Controls/button");
            background = _content.Load<Texture2D>("Sprites/menu-background");

            click_sound = _content.Load<SoundEffect>("Sound Effects/interface1");

            button_play = new Button(button_texture, new Vector2(Game1.screenWidth / 2 - button_texture.Width / 2, Game1.screenHeight / 2 - button_texture.Height / 2), font, "Play");
            button_exit = new Button(button_texture, new Vector2(Game1.screenWidth / 2 - button_texture.Width / 2, Game1.screenHeight / 2 - button_texture.Height / 2 + 150), font, "Exit");
        }
        public override void LoadContent()
        {

        }

        public override void Update(GameTime gameTime)
        {

            button_play.Update(); 
            if (button_play.clicked) // Om klickar play --> spelet startar
            {
                click_sound.Play();
                Thread.Sleep(200);
                GameState.enemies.Clear();
                GameState.coins.Clear();
                GameState.platforms.Clear();
                GameState.movingPlatforms.Clear();
                GameState.backgroundTiles.Clear(); 

                _game.ChangeState(new GameState(_game, _graphics, _content));

                DeadState.savedPlayerScore = 0;
                DeadState.savedPlayerLevel = 3;
                Enemy.speed = 2f; // Börjanhastighet för fiender

                _game.IsMouseVisible = false;
            }

            button_exit.Update();
            if (button_exit.clicked)
            {
                click_sound.Play();
                Thread.Sleep(200);
                _game.Quit();  // Om klickar exit --> spelet avslutas
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _game.GraphicsDevice.Clear(Color.Green);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            button_play.Draw(spriteBatch);
            button_exit.Draw(spriteBatch);
            spriteBatch.DrawString(font_larger, "2DPlatformer", new Vector2(1280 / 2 - 250, 150), Color.White);
            spriteBatch.DrawString(font_smaller, "Controls:", new Vector2(100, 150), Color.White);
            spriteBatch.DrawString(font_smallest, "Move Left: A / LeftArrow", new Vector2(100 , 185), Color.White);
            spriteBatch.DrawString(font_smallest, "Move Right: D / RightArrow", new Vector2(100, 205), Color.White);
            spriteBatch.DrawString(font_smallest, "Jump: Space", new Vector2(100, 225), Color.White);
            spriteBatch.DrawString(font_smallest, "Climb Ladder: Hold Space", new Vector2(100, 245), Color.White);
            spriteBatch.DrawString(font_smallest, "Exit to menu: Escape", new Vector2(100, 265), Color.White);
            spriteBatch.End();
        }
    }
```

Esta classe tem as seguintes variáveis `font, font_larger, font_smallest, font_small,2D button_texture, background,SoundEffect click_sound`.

`button_play` e `button_exit` são objetos da classe `Button` que representam os botões "Play" e "Exit" no menu, respectivamente.`buttons`: é uma lista de objetos da classe `Button` que contém todos os botões no menu.

O construtor da classe `MenuState` carrega todas as texturas, fontes e sons necessários e cria os objetos de botão "Play" e "Exit".

O método `LoadContent()` está vazio, uma vez que os objetos são criados no construtor.

O método `Update()` verifica se os botões de jogo (Play e Exit) foram clicados e, em seguida, executa a ação correspondente. Se o botão Play for clicado, o jogo muda para o estado `GameState` e limpa todos os objetos do jogo, ficando tambem o mouse invisível. Se o botão Exit for clicado, o jogo é encerrado. 

O método `Draw()`  é utilizado para desenhar o menu principal.

----

### GameState:

Esta classe serve para gerir a inicialização do mapa, o relógio, câmara e o jogador.

```cs
public class GameState : State
    {

        // Listor som håller koll på olika objekt i spelet
        public static List<Platform> platforms = new List<Platform>();
        public static List<Enemy> enemies = new List<Enemy>();
        public static List<Coin> coins = new List<Coin>();
        public static List<MovingPlatform> movingPlatforms = new List<MovingPlatform>();
        public static List<BackgroundTile> backgroundTiles = new List<BackgroundTile>();
        public static List<Ladder> ladders = new List<Ladder>();

        // Texturer som används i spelet
        public static Texture2D grass2_texture, dirt_texture, grass_texture, grass3_texture, grass4_texture,
                                spike_texture, spike_right_texture, spike_top_texture,
                                sign_texture, spring_texture, coin_texture, ladder_texture, box_texture,
                                player_texture, player_walking_texture, player_jumping_texture,
                                enemy_walking_texture,
                                cloud_texture, water1_texture, water2_texture, mushroom_texture, plant1_texture, plant2_texture, plant3_texture;

        // Ljud som används i spelet
        public static SoundEffect jump_sound, enemy_death_sound, coin_sound, player_death_sound, win_sound, clock_sound;
        SoundEffectInstance clock_soundInstance;


        Song music; // Musik som spelas i spelet

        SpriteFont font;
        public static string score_str = "0";
        public static string timer_str = "60s";
        public static Vector2 score_pos, timer_pos;

        public static Player player; // Spelarobjektet
        Map map; // Klass som sköter spelets Map-generering
        Camera camera; // Klass som sköter kamerarörelserna

        float timer = 100; // Timer
        bool clockSoundPlayed = false;

        public GameState(Game1 game, GraphicsDevice graphics, ContentManager content)
            : base(game, graphics, content)
        {

        }
        public override void LoadContent()
        {
            // Laddar in alla texturer
            player_walking_texture = _content.Load<Texture2D>("Sprites/walk");
            player_jumping_texture = _content.Load<Texture2D>("Sprites/jump");
            player_texture = _content.Load<Texture2D>("Sprites/player");
            dirt_texture = _content.Load<Texture2D>("Sprites/ground6");
            grass2_texture = _content.Load<Texture2D>("Sprites/ground4");
            grass_texture = _content.Load<Texture2D>("Sprites/ground11");
            grass3_texture = _content.Load<Texture2D>("Sprites/ground10");
            grass4_texture = _content.Load<Texture2D>("Sprites/ground14");

            spike_texture = _content.Load<Texture2D>("Sprites/spike2");
            spike_right_texture = _content.Load<Texture2D>("Sprites/spike-right");
            spike_top_texture = _content.Load<Texture2D>("Sprites/spike-top");

            coin_texture = _content.Load<Texture2D>("Sprites/coin4");
            enemy_walking_texture = _content.Load<Texture2D>("Sprites/zombie-walk");
            sign_texture = _content.Load<Texture2D>("Sprites/pointer2");
            spring_texture = _content.Load<Texture2D>("Sprites/spring2");
            box_texture = _content.Load<Texture2D>("Sprites/box");
            cloud_texture = _content.Load<Texture2D>("Sprites/cloud");
            water1_texture = _content.Load<Texture2D>("Sprites/water1");
            water2_texture = _content.Load<Texture2D>("Sprites/water2");
            mushroom_texture = _content.Load<Texture2D>("Sprites/mushroom");
            ladder_texture = _content.Load<Texture2D>("Sprites/ladder");
            plant1_texture = _content.Load<Texture2D>("Sprites/grass1");
            plant2_texture = _content.Load<Texture2D>("Sprites/grass2");
            plant3_texture = _content.Load<Texture2D>("Sprites/grass3");

            // Laddar in alla ljud
            jump_sound = _content.Load<SoundEffect>("Sound Effects/jump01");
            player_death_sound = _content.Load<SoundEffect>("Sound Effects/death2");
            enemy_death_sound = _content.Load<SoundEffect>("Sound Effects/death1");
            coin_sound = _content.Load<SoundEffect>("Sound Effects/coin01");
            win_sound = _content.Load<SoundEffect>("Sound Effects/win");
            clock_sound = _content.Load<SoundEffect>("Sound Effects/ticking_clock2");
            clock_soundInstance = clock_sound.CreateInstance(); // Krävs för att kunna stoppa ljudet
            music = _content.Load<Song>("Music/love");

            MediaPlayer.IsRepeating = true; // Musiken loopas
            MediaPlayer.Volume = 0.1f;
            MediaPlayer.Play(music);

            player = new Player(player_texture, player_walking_texture, player_jumping_texture, new Vector2(46, 489));
            player.level = DeadState.savedPlayerLevel;
            player.savedScore = DeadState.savedPlayerScore;
            player.currentScore = player.savedScore;

            map = new Map();
            Map.Generate();
            camera = new Camera();

            font = _content.Load<SpriteFont>("Fonts/font");
            score_str = player.currentScore.ToString();
        }
        public override void Update(GameTime gameTime)
        {
            timer_str = Math.Round(timer, 0).ToString() + " s";
            timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (timer <= 0)
            {
                player.Die();
                clock_soundInstance.Stop();
            }
            if (timer < 10 && clockSoundPlayed == false)
            {
                clock_soundInstance.Play();
                clockSoundPlayed = true;
            }

            // Centerar spelaren
            camera.Follow(player);

            // Uppdaterar objekt i spelet
            player.Update(gameTime);
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime);
            }
            foreach (Coin coin in coins)
            {
                coin.Update();
            }
            foreach (MovingPlatform platform in movingPlatforms)
            {
                platform.Update();
            }

            if (player.position.X < 625) // När kameran inte är centrerad på spelaren ska score-visaren inte vara baserad på spelarens position
            {
                score_pos = new Vector2(_graphics.Viewport.Width - 100, 100);
                timer_pos = new Vector2(0, 100);
            }
            else if (player.position.X > 3390)
            {
                score_pos = new Vector2(3940, 100);

            }
            else
            {
                score_pos = new Vector2(player.position.X + (_graphics.Viewport.Width / 2) - 85, 100);
                timer_pos = new Vector2(player.position.X - (_graphics.Viewport.Width / 2), 100);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape)) // Återgår till menyn om man klickar Escape
            {
                MediaPlayer.Stop();
                enemies.Clear();
                coins.Clear();
                platforms.Clear();
                movingPlatforms.Clear();
                backgroundTiles.Clear();
                _game.ChangeState(new MenuState(_game, _graphics, _content));
                _game.IsMouseVisible = true; // Gör muspekaren synlig för menysammanhang
            }
            if (player.win)
            {
                timer = 100;
                Enemy.speed += 1f; // Ökar fiendernas hastighet för varje level
                clockSoundPlayed = false;
                player.win = false;
                player.position.X = 46;
                player.position.Y = 489;
                player.level++;
                player.savedScore = player.currentScore; // Sparar spelarens score då spelaren ska endast ska kunna förlora poängen för den aktuella leveln då han dör
                MediaPlayer.Stop();
                win_sound.Play();
                Thread.Sleep(2500);
                enemies.Clear();
                coins.Clear();
                platforms.Clear();
                movingPlatforms.Clear();
                backgroundTiles.Clear();
                ladders.Clear();
                Map.Generate();

                if (player.level >= 4)
                {
                    _game.ChangeState(new EndState(_game, _graphics, _content));
                    _game.IsMouseVisible = true;
                }
                else
                {
                    MediaPlayer.Play(music);
                }
            }
            if (player.isDead)
            {
                Thread.Sleep(200);
                _game.ChangeState(new DeadState(_game, _graphics, _content));
                _game.IsMouseVisible = true;
            }
        }
        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: camera.Transform);
            foreach (BackgroundTile backgroundTile in backgroundTiles)
            {
                backgroundTile.Draw(spriteBatch);
            }
            foreach (Ladder ladder in ladders)
            {
                ladder.Draw(spriteBatch);
            }
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(spriteBatch);
            }
            foreach (Platform platform in platforms)
            {
                platform.Draw(spriteBatch);
            }
            foreach (Coin coin in coins)
            {
                coin.Draw(spriteBatch);
            }
            foreach (MovingPlatform platform in movingPlatforms)
            {
                platform.Draw(spriteBatch);
            }
            player.Draw(spriteBatch);

            spriteBatch.DrawString(font, "Time:", new Vector2(timer_pos.X + 50, timer_pos.Y - 50), Color.Yellow);
            if (timer > 10)
            {
                spriteBatch.DrawString(font, timer_str, new Vector2(timer_pos.X + 50, timer_pos.Y), Color.White);
            }
            else // Röd text om timern är under 10 sekunder
            {
                spriteBatch.DrawString(font, timer_str, new Vector2(timer_pos.X + 50, timer_pos.Y), Color.Red);
            }id Update(GameTime gameTime)
```

A classe `GameState` é uma classe que contém uma lista de objetos do jogo, texturas e sons e que serve para definir o estado atual do jogo. Nela existe um método `LoadContent()` que carrega todas as texturas e sons necessários, inicializa o mapa e a câmera do jogo, e configura o objeto do jogador.
O método `Update()` atualiza o temporizador do jogo, que é usado para limitar o tempo de jogo, e faz com que o jogador morra se o tempo acabar. Também é verificado se o som do relógio deve ser reproduzido ou não.

---

### EndState:

----

### DeadState:

----

### Enemy:

O objetivo desta classe é a criação de um inimigo e a sua movimentação.

```cs
public class Enemy
    {
        Texture2D texture;
        public Vector2 position;
        public static float speed = 2f;
        public Vector2 velocity;
        public Rectangle rectangle, turningRectangle, rectangleHead, rectangleRight, rectangleLeft;
        bool intersected, isGrounded;
        bool movingRight;
        public bool isDead = false;
        Animation animation;
        int framesX = 10;

        public Enemy(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
            velocity.X = speed;
            movingRight = true;
            animation = new Animation(texture, framesX, 0.1f);
        }

        public void Update(GameTime gameTime)
        {

            #region Uppdatering

            position += velocity;
            rectangle = new Rectangle((int)position.X + 10, (int)position.Y + 20, (int)texture.Width / framesX - 10, texture.Height - 30);
            rectangleHead = new Rectangle((int)position.X + 10, (int)position.Y, (int)texture.Width / framesX - 10, 10);
            rectangleLeft = new Rectangle((int)position.X, (int)position.Y, 1, texture.Height - 10);
            rectangleRight = new Rectangle((int)position.X + texture.Width / framesX, (int)position.Y, 1, texture.Height - 10);
            animation.Update();

            if (movingRight)
            {
                // Rektangel som är fiendens bredd längre ut än fienden själv i riktningen som fienden rör sig
                turningRectangle = new Rectangle((int)position.X + texture.Width / framesX, (int)position.Y, (int)texture.Width / framesX, (int)texture.Height); 
            }
            else if (!movingRight)
            {
                turningRectangle = new Rectangle((int)position.X - texture.Width / framesX, (int)position.Y, (int)texture.Width / framesX, (int)texture.Height);
            }

            #endregion

            intersected = false;
            foreach (Platform platform in GameState.platforms)
            {
                if (rectangleRight.Intersects(platform.rectangle) || rectangleLeft.Intersects(platform.rectangle))
                {
                    Turn();
                }
                else if (turningRectangle.Intersects(platform.rectangle))
                {
                    intersected = true;
                }
            }
            isGrounded = intersected;

            if (isGrounded == false) // Om turningRectangle inte längre rör plattformen vänder fienden håll
            {
                Turn();
            }
        }

        public void Turn()
        {
            velocity.X = -velocity.X;
            if (movingRight)
            {
                movingRight = false;
            }
            else if (!movingRight)
            {
                movingRight = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (movingRight)
            {
                animation.Draw(position, spriteBatch, SpriteEffects.None);
            }
            else
            {
                animation.Draw(position, spriteBatch, SpriteEffects.FlipHorizontally);
            }

        }
    }
```

A classe Enemy tem as propriedades `texture`, `position`, `speed`, um vetor `velocity`, um retângulo, quatro bools: `intersected, isGrounded, movingRight e isDead`. Por fim, o inimigo têm uma animação com 10 frames.

Esta classe tem um construtor, que apenas recebe a textura e a posição do inimigo.

Na função `Update()` o inimigo verifica se consegue continuar a andar em frente, no caso de ser incapaz de seguir a mesma direção, este executa a função `Turn()`, e começa a mover-se na direção oposta.

A função `Turn` inverte a direção em que o personagem se move, através da alterção do vetor `velocity` e do bool `movingRight`.

Por fim, a função `Draw()` desenha a sprite do inimigo e, quando necessário, inverte a orientação da sprite.

----

### Player:

A classe "Player" representa o jogador e inclui várias propriedades, variáveis e objetos de controlo que são usados para determinar o estado e o comportamento do jogador durante o jogo.

```cs
public class Player
    {
        Texture2D texture, walking_texture, jumping_texture;
        SoundEffect jump_sound, enemy_death_sound, coin_sound, death_sound;

        public Vector2 position, velocity;
        public bool hasJumped, isGrounded, blockedLeft, blockedRight;
        public Rectangle rectangle, rectangleFeet, rectangleHead, rectangleLeft, rectangleRight;
        public int currentScore = 0;
        public int savedScore = 0;
        public int level;
        float speed = 4f;
        public bool win = false;
        bool justJumped = false; // Används för att ta bort förmågan att hålla in Space för att hoppa
        bool touchingLadder = false;
        public bool isDead = false;
        Animation walk_animation;

        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        SpriteEffects s = SpriteEffects.FlipHorizontally;

public Player(Texture2D newTexture, Texture2D newWalkingTexture, Texture2D newJumpingTexture, Vector2 newPosition)
        {
            texture = newTexture;
            walking_texture = newWalkingTexture;
            jumping_texture = newJumpingTexture;
            position = newPosition;
            hasJumped = true;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);
            walk_animation = new Animation(walking_texture, 10, 0.1f);
            jump_sound = GameState.jump_sound;
            enemy_death_sound = GameState.enemy_death_sound;
            coin_sound = GameState.coin_sound;
            death_sound = GameState.player_death_sound;
        }
```

O código acima é o construtor da classe Player, que recebe quatro parâmetros: `newTexture`, `newWalkingTexture`, `newJumpingTexture` e `newPosition`.
O primeiro parâmetro, `newTexture`, é uma textura que será usada para desenhar o jogador enquanto *idle*.
O segundo parâmetro, `newWalkingTexture`, é uma textura que será usada para desenhar o jogador enquanto ele está em movimento.
O terceiro parâmetro, `newJumpingTexture`, é uma textura que será usada para desenhar o jogador enquanto ele a saltar.
O quarto parâmetro, `newPosition`, é a posição inicial do jogador no jogo.

```cs
public void Die()
        {
            isDead = true;
            position.X = 46;
            position.Y = 489;  
            death_sound.Play(0.1f, 0, 0);
            GameState.enemies.Clear();
            GameState.coins.Clear();
            GameState.platforms.Clear();
            GameState.movingPlatforms.Clear();
            Map.Generate();
            currentScore = savedScore;
            GameState.score_str = currentScore.ToString()
```

Este é um método na classe `Player` que é usado para definir o comportamento do jogador quando ele morre no jogo.
O método define a variável `isDead` como `true` para indicar que o jogador está morto. A posição do jogador é redefinida para uma posição pré-definida (X=46, Y=489) no mapa, onde o jogador reaparece após morrer. O som de morte do jogador é reproduzido usando a propriedade `death_sound`.
Em seguida, as listas de inimigos, moedas e plataformas no jogo (GameState.enemies, GameState.coins, GameState.platforms e GameState.movingPlatforms) são limpas, o mapa é gerado novamente e a pontuação atual é definida como a pontuação salva anteriormente (savedScore). A pontuação atualizada é então convertida numa string e armazenada na propriedade `score_str` de `GameState`.

```cs
public void Update(GameTime gameTime)
        {
            #region Uppdatering variabler

            walk_animation.Update();
            position += velocity;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height); // Rektangel för spelaren 
            rectangleFeet = new Rectangle((int)position.X + 5, (int)position.Y + (int)texture.Height, (int)texture.Width - 5, 1); // Rektangel för spelarens botten
            rectangleHead = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, 1); // Rektangel för spelarens topp
            rectangleLeft = new Rectangle((int)position.X - 10, (int)position.Y, 1, (int)texture.Height - 1); // Rektangel för spelarens vänstra sida
            rectangleRight = new Rectangle((int)position.X + (int)texture.Width + 10, (int)position.Y, 1, (int)texture.Height - 3); //Rektangel för spelarens högra sida

            #endregion

            #region Input

            if (Keyboard.GetState().IsKeyDown(Keys.Right) && blockedRight == false || 
                Keyboard.GetState().IsKeyDown(Keys.D) && blockedRight == false)
            {
                velocity.X = speed;
                s = SpriteEffects.None;
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Left) && blockedLeft == false ||
                     Keyboard.GetState().IsKeyDown(Keys.A) && blockedLeft == false)
            {
                velocity.X = -speed;
                s = SpriteEffects.FlipHorizontally;
            }
            else
            {
                velocity.X = 0;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && touchingLadder)
            {
                velocity.Y = -3f; // Spelaren klättrar på stege
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Space) && hasJumped == false && justJumped == false)
            {
                hasJumped = true;
                velocity.Y = -10f;
                isGrounded = false;
                justJumped = true;
            }
            else if ((Keyboard.GetState().IsKeyUp(Keys.Space)))
            {
                justJumped = false;
            }

            // Varierande hopp beroende på hur länge man trycker Space
            currentKeyboardState = Keyboard.GetState();
            if (previousKeyboardState.IsKeyDown(Keys.Space) && currentKeyboardState.IsKeyUp(Keys.Space) && velocity.Y < 0) // Varje gång man släpper Space
            {
                velocity.Y = -3f;
                hasJumped = true;
            }
            previousKeyboardState = currentKeyboardState;
            #endregion

            #region Gravity
            if (hasJumped && !touchingLadder)
            {
                velocity.Y += 0.5f;
                isGrounded = false;
            }

            if (touchingLadder && Keyboard.GetState().IsKeyUp(Keys.Space))
            {
                velocity.Y += 0.5f;
            }

            if (isGrounded == false)
            {
                hasJumped = true;
            }
            if (isGrounded && velocity.Y >= 0 && !touchingLadder)
            {
                velocity.Y = 0f;
            }
            if (position.Y > 720)
            {
                Die();
            }
            #endregion

            #region Kollisioner med plattformar

            bool intersectedFeet = false;
            bool intersectedLeft = false;
            bool intersectedRight = false;

            for (int i = 0; i < GameState.platforms.Count; i++)
            {
                if (rectangle.Intersects(GameState.platforms[i].rectangle) && GameState.platforms[i].texture == GameState.sign_texture) // om spelaren rör en skylt ska han vinna
                {
                    win = true;
                }
                if (rectangleFeet.Intersects(GameState.platforms[i].rectangleTop) && velocity.Y >= 0)
                {
                    if (GameState.platforms[i].isDeadly)
```

A função `Update` é responsável por atualizar o estado do jogo a cada frame. Ela começa a atualizar algumas variáveis relacionadas à animação do personagem, como a posição e a forma do personagem.
Em seguida, há uma seção de entrada, que verifica os inputs para movimentar o personagem. Se a tecla da direita ou "D" for pressionada, o personagem move-se para a direita. Se a tecla da esquerda ou "A" for pressionada, o personagem move-se para a esquerda. A tecla de espaço é usada para saltar ou subir escadas, dependendo da situação do personagem.
Depois, há uma seção de gravidade que atualiza a velocidade do personagem com base na gravidade e na interação com a escada ou a plataforma. A seção de colisão detecta se o personagem está em contato com as plataformas e atualiza a posição e a velocidade dele de acordo com as interações. Além disso, verifica se o personagem está em contato com uma placa, que indica que ele venceu o jogo. Se ele estiver em contato com uma plataforma que é fatal, ele morre.
Finalmente, há uma verificação para ver se o personagem está abaixo da tela, o que significa que ele morreu e precisa ser reiniciado.

----

### Coin:

Esta classe tem como objetivo a crianção de uma moeda animada.

```cs
       public class Coin
    {
        public Texture2D texture;
        public Vector2 position;
        public Rectangle rectangle;
        Animation animation;
        public Coin(Texture2D newTexture, Vector2 newPosition)
        {
            texture = newTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, texture.Width / 6, texture.Height);
            animation = new Animation(texture, 6, 0.1f);
        }

        public void Update()
        {
            animation.Update();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            animation.Draw(position, spriteBatch, SpriteEffects.None);
        }


    }
```

Esta classe utiliza as seguintes variáveis: `texture` , `position` e os seguintes objetos: `rectangle` que vai ser utilizado para detetar colisões e  `animation` que contém as animações da moeda.  

É utilizado um construtor `Coin(Texture2D newTexture, Vector2 newPosition)`  que recebe como parâmetros a posição e a textura da moeda.

Dentro do construtor são atribuídos novos valores a estes parâmetros. O `rectangle` é criado utilizando o tamanho e a posição da textura. A `animation` é criada com a textura e tem o número de frames e a velocidade dos mesmos.

A função `Update()` é utilizada para atualizar a animação da moeda.

A função `Draw()` é utilizada para desenhar a moeda na posição correta da tela.

-----

### Animation:

Esta classe tem como objetivo gerir as animações.

```cs
public class Animation
    {
        Texture2D texture; // Spritesheet
        List<Rectangle> sourceRectangles = new List<Rectangle>(); // Lista av enskilda frames i spritesheet
        int frames; // Antal frames i spritesheet
        int frame; // Aktiva framen
        float frameTime; // Hur länge varje frame ska visas
        float frameTimeLeft; 
        public Animation(Texture2D newTexture, int framesX, float newFrameTime)
        {
            texture = newTexture;
            frameTime = newFrameTime;
            frameTimeLeft = frameTime;
            frames = framesX;

            var frameWidth = texture.Width / framesX;
            var frameHeight = texture.Height;

            for (int i = 0; i < frames; i++) // Lägger till en rektangel för varje frame i spritesheet
            {
                sourceRectangles.Add(new Rectangle(i * frameWidth, 0, frameWidth, frameHeight));
            }
        }

        public void Update()
        {
            frameTimeLeft -= Game1.TotalSeconds;
            if (frameTimeLeft <= 0)
            {
                frameTimeLeft += frameTime;
                frame = (frame + 1) % frames; // Flyttar aktiva framen ett steg framåt i spritesheeten
            }
        }

        public void Draw(Vector2 position, SpriteBatch spriteBatch, SpriteEffects s)
        {
            spriteBatch.Draw(texture, position, sourceRectangles[frame], Color.White, 0, Vector2.Zero, Vector2.One, s, 1); // Ritar enbart ut aktiva framen (sourceRectangles[frame])
        }

    }
    ```

A animação tem as propriedades `texture`, uma lista de quadrados que representa os quadrados da spritesheet, frames, que nos indica a quantidade de quadrados da spritesheet, frame, que representa o frame que está a ser utilizado, `frameTime`, que nos indica por quanto tempo cada frame deve ser mostrado, e `frameTimeLeft`, que vai contar o tempo até a mudança de frame.

O construtor da animação recebe a textura, o tempo que cada frame deve ficar no ecrã e a quantidade de frames na animação.

A função `Update()` conta o tempo necessário antes de passar para o próximo frame e, quando o tempo chega ao fim avança para o frame a seguir. 

A função `Draw()` vai desenhar apenas o frame da animação desejado.

------

### Button:

Esta classe tem como objetivo a criação de um botão para o menu, o qual vai ser possível interagir com o rato.

```cs
      public class Button
    {
        Texture2D texture;
        Vector2 position;
        Rectangle rectangle;
        SpriteFont font;
        MouseState lastMouseState, mouseState;
        public bool clicked = false;
        public string text;

        Color background_color = Color.White;
        Color text_color = Color.Black;

        public Button(Texture2D newTexture, Vector2 newPosition, SpriteFont newFont, string newText)
        {
            texture = newTexture;
            position = newPosition;
            rectangle = new Rectangle((int)position.X, (int)position.Y, (int)texture.Width, (int)texture.Height);
            font = newFont;
            text = newText;
        }

        public void Update()
        {
            mouseState = Mouse.GetState();
            Rectangle cursor = new Rectangle(mouseState.X, mouseState.Y, 10, 10); // Rektangel för muspekaren
            if (cursor.Intersects(rectangle))
            {
                background_color = Color.DarkGray;
                text_color = Color.White;
                if (mouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton == ButtonState.Released) // Om man klickar
                {
                    clicked = true; // När clicked är true hanteras händelser i klassen där knappen finns
                }
            }
            else
            {
                background_color = Color.White;
                text_color = Color.Black;
            }
            lastMouseState = mouseState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, background_color);
            spriteBatch.DrawString(font, text, new Vector2(position.X + 100, position.Y + 15), text_color);
        }
    }
```

A classe `Button` tem como propriedades a sua textura, a sua posição, um retângulo que representa o espaço ocupado pelo botão, o tipo de letra utilizado pelo botão, o estado do rato, e por fim um boleano que diz se o botão foi pressionado ou não.

Na função `Update` utilizamos um retângulo colocado no lugar em que se encontra o ponteiro do rato e verifica-se se o ponteiro interseta o botão, se isto acontecer, a cor do botão e do texto muda, e a seguir verifica-se se o botão foi pressionado, caso ele seja, muda-se o bool `clicked` para true.

A função `Draw` desenha a textura do botão na posição desejada e depois escreve o texto desejado no botão.

---------
