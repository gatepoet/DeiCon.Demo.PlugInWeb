using System.Collections.Generic;
using System.Linq;
using System;

namespace org.theGecko.Utilities.MefMvc
{
    /// <summary>
    /// A menu contains a set of <see cref="Item"/>s but is not a menu item itself.  
    /// It is not rendered in the UI but the items that it contains are.  
    /// For example, a menu can contain a set of top-level menu items that make up the main menu of a site.
    /// </summary>
    public class Menu
    {
        private const string DefaultAction = "Index";

        /// <summary>
        /// Gets the set of <see cref="Item"/>s in the menu.
        /// </summary>
        public ISet<Item> Items 
        {
            get; private set;
        }

        /// <summary>
        /// Creates a new <see cref="Menu"/> instance.
        /// </summary>
        public Menu()
        {
            Items = new SortedSet<Item>(new MenuItemComparer());
        }

        /// <summary>
        /// Adds a menu item without a link for containing other items
        /// </summary>
        /// <param name="title">Title of item</param>
        /// <param name="order">Order of item</param>
        /// <returns>Item which was added</returns>
        public Item AddParent(string title, int order)
        {
            return Add(title, order, null, null);
        }

        /// <summary>
        /// Adds a menu item with a link
        /// </summary>
        /// <param name="title">Title of item</param>
        /// <param name="order">Order of item</param>
        /// <returns>Item which was added</returns>
        public Item AddLink(string title, int order)
        {
            return AddLink(title, order, title);
        }

        /// <summary>
        /// Adds a menu item with a link
        /// </summary>
        /// <param name="title">Title of item</param>
        /// <param name="order">Order of item</param>
        /// <param name="controller"></param>
        /// <returns>Item which was added</returns>
        public Item AddLink(string title, int order, string controller)
        {
            return AddLink(title, order, controller, DefaultAction);
        }

        /// <summary>
        /// Adds a menu item with a link
        /// </summary>
        /// <param name="title">Title of item</param>
        /// <param name="order">Order of item</param>
        /// <param name="controller">Controller to use for link</param>
        /// <param name="action">Action to use for link</param>
        /// <returns>Item which was added</returns>
        public Item AddLink(string title, int order, string controller, string action)
        {
            return Add(title, order, controller, action);
        }

        /// <summary>
        /// Returns a <see cref="Item"/> with the specified key from the menu, creating it if it doesn't already exist.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="order"></param>
        /// <param name="controller"></param>
        /// <param name="action"></param>
        /// <returns>A <see cref="Item"/> with the specified key.</returns>
        protected Item Add(string title, int order, string controller, string action)
        {
            Item item = GetItem(title);

            if (item == null)
            {
                item = new Item(title, order, controller, action);
                Items.Add(item);
            }

            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        private Item GetItem(string title)
        {
            return Items.FirstOrDefault(item => item.Title == title);
        }

        /// <summary>
        /// An item on a menu.  A MenuItem is also a <see cref="Menu"/> to allow it to have a set of child menu items.
        /// </summary>
        public class Item : Menu
        {
            internal Item(string title, int order, string controller, string action)
            {
                Title = title;
                Order = order;
                Controller = controller;
                Action = action;
            }

            /// <summary>
            /// SiteMap node title
            /// </summary>
            public string Title { get; private set; }

            /// <summary>
            /// The order index of the SiteMap node
            /// </summary>
            public int Order { get; internal set; }

            /// <summary>
            /// SiteMap node controller
            /// </summary>
            public string Controller { get; private set; }

            /// <summary>
            /// SiteMap node action
            /// </summary>
            public string Action { get; private set; }

            public override string ToString()
            {
                return string.Format("{0} ({1})", Title, Order);
            }
        }

        private class MenuItemComparer : IComparer<Item>
        {
            public int Compare(Item x, Item y)
            {              
                int result = x.Order.CompareTo(y.Order);

                if (result == 0)
                {
                    throw new Exception(string.Format("Cannot register menu item with Title '{0}', Order {1} because a menu item with the same Order is already registered at the same menu level.", x.Title, x.Order));
                }

                if (x.Title == y.Title)
                {
                    throw new Exception(string.Format("Cannot register menu item with Title '{0}' because a menu item with the same Title is already registered at the same menu level.", x.Title));
                } 

                return result;
            }
        }
    }
}