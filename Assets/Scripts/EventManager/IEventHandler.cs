using System.Collections;
using System.Collections.Generic;
public delegate void Handler(IEvent evt);
public interface IEventHandler {
    bool addHandler(Handler handler);
    bool removeHandler(Handler handler);
    void broadcast(IEvent evt);
    void clear();
    void destroy();
}