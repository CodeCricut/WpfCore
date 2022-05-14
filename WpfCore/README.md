# WpfCore

WpfCore is a lightweight, opinionated package with commonly used WPF ICommand wrappers, view management solutions, and an event aggregator for global messaging.

## Quick Start

### `DelegateCommand` and `AsyncDelegateCommand`

These two classes provide a sensible implementation of WPF's built-in `ICommand` interface used for event handlers. They should be used in the view model or backing logic for the view you want to register handlers for.

Example:

```xml
<Button Command="{Binding ViewModel.LoadAsyncCommand}">
    Load async
</Button>
<Button Command="{Binding ViewModel.CloseCommand}">
    Close
</Button>
```

```cs
public class DelegateCommandExample {
    public AsyncDelegateCommand LoadAsyncCommand { get; }
    public DelegateCommand CloseCommand { get; }

    public DelegateCommandExample(){
        LoadAsyncCommand = new AsyncDelegateCommand(LoadAsync);
        CloseCommand = new DelegateCommand(Close);
    }


    // Commands accept an object argument, which will default to null if not provided
    public async Task LoadAsync(object param = null){
        // do some long running async task...
    }

    public void Close(object param = null) {
        // close window...
    }
}
```

## Software Architecture

### Model-View-ViewModel (MVVM)

This library is designed to be the core of a WPF app using [MVVM architecture](https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern). The MVVM pattern helps create a WPF application in which concerns like presentation logic and buisness logic are separate.

More details can be found in `mvvm.md`.

### Window Manager

A [window manager](http://nichesoftware.co.nz/2015/08/23/wpf-window-manager.html) is available to handle the details of window creation and destruction, and to decouple views
from view models.

More details can be found in `window-manager.md`.

### Message Bus (Event Aggregator)

The [Event Aggregator pattern](http://www.nichesoftware.co.nz/2015/08/16/wpf-event-aggregates.html) is a pattern used to handle communication between disparate parts of the application without those parts having references to each other. Instead, a
central "message bus" or "event aggregator" is referenced by both and brokers messages.

The interface and default implementation of an event aggregator can be found within the `MessageBus` directory.

### Services

The `DependencyInjection` class can be used by projects depending on this one to register available services to a DI Container.
