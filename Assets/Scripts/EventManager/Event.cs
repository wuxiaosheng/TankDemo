using System.Collections;
using System.Collections.Generic;
public class Event : IEvent {
    private EventType _type;
    private Dictionary<string, object> _args;
    public Event() {
        _args = new Dictionary<string, object>();
    }
    public void addArg(string name, object val) {
        if (_args.ContainsKey(name)) {
            _args[name] = val;
        } else {
            _args.Add(name, val);
        }
    }
    public void setType(EventType type) {
        _type = type;
    }
    public EventType getType() {
        return _type;
    }

    public object getArg(string name) {
        if (_args.ContainsKey(name)) {
            return _args[name];
        }
        return null;
    }

    public Dictionary<string, object> getArgs() {
        return _args;
    }

    public void destroy() {
        _args.Clear();
        _args = null;
    }

}