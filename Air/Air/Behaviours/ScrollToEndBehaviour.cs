using System.Collections.Specialized;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;

namespace Air.Behaviours;

public class ScrollToEndBehaviour : Behavior<ListBox>
{
    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject != null)
        {
            AssociatedObject.Items.CollectionChanged += ScrollToBottom_OnCollectionChanged;
        }
    }

    private void ScrollToBottom_OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        AssociatedObject?.ScrollIntoView(AssociatedObject.Items.Count - 1);
    }
}