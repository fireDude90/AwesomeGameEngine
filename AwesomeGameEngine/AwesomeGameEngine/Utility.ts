module AwesomeGameEngine {
    export class MathHelpers {
        static DegreeToRadian(angle: number): number {
            return angle * (Math.PI / 180);
        }
    }

    export class Color {
        constructor(public r: number, public g: number, public b: number, public a: number = 255) { }
    }

    export class Rectangle {
        constructor(public position: Vector2, public size: Vector2) { }
        Intersects(b: Rectangle): boolean {
            return !(
                this.position.x > (b.position.x + b.size.x) ||
                (this.position.x + this.size.x) < b.position.x ||
                this.position.y > (b.position.y + b.size.y) ||
                (this.position.y + this.size.y) < b.size.y);
        }

        Draw(game: Game): void {
            game.Context.beginPath();
            game.Context.rect(this.position.x, this.position.y, this.size.x, this.size.y);
            game.Context.lineWidth = 5;
            game.Context.strokeStyle = 'black';
            game.Context.stroke();
            game.Context.closePath();
        }
    }

    export class Vector2 {
        constructor(public x: number, public y: number) { }
        /** Copies this Vector2 */
        Clone(): Vector2 {
            return new Vector2(this.x, this.y);
        }

        /** Adds another Vector2 to this Vector2 */
        Add(b: Vector2): void {
            this.x += b.x;
            this.y += b.y;
        }

        /** Adds two Vector2's together */
        static Add(a: Vector2, b: Vector2): Vector2 {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        /** Multiplies this Vector2 by a constant*/
        Multiply(c: number): Vector2 {
            this.x *= c;
            this.y *= c;

            return this;
        }
    }

}