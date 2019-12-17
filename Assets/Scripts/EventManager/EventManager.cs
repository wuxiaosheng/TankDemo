using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : IEventManager
{
    private static EventManager _instance;
    private Dictionary<EventType, IEventHandler> _dict;
    private Queue<IEvent> _queue;

    public static EventManager getInstance() {
        if (_instance == null) {
            _instance = new EventManager();
        }
        return _instance;
    }
    public EventManager() {
        _dict = new Dictionary<EventType, IEventHandler>();
        _queue = new Queue<IEvent>();
    }

    public bool addEventListener(EventType gEvt, Handler handler) {
        bool bRet;
        if (!_dict.ContainsKey(gEvt)) {
            _dict.Add(gEvt, new EventHandler());
        }
        bRet = _dict[gEvt].addHandler(handler);
        return bRet;
    }
    public bool removeEventListener(EventType gEvt) {
        if (_dict.ContainsKey(gEvt)) {
            _dict.Remove(gEvt);
            return true;
        }
        return false;
    }
    public bool removeEventListener(EventType gEvt, Handler handler) {
        bool bRet = true;
        if (_dict.ContainsKey(gEvt)) {
            bRet = _dict[gEvt].removeHandler(handler);
        }
        return bRet;
    }
    public Event createEvent(EventType type, string argKey, object argVal) {
        Event evt = new Event();
        evt.setType(type);
        evt.addArg(argKey, argVal);
        return evt;
    }
    public Event createEvent(EventType type) {
        Event evt = new Event();
        evt.setType(type);
        return evt;
    }
    public void trigger(EventType type, string argKey, object argVal) {
        Event evt = createEvent(type, argKey, argVal);
        trigger(evt);
    }
    public void trigger(EventType type) {
        Event evt = createEvent(type);
        trigger(evt);
    }
    public void trigger(IEvent evt) {
        _queue.Enqueue(evt);
    }
    public void broadcast(EventType type, string argKey, object argVal) {
        Event evt = createEvent(type, argKey, argVal);
        broadcast(evt);
    }
    public void broadcast(EventType type) {
        Event evt = createEvent(type);
        broadcast(evt);
    }
    public void broadcast(IEvent evt) {
        if (evt == null) { return; }
        EventType type = evt.getType();
        if (_dict.ContainsKey(type)) {
            _dict[type].broadcast(evt);
        }
    }
    public void update() {
        if (_queue.Count > 0) {
            IEvent evt = _queue.Dequeue();
            broadcast(evt);
        }
    }
    public void destroy() {
        _dict.Clear();
        _queue.Clear();
        _dict = null;
        _queue = null;
    }
}
