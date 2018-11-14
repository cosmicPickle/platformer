using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBotNavigator {

    bool IsTargetOnPath(Vector2 target);

    bool GetKnockbackStatus();

    void Patrol();

    void KeepAttackDistance();

    void Stop();

    void Pause();

    void Resume();
}
