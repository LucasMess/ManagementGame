using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using Microsoft.Xna.Framework.Input;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ManagementGame.Utils
{
    static class InputManager
    {
        private static Dictionary<Keys, ISubject<KeyEvent>> pressedSubjects = new Dictionary<Keys, ISubject<KeyEvent>>();
        private static Dictionary<Keys, ISubject<KeyEvent>> releasedSubjects = new Dictionary<Keys, ISubject<KeyEvent>>();
        
        private static Subject<MouseEvent> leftMouseButtonPressed = new Subject<MouseEvent>();
        private static Subject<MouseEvent> leftMouseButtonReleased = new Subject<MouseEvent>();

        private static Subject<MouseEvent> rightMouseButtonPressed = new Subject<MouseEvent>();
        private static Subject<MouseEvent> rightMouseButtonReleased = new Subject<MouseEvent>();

        private static Subject<MouseEvent> leftMouseButtonState = new Subject<MouseEvent>();
        private static Subject<MouseEvent> rightMouseButtonState = new Subject<MouseEvent>();

        private static Subject<Point> mousePosition = new Subject<Point>();

        public static void Update(GameTime gameTime)
        {
            foreach (var entry in pressedSubjects)
            {
                if (Keyboard.GetState().IsKeyDown(entry.Key))
                {
                    entry.Value.OnNext(new KeyEvent() { Key = entry.Key, State = KeyState.Down });
                }
            }

            foreach (var entry in releasedSubjects)
            {
                if (Keyboard.GetState().IsKeyUp(entry.Key))
                {
                    entry.Value.OnNext(new KeyEvent() { Key = entry.Key, State = KeyState.Up });
                }
            }
                      
            var mouseState = Mouse.GetState();
            var mousePos = new Point(mouseState.X, mouseState.Y);
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                leftMouseButtonPressed.OnNext(new MouseEvent() { Pressed = true, Position = mousePos });
                leftMouseButtonState.OnNext(new MouseEvent() { Pressed = true, Position = mousePos });
            } 
            else
            {
                leftMouseButtonReleased.OnNext(new MouseEvent() { Pressed = false, Position = mousePos });
                leftMouseButtonState.OnNext(new MouseEvent() { Pressed = false, Position = mousePos });
            }

            if (mouseState.RightButton == ButtonState.Pressed)
            {
                rightMouseButtonPressed.OnNext(new MouseEvent() { Pressed = true, Position = mousePos });
                rightMouseButtonState.OnNext(new MouseEvent() { Pressed = true, Position = mousePos });
            }
            else
            {
                rightMouseButtonReleased.OnNext(new MouseEvent() { Pressed = false, Position = mousePos });
                rightMouseButtonState.OnNext(new MouseEvent() { Pressed = false, Position = mousePos });
            }

            mousePosition.OnNext(mousePos);

        }

        public static IObservable<KeyEvent> KeyPressed(Keys key)
        {
            return MakeObservable(key, pressedSubjects);
        }

        public static IObservable<KeyEvent> KeyReleased(Keys key)
        {
            return MakeObservable(key, releasedSubjects);
        }

        private static IObservable<KeyEvent> MakeObservable(Keys key, Dictionary<Keys, ISubject<KeyEvent>> dict)
        {
            if (dict.ContainsKey(key))
            {
                return dict[key];
            }
            dict.Add(key, new Subject<KeyEvent>());
            return dict[key];
        }

        public static IObservable<MouseEvent> LeftMouseButtonPressed => leftMouseButtonPressed.AsObservable();

        public static IObservable<MouseEvent> LeftMouseButtonReleased => leftMouseButtonReleased.AsObservable();

        public static IObservable<MouseEvent> RightMouseButtonPressed => rightMouseButtonPressed.AsObservable();

        public static IObservable<MouseEvent> RightMouseButtonReleased => rightMouseButtonReleased.AsObservable();

        public static IObservable<MouseEvent> LeftMouseButtonState => leftMouseButtonState.AsObservable();

        public static IObservable<MouseEvent> RightMouseButtonState => rightMouseButtonState.AsObservable();

        public static IObservable<Point> MousePosition => mousePosition.AsObservable();
    }

    struct KeyEvent
    {
        public Keys Key;
        public KeyState State;
    }

    struct MouseEvent
    {
        public bool Pressed;
        public Point Position;
    }

}
