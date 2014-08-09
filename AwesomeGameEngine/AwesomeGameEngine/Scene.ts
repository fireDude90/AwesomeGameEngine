module AwesomeGameEngine {
    export class Scene {
        /**
         * The internal list 
         * 
         * @api private
         */
        private entities: { [name: string]: Entity; } = {};

        /**
         * Creates a new empty scene
         * @param Name Scene name
         * @constructor
         */
        constructor(public Name: string) { }

        /**
         * Updates this scene
         * 
         * @param delta Time since last frame
         * @api private
         */
        Update(delta: number): void {
            Object.keys(this.entities).forEach((name) => {
                this.entities[name].Update(delta);
            });
        }

        /**
         * Draws all drawable objects in this scene
         * 
         * @param game The current game
         * @param context Canvas2D context
         * 
         * @api private
         */
        Draw(game: Game): void {
            Object.keys(this.entities).forEach((name) => {
                if (this.entities[name] instanceof Drawable) {
                    (<Drawable>this.entities[name]).Draw(game);
                }
            });
        }

        /**
         * Adds an entity to the scene
         */
        Add(entity: Entity): void {
            this.entities[entity.Name] = entity;
        }

        /**
         * Gets an entity from the scene
         */
        Get(name: string): Entity {
            return this.entities[name];
        }

        /**
         * Removes an entity from the scene
         */
        Remove(name: string): void {
            delete this.entities[name];
        }
    }
}