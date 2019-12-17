using System.Collections;
using System.Collections.Generic;
public interface IEvent {
    
    void addArg(string name, object val);
    void setType(EventType type);
    EventType getType();
    object getArg(string name);
    Dictionary<string, object> getArgs();
    void destroy();
}