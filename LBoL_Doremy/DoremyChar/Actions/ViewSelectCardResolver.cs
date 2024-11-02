using LBoL.Presentation.UI.Panels;
using LBoL.Presentation.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LBoL_Doremy.DoremyChar.Actions
{
    public class ViewSelectCardResolver
    {
        public Action action;

        public ViewSelectCardResolver(Action action)
        {
            this.action = action;
        }

        public IEnumerator ExtendEnumerator(IEnumerator enumerator)
        {
            enumerator.MoveNext();

            var showAsyncEnum = enumerator.Current as IEnumerator;
            showAsyncEnum.MoveNext();

            action();
            yield return showAsyncEnum.Current;

            yield return enumerator;
        }

    }
}
