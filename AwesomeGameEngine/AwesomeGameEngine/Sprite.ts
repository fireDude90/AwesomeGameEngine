module AwesomeGameEngine {
    /** Possible directions to move in */
    export enum Directions {
        Forward,
        Right,
        Back,
        Left
    };

    /** An interface for drawing an image */
    export class Sprite extends Drawable implements IHasBounds {
        /** The scale of this Sprite */
        public Scale: Vector2 = new Vector2(1, 1);
        /** Rotation of the sprite in degrees */
        public Rotation: number = 0;

        /**
         * Creates a new sprite
         * @param name Entity name
         * @param update Function to be called when the game updates
         * @param Image Image to be displayed
         * @param Position Initial position
         * @constructor
         */
        constructor(name, update: (delta: number) => void,
            public Image: HTMLImageElement,
            public Position: Vector2) {
            super(name, update, void (0));
        }

        /** Draws this sprite to the screen */
        Draw(game: Game): void {
            super.Draw(game);

            game.Context.save();

            var position = this.Position.Clone();
            if (this.Rotation !== 0) {
                game.Context.translate(
                    position.x + (this.Image.width * this.Scale.x / 2),
                    position.y + (this.Image.height * this.Scale.y / 2));
                game.Context.rotate(MathHelpers.DegreeToRadian(this.Rotation));
                position = new Vector2(this.Image.width * this.Scale.x, this.Image.height * this.Scale.y).Multiply(-0.5);
            }
            game.Context.drawImage(this.Image,
                position.x, position.y,
                this.Image.width * this.Scale.x, this.Image.height * this.Scale.y);

            this.GetBoundingBox().Draw(game);

            game.Context.restore();
        }

        /** 
         * Moves this sprite in the given direction, accounting for rotation.
         * 
         * @param direction Direction to move in
         * @param speed Amount to move by
         */
        Move(direction: Directions, speed: number): void {
            var angle = MathHelpers.DegreeToRadian(this.Rotation + (90 * <number>direction) + 90);
            // Not sure why cos and sin have to be negative
            this.Position.Add(new Vector2(-Math.cos(angle), -Math.sin(angle)).Multiply(speed));
        }

        /**
         * Centers this sprite at the given location
         * @param position New position of the sprite
         */
        Center(position: Vector2): void {
            this.Position = new Vector2(position.x - (this.Image.width / 2), position.y - (this.Image.height / 2));
        }

        /** Gets the bounding box of this sprite as defined by the image */
        GetBoundingBox(): Rectangle {
            return new Rectangle(this.Position, new Vector2(this.Image.width, this.Image.height));
        }

        /** Tints this sprite's image */
        Tint(color: Color): void {
            throw new Error("Feature not yet implemented");
        }
    }
} 