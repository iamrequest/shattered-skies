using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class OnHealthChangedEvent: UnityEvent<float, BaseDamager, Damageable> 
{ }

[Serializable]
public class OnDeathEvent: UnityEvent<BaseDamager, Damageable> 
{ }
