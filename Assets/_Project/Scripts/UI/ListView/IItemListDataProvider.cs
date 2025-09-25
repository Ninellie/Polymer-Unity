using System;

namespace UI.ListView
{
    public interface IItemListDataProvider
    {
        int ItemsCount { get; }
        string GetTitle(int index);
        string GetSubtitle(int index);
        Action<int> OnItemSelected { get; }
    }
}