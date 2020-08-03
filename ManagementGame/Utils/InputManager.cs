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
       
    }

    struct KeyEvent
    {
        public Keys Key;
        public KeyState State;
    }


}
