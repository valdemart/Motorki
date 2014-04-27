using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Motorki
{
    public struct MouseData
    {
        public int X, Y;
        public bool Left, Right, Center;
        public int old_X, old_Y;
        public bool old_Left, old_Right, old_Center;

        public MouseData(int x, int y, bool left, bool right, bool center,
                         int old_x, int old_y, bool old_left, bool old_right, bool old_center)
        {
            X = x;
            Y = y;
            Left = left;
            Right = right;
            Center = center;
            old_X = old_x;
            old_Y = old_y;
            old_Left = old_left;
            old_Right = old_right;
            old_Center = old_center;
        }
    }

    public enum KeyModifiers : int
    {
        None = 0,
        LeftShift = 1,
        RightShift = 2,
        Shift = LeftShift | RightShift,
        LeftControl = 4,
        RightControl = 8,
        Control = LeftControl | RightControl,
        LeftAlt = 16,
        RightAlt = 32,
        Alt = LeftAlt | RightAlt
    }

    public delegate void KeyboardEvent(Keys key, bool state, int modifiers);
    public delegate void MouseEvent(MouseData md);

    public class InputEvents : Microsoft.Xna.Framework.GameComponent
    {
        long[] keypress_times;
        public static long key_repeat_edge, key_repeat, mousekey_repeat_edge, mousekey_repeat;
        //last mouse states
        int mouse_x, mouse_y;
        long mbtnLeft, mbtnRight, mbtnCenter;

        /// <param name="key_repeat">time between key repeats (in milliseconds)</param>
        /// <param name="mousekey_repeat">time between key repeats (in milliseconds)</param>
        public InputEvents(Game game, long key_repeat_edge, long key_repeat, long mousekey_repeat_edge, long mousekey_repeat)
            : base(game)
        {
            keypress_times = new long[Enum.GetNames(typeof(Keys)).Length];
            for (int i = 0; i < keypress_times.Length; i++)
                keypress_times[i] = 0;
            InputEvents.key_repeat_edge = key_repeat_edge;
            InputEvents.key_repeat = key_repeat;
            InputEvents.mousekey_repeat_edge = mousekey_repeat_edge;
            InputEvents.mousekey_repeat = mousekey_repeat;
            MouseState ms = Mouse.GetState();
            mouse_x = ms.X;
            mouse_y = ms.Y;
            mbtnLeft = 0;
            mbtnRight = 0;
            mbtnCenter = 0;
        }

        public void CheckForEvents(GameTime gameTime)
        {
            //detect keyboard events
            Keys[] keys = Keyboard.GetState().GetPressedKeys();
            int modifiers = (keys.Contains(Keys.LeftShift) ? (int)KeyModifiers.LeftShift : 0) |
                            (keys.Contains(Keys.RightShift) ? (int)KeyModifiers.RightShift : 0) |
                            (keys.Contains(Keys.LeftControl) ? (int)KeyModifiers.LeftControl : 0) |
                            (keys.Contains(Keys.RightControl) ? (int)KeyModifiers.RightControl : 0) |
                            (keys.Contains(Keys.LeftAlt) ? (int)KeyModifiers.LeftAlt : 0) |
                            (keys.Contains(Keys.RightAlt) ? (int)KeyModifiers.RightAlt : 0);
            for (int i = 0; i < keypress_times.Length; i++)
            {
                if (keypress_times[i] != 0)
                {
                    if (keys.Contains((Keys)i))
                    {
                        //add current time
                        keypress_times[i] += gameTime.ElapsedGameTime.Milliseconds;
                        //check for key repeat event
                        if (keypress_times[i] - key_repeat_edge > key_repeat)
                        {
                            keypress_times[i] -= key_repeat;
                            if (KeyRepeated != null)
                                KeyRepeated((Keys)i, true, modifiers);
                        }
                    }
                    else
                    {
                        //clear time
                        keypress_times[i] = 0;
                        if (KeyReleased != null)
                            KeyReleased((Keys)i, false, modifiers);
                    }
                }
                else if (keys.Contains((Keys)i))
                {
                    keypress_times[i] += gameTime.ElapsedGameTime.Milliseconds;
                    if (KeyPressed != null)
                        KeyPressed((Keys)i, true, modifiers);
                }
            }

            //detect mouse events
            MouseState ms = Mouse.GetState();
            MouseData md = new MouseData(ms.X, ms.Y,
                                         ms.LeftButton == ButtonState.Pressed, ms.RightButton == ButtonState.Pressed, ms.MiddleButton == ButtonState.Pressed,
                                         mouse_x, mouse_y, mbtnLeft != 0, mbtnRight != 0, mbtnCenter != 0);

            if ((mouse_x != md.X) || (mouse_y != md.Y))
            {
                if (MouseMoved != null)
                    MouseMoved(md);
            }
            if ((mbtnLeft != 0) != md.Left)
            {
                if (MouseLeftChanged != null)
                    MouseLeftChanged(md);
                if (!md.Left)
                    mbtnLeft = 0;
                else
                    mbtnLeft += gameTime.ElapsedGameTime.Milliseconds;
            }
            else //left mouse button remained in its state
            {
                if (md.Left)
                {
                    mbtnLeft += gameTime.ElapsedGameTime.Milliseconds;
                    if (mbtnLeft - mousekey_repeat_edge > mousekey_repeat)
                    {
                        if (MouseLeftRepeat != null)
                            MouseLeftRepeat(md);
                        mbtnLeft -= mousekey_repeat;
                    }
                }
            }
            if ((mbtnRight != 0) != md.Right)
            {
                if (MouseRightChanged != null)
                    MouseRightChanged(md);
                if (!md.Right)
                    mbtnRight = 0;
                else
                    mbtnRight += gameTime.ElapsedGameTime.Milliseconds;
            }
            else //right mouse button remained in its state
            {
                if (md.Right)
                {
                    mbtnRight += gameTime.ElapsedGameTime.Milliseconds;
                    if (mbtnRight - mousekey_repeat_edge > mousekey_repeat)
                    {
                        if (MouseRightRepeat != null)
                            MouseRightRepeat(md);
                        mbtnRight -= mousekey_repeat;
                    }
                }
            }
            if ((mbtnCenter != 0) != md.Center)
            {
                if (MouseCenterChanged != null)
                    MouseCenterChanged(md);
                if (!md.Center)
                    mbtnCenter = 0;
                else
                    mbtnCenter += gameTime.ElapsedGameTime.Milliseconds;
            }
            else //center mouse button remained in its state
            {
                if (md.Center)
                {
                    mbtnCenter += gameTime.ElapsedGameTime.Milliseconds;
                    if (mbtnCenter - mousekey_repeat_edge > mousekey_repeat)
                    {
                        if (MouseCenterRepeat != null)
                            MouseCenterRepeat(md);
                        mbtnCenter -= mousekey_repeat;
                    }
                }
            }
            mouse_x = md.X;
            mouse_y = md.Y;
        }

        public static event KeyboardEvent KeyPressed;
        public static event KeyboardEvent KeyRepeated;
        public static event KeyboardEvent KeyReleased;

        public static event MouseEvent MouseMoved;
        public static event MouseEvent MouseLeftChanged;
        public static event MouseEvent MouseRightChanged;
        public static event MouseEvent MouseCenterChanged;
        public static event MouseEvent MouseLeftRepeat;
        public static event MouseEvent MouseRightRepeat;
        public static event MouseEvent MouseCenterRepeat;
    }
}
