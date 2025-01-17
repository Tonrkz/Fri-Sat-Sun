using UnityEngine;

public interface IActivatables {
    string AssignedWord { get; set; }
    void Activate();
}