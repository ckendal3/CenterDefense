using Unity.Entities;

public class NetworkSystemGroup : ComponentSystemGroup { }
// Gather all network messages and apply them to the right spot
// Such as Input Gathering from each player

[UpdateAfter(typeof(NetworkSystemGroup))]
public class InputSystemGroup : ComponentSystemGroup { }
// apply all input received to the needed components

[UpdateAfter(typeof(InputSystemGroup))]
public class MovementSystemGroup : ComponentSystemGroup { }
// Update rotations system then update movement system

[UpdateAfter(typeof(MovementSystemGroup))]
public class HitSystemGroup : ComponentSystemGroup { }
// Raycasts, projectiles, etc and then do logic for hits such as reduce life times to 0

[UpdateAfter(typeof(HitSystemGroup))]
public class LifeSystemGroup : ComponentSystemGroup { }
// lifetime reduction system and then destroy system
