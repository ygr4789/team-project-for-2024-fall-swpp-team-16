using UnityEngine;

public static class InteractionDefinitions
{
    [RuntimeInitializeOnLoadMethod]
    public static void DefineInteractions()
    {
        InteractionRegistry.Register<AnimalController, BreakableObject>(AnimalDamageBreakable);
        InteractionRegistry.Register<RockController, BreakableObject>(RockDamageBreakable);
        InteractionRegistry.Register<AnimalController, TreeController>(AnimalDamageTree);
        InteractionRegistry.Register<RockController, TreeController>(RockDamageTree);
    }

    private static void AnimalDamageBreakable(AnimalController animal, BreakableObject breakable)
    {
        breakable.Damage();
    }
    
    private static void RockDamageBreakable(RockController rock, BreakableObject breakable)
    {
        breakable.Damage();
    }
    
    private static void AnimalDamageTree(AnimalController animal, TreeController tree)
    {
        tree.Damage(animal.transform);
    }
    
    private static void RockDamageTree(RockController rock, TreeController tree)
    {
        tree.Damage(rock.transform);
    }
}