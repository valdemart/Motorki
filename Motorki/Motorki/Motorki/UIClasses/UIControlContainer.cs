using System.Collections.Generic;

namespace Motorki.UIClasses
{
    public class UIControlContainer : IList<UIControl>
    {
        protected List<UIControl> ChildControls;

        public UIControlContainer()
        {
            ChildControls = new List<UIControl>();
        }

        public UIControl this[int index]
        {
            get { return ChildControls[index]; }
            set { ChildControls[index] = value; }
        }

        public UIControl this[string name]
        {
            get
            {
                int index = 0;
                while (true)
                {
                    if (ChildControls[index].Name == name)
                        return ChildControls[index];
                    index++;
                }
            }
            set
            {
                int index = 0;
                while (true)
                {
                    if (ChildControls[index].Name == name)
                        ChildControls[index] = value;
                    index++;
                }
            }
        }

        public int IndexOf(UIControl item)
        {
            return ChildControls.IndexOf(item);
        }

        public void Insert(int index, UIControl item)
        {
            ChildControls.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            ChildControls.RemoveAt(index);
        }

        public void Add(UIControl item)
        {
            ChildControls.Add(item);
        }

        public void Clear()
        {
            ChildControls.Clear();
        }

        public bool Contains(UIControl item)
        {
            return ChildControls.Contains(item);
        }

        public void CopyTo(UIControl[] array, int arrayIndex)
        {
            ChildControls.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return ChildControls.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(UIControl item)
        {
            return ChildControls.Remove(item);
        }

        public IEnumerator<UIControl> GetEnumerator()
        {
            return ChildControls.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ChildControls.GetEnumerator();
        }
    }
}
