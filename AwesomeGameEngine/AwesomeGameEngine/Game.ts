module AwesomeGameEngine {
    export interface Size {
        width: number;
        height: number
    }

    /** Game class. */
    export class Game {
        /** Canvas rendering context */
        public Context: CanvasRenderingContext2D;
        private contextDimensions: Vector2;
        private timestamp: number = 0;

        private scenes: { [name: string]: Scene } = {};
        private currentScene: string = void (0);

        /**
         * Creates a new game
         * 
         * @param selector Target canvas selector
         * @param backgroundColor Canvas clear color
         * @param fps Amount of frames per second
         */
        constructor(size: Size = { width: 500, height: 500 }, canvasSelector: string = "canvas", resourceSelector: string = "#resources", public backgroundColor: string = "lightcoral") {
            var canvas = <HTMLCanvasElement>document.querySelector(canvasSelector);
            canvas.width = size.width;
            canvas.height = size.height;
            this.Context = canvas.getContext('2d');
            this.contextDimensions = new Vector2(canvas.width, canvas.height);

            this.Initialize(resourceSelector);
        }

        /**
         * Adds a new scene to the scene list
         */
        AddScene(scene: Scene): void {
            this.scenes[scene.Name] = scene;
        }

        /** Sets the current scene to be displayed */
        SetScene(name: string): void {
            this.currentScene = name;
        }

        /**
         * Called before the game begins.
         * Initializes all systems
         */
        Initialize(selector: string): void {
            InitializeResources(selector);
            InitializeInput(this);
        }

        /**
         * Starts the game
         */
        Start(): void {
            window.requestAnimationFrame(this.Draw.bind(this));
        }

        /**
         * Draws all game elements
         */
        Draw(timestamp: number): void {
            this.Context.fillStyle = this.backgroundColor;
            this.Context.fillRect(0, 0, this.contextDimensions.x, this.contextDimensions.y);

            this.scenes[this.currentScene].Update((timestamp - this.timestamp) / 1000);
            this.scenes[this.currentScene].Draw(this);

            this.timestamp = timestamp;

            window.requestAnimationFrame(this.Draw.bind(this));
        }
    }
}