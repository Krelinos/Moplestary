using Godot;
using System;

public class ItemDatabaseManager
{
    Godot.Collections.Dictionary ItemDatabase;

    public ItemDatabaseManager()
    {
        ItemDatabase = new Godot.Collections.Dictionary();

        AddItem( new Godot.Collections.Dictionary{
            {"name", "bruh"}
            ,{"boowomp", "uwu"}
            ,{"attackSpeed", 2.5f}
        } );

        foreach( var (key, value) in ItemDatabase )
            GD.Print("K: " + key + " | V: " + value);
    }

    bool AddItem( Godot.Collections.Dictionary itemDictionary )
    {
        int itemHash = GD.Hash( itemDictionary );

        if ( !ItemDatabase.ContainsKey(itemHash) )
        {
            ItemDatabase[itemHash] = itemDictionary;
            return true;
        }
        else
            return false;
    }
}

// class Item
// {
//     String name;
//     String sprite;
//     String flavor;
//     int rarity; // 0 Common | 1 Uncommon | 2 Rare | 3 Epic | 4 Legendary | 5 Unique
//     int maxStack;
// }

// class Equipment : Item
// {
//     int type;   // 0 Head | 1 Torso | 2 Legs | 3 Warrior | 4 Pirate | etc.

//     int minDamage;
//     int maxDamage;
//     float attackSpeed;

//     // Dictionary<int, int> statModifiers;
//     // Dictionary<int, int> skillModifiers;
// }

// class Potion : Item
// {
//     int HPRestored;
//     int MPRestored;

//     int HPHalfLife;
//     int MPHalfLife;

//     float consumeTime;

//     // List sideEffects;
// }