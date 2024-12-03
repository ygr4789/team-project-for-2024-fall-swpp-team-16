using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable: MonoBehaviour
{
    public void InteractWith(Interactable other)
    {
        var interaction = InteractionRegistry.GetInteraction(this.GetType(), other.GetType());
        if (interaction != null)
        {
            interaction(this, other);
        }
        else
        {
            Debug.Log($"No interaction defined between {this.GetType().Name} and {other.GetType().Name}");
        }
    }
}

public static class InteractionRegistry
{
    private static readonly Dictionary<(Type, Type), Action<Interactable, Interactable>> Interactions = new();

    // Register Interaction
    public static void Register<T1, T2>(Action<Interactable, Interactable> interaction)
        where T1 : Interactable where T2 : Interactable
    {
        Interactions[(typeof(T1), typeof(T2))] = interaction;
    }

    // Search Interaction
    public static Action<Interactable, Interactable> GetInteraction(Type type1, Type type2)
    {
        Interactions.TryGetValue((type1, type2), out var interaction);
        return interaction;
    }
}