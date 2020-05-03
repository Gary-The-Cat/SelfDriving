using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breakout.Entities
{
    public class Text
    {
        public Text(SpriteFont font, string stringText, Color textColour)
        {
            this.Font = font;
            this.StringText = stringText;
            this.TextColour = textColour;
        }

        public SpriteFont Font;

        public string StringText;

        public Color TextColour;
    }
}
