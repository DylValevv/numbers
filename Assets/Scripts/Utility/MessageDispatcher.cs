using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;

public static class MessageDispatcher {
    private static Dictionary<string, Action<Message>> listeners = new Dictionary<string, Action<Message>>();

    public static void AddListener(string messageType, Action<Message> listener) {
        if (!listeners.ContainsKey(messageType)) {
            listeners.Add(messageType, null);
        }
        listeners[messageType] += listener;
    }

    public static void RemoveListener(string messageType, Action<Message> listener) {
        if (listeners.ContainsKey(messageType)) {
            listeners[messageType] -= listener;
        }
    }

    public static void SendMessage(Message message) {
        if (listeners.ContainsKey(message.MessageType)) {
            listeners[message.MessageType]?.Invoke(message);
        }
    }
}

public class Message {
    public string MessageType;
    public object Data;

    public Message(string type, object data) {
        MessageType = type;
        Data = data;
    }
}