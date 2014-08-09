module AwesomeGameEngine {
    export function InitializeInput(game: Game) {
        KeyboardState.Initialize();
        MouseState.Initialize(game);
    }

    /**
     * Stores the current keys pressed
     * Can be accessed with the indexer operator
     */
    export class KeyboardState {
        static Initialize(): void {
            /**
             * Figures out which key was pressed
             * @param key Keycode
             * @returns String representing which key was pressed
             */
            function MapKey(key: number): string {
                switch (key) {
                    case 37: return 'LEFT';
                    case 38: return 'UP';
                    case 39: return 'RIGHT';
                    case 40: return 'DOWN';
                    default: return String.fromCharCode(key);
                }
            }
            window.addEventListener('keydown', (event: KeyboardEvent) => {
                this[MapKey(event.which)] = true;
            });

            window.addEventListener('keyup', (event: KeyboardEvent) => {
                this[MapKey(event.which)] = false;
            });
        }
    }

    export class MouseState {
        /** Curent position of the mouse, relative to the canvas */
        public static Position: Vector2;
        public static Buttons = {
            Left: false,
            Scroll: false,
            Right: false
        };
        

        /** 
         * The game, relative to which the mouse position is.
         * @api private
         */
        private static game: Game; 
        static Initialize(game: Game): void {
            this.game = game;

            game.Context.canvas.addEventListener('mousemove', (event: MouseEvent) => {
                var rect = game.Context.canvas.getBoundingClientRect();
                this.Position = new Vector2(event.clientX - rect.left, event.clientY - rect.top);
            });

            /**
             * Figures out which mouse button was clicked
             * @param button Event code
             * @returns String representing which button was clicked
             */
            function MapMouseButton(button: number): string {
                switch (button) {
                    case 0: return 'Left';
                    case 1: return 'Scroll';
                    case 2: return 'Right';
                }
            }

            // Stop the right click menu from appearing
            game.Context.canvas.addEventListener('contextmenu', (event: MouseEvent) => event.preventDefault());
            game.Context.canvas.addEventListener('mousedown', (event: MouseEvent) => {
                this.Buttons[MapMouseButton(event.button)] = true;
            });
            game.Context.canvas.addEventListener('mouseup', (event: MouseEvent) => {
                this.Buttons[MapMouseButton(event.button)] = false;
            });
        } 
    }
} 