using UnityEngine;

public interface IJSONSerializer
{
    string Serialize<T>(T value);

    T Deserialize<T>(string value);
}
