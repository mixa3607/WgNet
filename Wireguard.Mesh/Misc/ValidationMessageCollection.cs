using System;
using System.Collections;
using System.Collections.Generic;

namespace ArkProjects.Wireguard.Mesh.Misc;

public class ValidationMessageCollection : IReadOnlyList<ConfigValidationMessage>
{
    private readonly List<ConfigValidationMessage> _messages = new();

    public IEnumerator<ConfigValidationMessage> GetEnumerator() => _messages.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _messages.Count;

    public ConfigValidationMessage this[int index]
    {
        get => _messages[index];
        set => throw new NotSupportedException();
    }

    public ValidationMessageCollection Add(string path, string message, ConfigValidationMessageLevel level)
    {
        _messages.Add(new ConfigValidationMessage(path, message, level));
        return this;
    }
}