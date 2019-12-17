using System.Collections;
using System.Collections.Generic;
public class EventHandler : IEventHandler {
    private List<Handler> _handlers;
    public EventHandler() {
        _handlers = new List<Handler>();
    }
    public bool addHandler(Handler handler) {
        _handlers.Add(handler);
        return true;
    }

    public bool removeHandler(Handler handler) {
        if (_handlers.Contains(handler)) {
            _handlers.Remove(handler);
            return true;
        }
        return false;
    }

    public void broadcast(IEvent evt) {
        foreach (Handler handler in _handlers) {
            handler(evt);
        }
    }

    public void clear() {
        _handlers.Clear();
    }

    public void destroy() {
        if (_handlers != null) {
            this.clear();
            _handlers = null;
        }
    }
}