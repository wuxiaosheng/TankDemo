using System.Collections;
using System.Collections.Generic;
public interface IEventManager {
    bool addEventListener(EventType gEvt, Handler handler);
    bool removeEventListener(EventType gEvt);
    bool removeEventListener(EventType gEvt, Handler handler);
    void trigger(IEvent evt);
    void broadcast(IEvent evt);
    void destroy();
}