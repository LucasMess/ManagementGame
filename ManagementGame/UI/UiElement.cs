using ManagementGame.Objects;
using ManagementGame.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.UI
{
    class UiElement
    {
        public bool IsHovered { get; private set; }
        public string Text { get; private set; }
        public int MarginLeft { get; set; }
        public int MarginTop { get; set; }
        public int MarginRight { get; set; }
        public int MarginBottom { get; set; }

        public int PaddingLeft { get; set; }
        public int PaddingTop { get; set; }
        public int PaddingRight { get; set; }
        public int PaddingBottom { get; set; }

        public int InnerWidth { get; set; }
        public int InnerHeight { get; set; }

        /// <summary>
        /// The position of an element in relation to the screen.
        /// </summary>
        public Point AbsolutePosition { get; protected set; }

        /// <summary>
        /// The position of an element in relation to its parent.
        /// </summary>
        public Point RelativePosition { get; protected set; }
        public Texture2D Texture { get; protected set; }

        /// <summary>
        /// The rectangle used to draw the UI element onto the screen. Includes padding
        /// but not margins.
        /// </summary>
        public Rectangle DrawRectangle { get; protected set; }
        public UiElement Parent { get; set; }

        private Subject<UiEvent> click = new Subject<UiEvent>();
        private Subject<UiEvent> hover = new Subject<UiEvent>();

        private SpriteFont font;

        private bool needsToCalcPositions = false;

        public IObservable<UiEvent> Click => click.AsObservable();
        public IObservable<UiEvent> Hover => hover.AsObservable();

        public UiElement()
        {
            InputManager.MousePosition.Subscribe(pos =>
            {
                if (DrawRectangle.Contains(pos))
                {
                    IsHovered = true;
                    hover.OnNext(new UiEvent() { Source = this });
                }
                else
                {
                    IsHovered = false;
                }
            });

            InputManager.LeftMouseButtonState.DistinctUntilChanged().Subscribe(mouseEvent =>
            {
                if (DrawRectangle.Contains(mouseEvent.Position))
                {
                    if (mouseEvent.Pressed)
                    {
                        click.OnNext(new UiEvent() { Source = this });
                    }
                }
            });

            font = ContentLoader.GetFont("x32");

            
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (needsToCalcPositions)
            {
                Reposition();
            }

            if (Texture != null)
            {
                spriteBatch.Draw(Texture, DrawRectangle, Color.White);
            }
            if (Text != null)
            {
                spriteBatch.DrawString(font, Text, DrawRectangle.Location.ToVector2() + new Vector2(PaddingLeft, PaddingTop), Color.White);
            }
        }

        public void SetPadding(int top, int right, int bottom, int left)
        {
            PaddingTop = top;
            PaddingRight = right;
            PaddingBottom = bottom;
            PaddingLeft = left;
            Invalidate();
        }

        public void SetMargin(int top, int right, int bottom, int left)
        {
            MarginTop = top;
            MarginRight = right;
            MarginBottom = bottom;
            MarginLeft = left;
            Invalidate();
        }

        public virtual void Reposition()
        {
            if (Parent != null)
            {
                Parent.Reposition();
            }
           
        }

        /// <summary>
        /// Tells the UI element to set its top-left corner to the given position
        /// and calculate its draw rectangle size and position from it.
        /// </summary>
        /// <param name="start"></param>
        public virtual void SetPosition(Point relativePosition)
        {
            RelativePosition = relativePosition + new Point(MarginLeft, MarginTop);
            AbsolutePosition = (Parent == null ? Point.Zero : Parent.AbsolutePosition) + RelativePosition;
            DrawRectangle = new Rectangle(AbsolutePosition.X, AbsolutePosition.Y, GetDrawWidth(), GetDrawHeight());
            needsToCalcPositions = false;
        }

        public virtual int GetInnerWidth()
        {
            if (Text != null)
            {
                return (int)font.MeasureString(Text).X + InnerWidth;
            }
            return InnerWidth;
        }

        public virtual int GetInnerHeight()
        {
            if (Text != null)
            {
                return (int)font.MeasureString(Text).Y + InnerHeight;
            }
            return InnerHeight;
        }

        public virtual int GetDrawWidth()
        {
            return GetInnerWidth() + PaddingLeft + PaddingRight;
        }

        public virtual int GetTotalWidth()
        {
            return GetDrawWidth() + MarginLeft + MarginRight;
        }

        public virtual int GetDrawHeight()
        {
            return GetInnerHeight() + PaddingTop + PaddingBottom;
        }

        public virtual int GetTotalHeight()
        {
            return GetDrawHeight() + MarginTop + MarginBottom;
        }

        public void Invalidate()
        {
            needsToCalcPositions = true;
            if (Parent != null)
            {
                Parent.Invalidate();
            }
        }

        public void SetText(string text)
        {
            Text = text;
            Invalidate();
        }

    }

    struct UiEvent
    {
        public UiElement Source;
    }
}
