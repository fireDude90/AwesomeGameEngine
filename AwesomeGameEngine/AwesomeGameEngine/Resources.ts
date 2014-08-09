module AwesomeGameEngine {
    /**
     * Initializes all the game's resources
     *
     * @param selector The selector of the element which contains all the elements
     */
    export function InitializeResources(selector: string): void {
        var container = <HTMLElement>document.querySelector(selector);
        container.style.display = 'none';

        Images.Initialize(container.querySelectorAll('img'));
        Audio.Initialize(container.querySelectorAll('audio'));
    }

    /**
     * Stores all loaded images
     * Can be accessed using the index operator
     */
    export class Images {
        static Initialize(elements: NodeList): void {
            for (var i = 0; i < elements.length; i++) {
                this[(<HTMLElement>(elements.item(i))).id] = <HTMLImageElement>elements.item(i);
            }
        }
    }

    /**
     * Stores all loaded audio
     * Can be accessed using the index operator
     */
    export class Audio {
        static Initialize(elements: NodeList): void {
            for (var i = 0; i < elements.length; i++) {
                this[(<HTMLElement>(elements.item(i))).id] = new AudioClip(<HTMLAudioElement>elements.item(i));
            }
        }
    }

    /**
     * Represents an audio clip, and allows operations on it
     * @api private
     */
    class AudioClip {
        /**
         * Creates a new AudioClip with the given
         * HTMLAudioElements
         * 
         * @constructor
         */
        constructor(private element: HTMLAudioElement) { }

        /** Starts playing this AudioClip */
        Start(): void {
            this.Stop();
            this.element.play();
        }

        /** Finds if this AudioClip is paused */
        get Paused(): boolean {
            return this.element.paused;
        }

        /** Toggles paused/unpaused state */
        set Paused(value: boolean) {
            if (value) {
                this.element.pause();
            }
            else {
                this.element.play();
            }
        }

        /** Stops and resets this audio clip */
        Stop(): void {
            this.element.pause();
            this.element.currentTime = 0;
        }
    }
}