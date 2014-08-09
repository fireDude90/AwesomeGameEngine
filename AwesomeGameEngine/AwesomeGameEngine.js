var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var AwesomeGameEngine;
(function (AwesomeGameEngine) {
    /**
    * A thing in the game which updates along with the rest of the game
    */
    var Entity = (function () {
        /**
        * Creates a new Entity
        * @param name The name to be used to access this entity. Should be unique
        * @param update Function to be called on game update
        * @constructor
        */
        function Entity(Name, update) {
            if (typeof update === "undefined") { update = function (delta) {
            }; }
            this.Name = Name;
            this.update = update;
        }
        /**
        * Updates the entity
        * @api private
        */
        Entity.prototype.Update = function (delta) {
            this.update(delta);
        };
        return Entity;
    })();
    AwesomeGameEngine.Entity = Entity;

    

    /** Anything which can be drawed to the screen */
    var Drawable = (function (_super) {
        __extends(Drawable, _super);
        /**
        * Creates a new Drawable
        * @param name Entity name
        * @param update Function to be called on game update
        * @param draw Function to be called on game draw
        * @constructor
        */
        function Drawable(name, update, draw) {
            if (typeof update === "undefined") { update = function (delta) {
            }; }
            if (typeof draw === "undefined") { draw = function (game) {
            }; }
            _super.call(this, name, update);
            this.draw = draw;
        }
        /** Draws this */
        Drawable.prototype.Draw = function (game) {
            this.draw(game);
        };

        /**
        * Tests for collision between two entities
        *
        * @param a First entity
        * @param b Second entity
        * @returns True if they intersect, false otherwise
        */
        Drawable.Collides = function (a, b) {
            return a.GetBoundingBox().Intersects(b.GetBoundingBox());
        };
        return Drawable;
    })(Entity);
    AwesomeGameEngine.Drawable = Drawable;
})(AwesomeGameEngine || (AwesomeGameEngine = {}));
var AwesomeGameEngine;
(function (AwesomeGameEngine) {
    /** Game class. */
    var Game = (function () {
        /**
        * Creates a new game
        *
        * @param selector Target canvas selector
        * @param backgroundColor Canvas clear color
        * @param fps Amount of frames per second
        */
        function Game(canvasSelector, resourceSelector, backgroundColor, fps) {
            if (typeof canvasSelector === "undefined") { canvasSelector = "canvas"; }
            if (typeof resourceSelector === "undefined") { resourceSelector = "#resources"; }
            if (typeof backgroundColor === "undefined") { backgroundColor = "lightcoral"; }
            if (typeof fps === "undefined") { fps = 60; }
            this.backgroundColor = backgroundColor;
            this.fps = fps;
            this.timestamp = 0;
            this.scenes = {};
            this.currentScene = void (0);
            var canvas = document.querySelector(canvasSelector);
            this.Context = canvas.getContext('2d');
            this.contextDimensions = new AwesomeGameEngine.Vector2(canvas.width, canvas.height);

            this.Initialize(resourceSelector);
        }
        /**
        * Adds a new scene to the scene list
        */
        Game.prototype.AddScene = function (scene) {
            this.scenes[scene.Name] = scene;
        };

        /** Sets the current scene to be displayed */
        Game.prototype.SetScene = function (name) {
            this.currentScene = name;
        };

        /**
        * Called before the game begins.
        * Initializes all systems
        */
        Game.prototype.Initialize = function (selector) {
            AwesomeGameEngine.InitializeResources(selector);
            AwesomeGameEngine.InitializeInput(this);
        };

        /**
        * Starts the game
        */
        Game.prototype.Start = function () {
            window.requestAnimationFrame(this.Draw.bind(this));
        };

        /**
        * Draws all game elements
        */
        Game.prototype.Draw = function (timestamp) {
            this.Context.fillStyle = this.backgroundColor;
            this.Context.fillRect(0, 0, this.contextDimensions.x, this.contextDimensions.y);

            this.scenes[this.currentScene].Update((timestamp - this.timestamp) / 1000);
            this.scenes[this.currentScene].Draw(this);

            this.timestamp = timestamp;

            window.requestAnimationFrame(this.Draw.bind(this));
        };
        return Game;
    })();
    AwesomeGameEngine.Game = Game;
})(AwesomeGameEngine || (AwesomeGameEngine = {}));
var AwesomeGameEngine;
(function (AwesomeGameEngine) {
    function InitializeInput(game) {
        KeyboardState.Initialize();
        MouseState.Initialize(game);
    }
    AwesomeGameEngine.InitializeInput = InitializeInput;

    /**
    * Stores the current keys pressed
    * Can be accessed with the indexer operator
    */
    var KeyboardState = (function () {
        function KeyboardState() {
        }
        KeyboardState.Initialize = function () {
            var _this = this;
            /**
            * Figures out which key was pressed
            * @param key Keycode
            * @returns String representing which key was pressed
            */
            function MapKey(key) {
                switch (key) {
                    case 37:
                        return 'LEFT';
                    case 38:
                        return 'UP';
                    case 39:
                        return 'RIGHT';
                    case 40:
                        return 'DOWN';
                    default:
                        return String.fromCharCode(key);
                }
            }
            window.addEventListener('keydown', function (event) {
                _this[MapKey(event.which)] = true;
            });

            window.addEventListener('keyup', function (event) {
                _this[MapKey(event.which)] = false;
            });
        };
        return KeyboardState;
    })();
    AwesomeGameEngine.KeyboardState = KeyboardState;

    var MouseState = (function () {
        function MouseState() {
        }
        MouseState.Initialize = function (game) {
            var _this = this;
            this.game = game;

            game.Context.canvas.addEventListener('mousemove', function (event) {
                var rect = game.Context.canvas.getBoundingClientRect();
                _this.Position = new AwesomeGameEngine.Vector2(event.clientX - rect.left, event.clientY - rect.top);
            });

            /**
            * Figures out which mouse button was clicked
            * @param button Event code
            * @returns String representing which button was clicked
            */
            function MapMouseButton(button) {
                switch (button) {
                    case 0:
                        return 'Left';
                    case 1:
                        return 'Scroll';
                    case 2:
                        return 'Right';
                }
            }

            // Stop the right click menu from appearing
            game.Context.canvas.addEventListener('contextmenu', function (event) {
                return event.preventDefault();
            });
            game.Context.canvas.addEventListener('mousedown', function (event) {
                _this.Buttons[MapMouseButton(event.button)] = true;
            });
            game.Context.canvas.addEventListener('mouseup', function (event) {
                _this.Buttons[MapMouseButton(event.button)] = false;
            });
        };
        MouseState.Buttons = {
            Left: false,
            Scroll: false,
            Right: false
        };
        return MouseState;
    })();
    AwesomeGameEngine.MouseState = MouseState;
})(AwesomeGameEngine || (AwesomeGameEngine = {}));
var AwesomeGameEngine;
(function (AwesomeGameEngine) {
    /**
    * Initializes all the game's resources
    *
    * @param selector The selector of the element which contains all the elements
    */
    function InitializeResources(selector) {
        var container = document.querySelector(selector);
        container.style.display = 'none';

        Images.Initialize(container.querySelectorAll('img'));
        Audio.Initialize(container.querySelectorAll('audio'));
    }
    AwesomeGameEngine.InitializeResources = InitializeResources;

    /**
    * Stores all loaded images
    * Can be accessed using the index operator
    */
    var Images = (function () {
        function Images() {
        }
        Images.Initialize = function (elements) {
            for (var i = 0; i < elements.length; i++) {
                this[(elements.item(i)).id] = elements.item(i);
            }
        };
        return Images;
    })();
    AwesomeGameEngine.Images = Images;

    /**
    * Stores all loaded audio
    * Can be accessed using the index operator
    */
    var Audio = (function () {
        function Audio() {
        }
        Audio.Initialize = function (elements) {
            for (var i = 0; i < elements.length; i++) {
                this[(elements.item(i)).id] = new AudioClip(elements.item(i));
            }
        };
        return Audio;
    })();
    AwesomeGameEngine.Audio = Audio;

    /**
    * Represents an audio clip, and allows operations on it
    * @api private
    */
    var AudioClip = (function () {
        /**
        * Creates a new AudioClip with the given
        * HTMLAudioElements
        *
        * @constructor
        */
        function AudioClip(element) {
            this.element = element;
        }
        /** Starts playing this AudioClip */
        AudioClip.prototype.Start = function () {
            this.Stop();
            this.element.play();
        };

        Object.defineProperty(AudioClip.prototype, "Paused", {
            /** Finds if this AudioClip is paused */
            get: function () {
                return this.element.paused;
            },
            /** Toggles paused/unpaused state */
            set: function (value) {
                if (value) {
                    this.element.pause();
                } else {
                    this.element.play();
                }
            },
            enumerable: true,
            configurable: true
        });


        /** Stops and resets this audio clip */
        AudioClip.prototype.Stop = function () {
            this.element.pause();
            this.element.currentTime = 0;
        };
        return AudioClip;
    })();
})(AwesomeGameEngine || (AwesomeGameEngine = {}));
var AwesomeGameEngine;
(function (AwesomeGameEngine) {
    var Scene = (function () {
        /**
        * Creates a new empty scene
        * @param Name Scene name
        * @constructor
        */
        function Scene(Name) {
            this.Name = Name;
            /**
            * The internal list
            *
            * @api private
            */
            this.entities = {};
        }
        /**
        * Updates this scene
        *
        * @param delta Time since last frame
        * @api private
        */
        Scene.prototype.Update = function (delta) {
            var _this = this;
            Object.keys(this.entities).forEach(function (name) {
                _this.entities[name].Update(delta);
            });
        };

        /**
        * Draws all drawable objects in this scene
        *
        * @param game The current game
        * @param context Canvas2D context
        *
        * @api private
        */
        Scene.prototype.Draw = function (game) {
            var _this = this;
            Object.keys(this.entities).forEach(function (name) {
                if (_this.entities[name] instanceof AwesomeGameEngine.Drawable) {
                    _this.entities[name].Draw(game);
                }
            });
        };

        /**
        * Adds an entity to the scene
        */
        Scene.prototype.Add = function (entity) {
            this.entities[entity.Name] = entity;
        };

        /**
        * Gets an entity from the scene
        */
        Scene.prototype.Get = function (name) {
            return this.entities[name];
        };

        /**
        * Removes an entity from the scene
        */
        Scene.prototype.Remove = function (name) {
            delete this.entities[name];
        };
        return Scene;
    })();
    AwesomeGameEngine.Scene = Scene;
})(AwesomeGameEngine || (AwesomeGameEngine = {}));
var AwesomeGameEngine;
(function (AwesomeGameEngine) {
    /** Possible directions to move in */
    (function (Directions) {
        Directions[Directions["Forward"] = 0] = "Forward";
        Directions[Directions["Right"] = 1] = "Right";
        Directions[Directions["Back"] = 2] = "Back";
        Directions[Directions["Left"] = 3] = "Left";
    })(AwesomeGameEngine.Directions || (AwesomeGameEngine.Directions = {}));
    var Directions = AwesomeGameEngine.Directions;
    ;

    /** An interface for drawing an image */
    var Sprite = (function (_super) {
        __extends(Sprite, _super);
        /**
        * Creates a new sprite
        * @param name Entity name
        * @param update Function to be called when the game updates
        * @param Image Image to be displayed
        * @param Position Initial position
        * @constructor
        */
        function Sprite(name, update, Image, Position) {
            _super.call(this, name, update, void (0));
            this.Image = Image;
            this.Position = Position;
            /** The scale of this Sprite */
            this.Scale = new AwesomeGameEngine.Vector2(1, 1);
            /** Rotation of the sprite in degrees */
            this.Rotation = 0;
        }
        /** Draws this sprite to the screen */
        Sprite.prototype.Draw = function (game) {
            _super.prototype.Draw.call(this, game);

            game.Context.save();

            var position = this.Position.Clone();
            if (this.Rotation !== 0) {
                game.Context.translate(position.x + (this.Image.width * this.Scale.x / 2), position.y + (this.Image.height * this.Scale.y / 2));
                game.Context.rotate(AwesomeGameEngine.MathHelpers.DegreeToRadian(this.Rotation));
                position = new AwesomeGameEngine.Vector2(this.Image.width * this.Scale.x, this.Image.height * this.Scale.y).Multiply(-0.5);
            }
            game.Context.drawImage(this.Image, position.x, position.y, this.Image.width * this.Scale.x, this.Image.height * this.Scale.y);

            this.GetBoundingBox().Draw(game);

            game.Context.restore();
        };

        /**
        * Moves this sprite in the given direction, accounting for rotation.
        *
        * @param direction Direction to move in
        * @param speed Amount to move by
        */
        Sprite.prototype.Move = function (direction, speed) {
            var angle = AwesomeGameEngine.MathHelpers.DegreeToRadian(this.Rotation + (90 * direction) + 90);

            // Not sure why cos and sin have to be negative
            this.Position.Add(new AwesomeGameEngine.Vector2(-Math.cos(angle), -Math.sin(angle)).Multiply(speed));
        };

        /**
        * Centers this sprite at the given location
        * @param position New position of the sprite
        */
        Sprite.prototype.Center = function (position) {
            this.Position = new AwesomeGameEngine.Vector2(position.x - (this.Image.width / 2), position.y - (this.Image.height / 2));
        };

        /** Gets the bounding box of this sprite as defined by the image */
        Sprite.prototype.GetBoundingBox = function () {
            return new AwesomeGameEngine.Rectangle(this.Position, new AwesomeGameEngine.Vector2(this.Image.width, this.Image.height));
        };

        /** Tints this sprite's image */
        Sprite.prototype.Tint = function (color) {
            throw new Error("Feature not yet implemented");
        };
        return Sprite;
    })(AwesomeGameEngine.Drawable);
    AwesomeGameEngine.Sprite = Sprite;
})(AwesomeGameEngine || (AwesomeGameEngine = {}));
var AwesomeGameEngine;
(function (AwesomeGameEngine) {
    var MathHelpers = (function () {
        function MathHelpers() {
        }
        MathHelpers.DegreeToRadian = function (angle) {
            return angle * (Math.PI / 180);
        };
        return MathHelpers;
    })();
    AwesomeGameEngine.MathHelpers = MathHelpers;

    var Color = (function () {
        function Color(r, g, b, a) {
            if (typeof a === "undefined") { a = 255; }
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }
        return Color;
    })();
    AwesomeGameEngine.Color = Color;

    var Rectangle = (function () {
        function Rectangle(position, size) {
            this.position = position;
            this.size = size;
        }
        Rectangle.prototype.Intersects = function (b) {
            return !(this.position.x > (b.position.x + b.size.x) || (this.position.x + this.size.x) < b.position.x || this.position.y > (b.position.y + b.size.y) || (this.position.y + this.size.y) < b.size.y);
        };

        Rectangle.prototype.Draw = function (game) {
            game.Context.beginPath();
            game.Context.rect(this.position.x, this.position.y, this.size.x, this.size.y);
            game.Context.lineWidth = 5;
            game.Context.strokeStyle = 'black';
            game.Context.stroke();
            game.Context.closePath();
        };
        return Rectangle;
    })();
    AwesomeGameEngine.Rectangle = Rectangle;

    var Vector2 = (function () {
        function Vector2(x, y) {
            this.x = x;
            this.y = y;
        }
        /** Copies this Vector2 */
        Vector2.prototype.Clone = function () {
            return new Vector2(this.x, this.y);
        };

        /** Adds another Vector2 to this Vector2 */
        Vector2.prototype.Add = function (b) {
            this.x += b.x;
            this.y += b.y;
        };

        /** Adds two Vector2's together */
        Vector2.Add = function (a, b) {
            return new Vector2(a.x + b.x, a.y + b.y);
        };

        /** Multiplies this Vector2 by a constant*/
        Vector2.prototype.Multiply = function (c) {
            this.x *= c;
            this.y *= c;

            return this;
        };
        return Vector2;
    })();
    AwesomeGameEngine.Vector2 = Vector2;
})(AwesomeGameEngine || (AwesomeGameEngine = {}));
