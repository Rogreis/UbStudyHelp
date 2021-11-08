using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace UbStudyHelp.Classes
{
    public static class ObservableCollectionExtension
    {
        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortableList = new List<T>(collection);
            sortableList.Sort(comparison);

            collection.Clear();

            foreach (T item in sortableList)
            {
                collection.Add(item);
            }
        }
    }
}
