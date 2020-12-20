using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagementGame.UI
{
    class Div: UiElement
    {
        private LayoutDirection _layoutDirection = LayoutDirection.Horizontal;

        public LayoutDirection LayoutDirection { 
            get
            {
                return _layoutDirection;
            }
            set
            {
                _layoutDirection = value;
                Invalidate();
            }
        }

        private List<UiElement> children = new List<UiElement>();

        public void AppendChild(UiElement child)
        {
            children.Add(child);
            child.Parent = this;
            Invalidate();
        }

        public void RemoveChild(UiElement child)
        {
            children.Remove(child);
            child.Parent = null;
            Invalidate();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (var child in children)
            {
                child.Draw(spriteBatch);
            }
        }

        public override void Reposition()
        {
            if (Parent != null)
            {
                Parent.Reposition();
                return;
            }

            int width = 0;
            int height = 0;

            if (_layoutDirection == LayoutDirection.Horizontal)
            {
                foreach (var child in children)
                {
                    child.SetPosition(new Point(width + PaddingLeft, height + PaddingTop));
                    width += child.GetTotalWidth();
                }
            }
            else if (_layoutDirection == LayoutDirection.Vertical)
            {
                foreach (var child in children)
                {
                    child.SetPosition(new Point(width + PaddingLeft, height + PaddingTop));
                    height += child.GetTotalWidth();
                }
            }

            SetPosition(Point.Zero);

            InnerWidth = width;
            InnerHeight = height;
        }
    }

    enum LayoutDirection
    {
        Vertical,
        Horizontal
    }
}
