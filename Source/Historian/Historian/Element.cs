/*
	This file is part of Historian /L Unleashed
		© 2018-2021 Lisias T : http://lisias.net <support@lisias.net>
		© 2016-2018 Aelfhe1n
		© 2015-2016 Zeenobit

	Historian /L Unleashed is licensed as follows:
		* GPL 3.0 : https://www.gnu.org/licenses/gpl-3.0.txt

	Historian /L Unleashed is distributed in the hope that it will be
	useful, but WITHOUT ANY WARRANTY; without even the implied
	warranty of	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

	You should have received a copy of the GNU General Public License 3.0 along
	with Historian /L Unleashed. If not, see <https://www.gnu.org/licenses/>.

*/
using UnityEngine;

namespace KSEA.Historian
{
    public interface IElement
    {
        string Name { get; set; }

        void Draw();
        void Load(ConfigNode node);
    }

    public abstract class Element : IElement
    {
        public string Name { get; set; }

        protected Vector2 Anchor { get; set; }
        protected Vector2 Position { get; set; }
        protected Vector2 Size { get; set; }

        public static IElement Create(string type)
        {
            switch (type)
            {
                case "RECTANGLE":
                    return new Rectangle();
                case "TEXT":
                    return new Text();
                case "PICTURE":
                    return new Picture();
                case "FLAG":
                    return new Flag();
                case "SITUATION_TEXT":
                    return new SituationText();
                case "ACTION_TEXT":
                    return new ActionText();
                case "TEXT_LIST":
                    return new TextList();
                case "INHERIT":
                    return new InheritedLayout();
                default:
                    return null;
            }
        }

        public void Draw()
        {
            float width = Size.x * Screen.width;
            float height = Size.y * Screen.height;

            float left = (Position.x * Screen.width) - (Anchor.x * width);
            float top = (Position.y * Screen.height) - (Anchor.y * height);

            Rect bounds = new Rect(left, top, width, height);

            OnDraw(bounds);
        }

        public void Load(ConfigNode node)
        {
            Anchor = node.GetVector2("Anchor", Vector2.zero);
            Position = node.GetVector2("Position", Vector2.zero);
            Size = node.GetVector2("Size", Vector2.zero);
            Name = node.GetString("Name", string.Empty);

            OnLoad(node);
        }

        protected abstract void OnDraw(Rect bounds);
        protected abstract void OnLoad(ConfigNode node);
    }
}