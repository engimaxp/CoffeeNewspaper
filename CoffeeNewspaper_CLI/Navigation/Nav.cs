using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoffeeNewspaper_CLI.Navigation
{
    public class Nav
    {
        private static Nav _navigation;

        private static readonly object lockobject = new object();
        
        public static Nav GetNavigation()
        {
            if (_navigation == null)
            {
                lock (lockobject)
                {
                    if (_navigation == null)
                    {

                        lock (lockobject)
                        {
                            _navigation = new Nav();
                        }
                    }
                }
            }
            return _navigation;
        }

        public Stack<BaseView> viewList;

        private Nav()
        {
            viewList = new Stack<BaseView>();
        }

        public void Goto(BaseView nextView)
        {
            nextView.Display();
        }

        public void Back()
        {
            viewList.Pop();
            viewList.Peek().Display();
        }
    }
}
