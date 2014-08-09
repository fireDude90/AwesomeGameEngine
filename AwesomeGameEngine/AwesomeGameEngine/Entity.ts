module AwesomeGameEngine {
    /**
     * A thing in the game which updates along with the rest of the game
     */
    export class Entity {
        /**
         * Creates a new Entity
         * @param name The name to be used to access this entity. Should be unique
         * @param update Function to be called on game update
         * @constructor
         */
        constructor(
            public Name: string,
            private update: (delta: number) => void = function (delta: number): void { }
            ) { }

        /**
         * Updates the entity
         * @api private
         */
        Update(delta: number): void {
            this.update(delta);
        }
    }

    /** Shows that this thing can have a box drawn around it */
    export interface IHasBounds {
        /** Gets this things bounding box */
        GetBoundingBox(): Rectangle;
    }

    /** Anything which can be drawed to the screen */
    export class Drawable extends Entity {
        /**
         * Creates a new Drawable
         * @param name Entity name
         * @param update Function to be called on game update
         * @param draw Function to be called on game draw
         * @constructor
         */
        constructor(name,
            update: (delta: number) => void = function (delta: number): void { },
            private draw: (game: Game) => void = function (game: Game): void { }) {
            super(name, update);
        }

        /** Draws this */
        Draw(game: Game): void {
            this.draw(game);
        }

        /**
         * Tests for collision between two entities
         * 
         * @param a First entity
         * @param b Second entity
         * @returns True if they intersect, false otherwise
         */
        static Collides(a: IHasBounds, b: IHasBounds): boolean {
            return a.GetBoundingBox().Intersects(b.GetBoundingBox());
        }
    }
} 