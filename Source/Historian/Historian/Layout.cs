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
using System.Collections.Generic;
using System.Linq;

namespace KSEA.Historian
{
    public class Layout
    {
        public static readonly Layout Empty = new Layout();
        readonly List<IElement> elements = new List<IElement>();

        public static Layout Load(string name, ConfigNode node)
        {
            try
            {
                Layout layout = new Layout(name);

                foreach (ConfigNode child in node.GetNodes())
                {
                    IElement element = Element.Create(child.name);

                    if (element != null)
                    {
                        element.Load(child);
                        layout.AddElement(element);
                    }
                    else
                    {
                        Historian.Print("Failed to load layout element of type '{0}'.", child.name);
                    }
                }

                return layout;
            }
            catch
            {
                Historian.Print("Failed to load layout '{0}'.", name);
            }

            return Empty;
        }

        public string Name { get; } 

        public Layout()
        {
        }

        public Layout(string name)
        {
            this.Name = name;
        }

        public void Draw()
        {
            foreach (IElement element in elements)
            {
                element.Draw();
            }
        }

        public void DrawExcept(IEnumerable<string> names)
        {
            foreach (IElement element in elements)
            {
                if (!names.Contains(element.Name))
                    element.Draw();
            }
        }

        void AddElement(IElement element) => elements.Add(element);
    }
}