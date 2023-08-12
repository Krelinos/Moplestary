using Godot;
using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace Moplestary
{
    public class ItemDatabaseManager
    {
        public HashSet< Item > ItemDatabase;

        public ItemDatabaseManager()
        {
            ItemDatabase = new HashSet< Item >();

            DeserializeBaseItems();

            // ItemDatabase.Add( new Dictionary<String, Object>
            // {
            //     {"name", "bruh"}
            //     ,{"boowomp", "uwu"}
            //     ,{"attackSpeed", 2.5f}
            // } );
            // ItemDatabase.Add( new Dictionary<String, Object>
            // {
            //     {"firstname", "trans"}
            //     ,{"slime", ":3"}
            //     ,{"cooldown", 5f}
            // } );

            foreach( var value in ItemDatabase )
            {
                GD.Print( value );
            
                var j = JsonSerializer.Serialize( value );
                GD.Print( "nyaaa" + j );
#nullable enable
                Item? k = JsonSerializer.Deserialize<Item>( j );
                GD.Print( $"name: {k?.Name}" );
            }
        }

        // https://docs.godotengine.org/en/stable/tutorials/io/saving_games.html
        private void DeserializeBaseItems()
        {
            String filePath = Godot.ProjectSettings.GlobalizePath("res://Defs/Stuff.json");
            GD.Print( filePath );

            String fileText = File.ReadAllText( filePath );
            GD.Print( fileText );

            #nullable enable
            JsonHelper? items = JsonSerializer.Deserialize< JsonHelper >( fileText );

            if( items == null )
            {
                GD.Print("oof");
                return;
            }

            GD.Print( items );

            foreach( Item? item in items.ItemDefs )
            {
                GD.Print( item );
                ItemDatabase.Add( item );
            }

            foreach( Item item in ItemDatabase )
                GD.Print( item.Name );

        }

        class JsonHelper
        {
            public List< Item > ItemDefs { get; set; }
        }
    }

    #nullable enable
    public class Item
    {
        public String Name { get; set; } = "Unnamed Item";
        public String? SpritePath { get; set; } = "Debug/SpriteNotFound";
        public String? FlavorText { get; set; }
        public int? Rarity { get; set; }    // 0 Common | 1 Uncommon | 2 Rare | 3 Epic | 4 Legendary | 5 Unique
        public int? MaxStack { get; set; }

        // For Equipment-type items
        public int? Type { get; set; }     // 0 Head | 1 Torso | 2 Legs | 3 Warrior weapon | 4 Pirate weapon | etc.

        public int? MinDamage { get; set; }
        public int? MaxDamage { get; set; }
        public float? AttackSpeed { get; set; }

        #nullable enable
        public Dictionary<int, int>? StatModifiers { get; set; }
        #nullable enable
        public Dictionary<int, int>? SkillModifiers { get; set; }

        // For Consumable-type items
        public int? HPRestored { get; set; }
        public int? MPRestored { get; set; }

        public int? HPHalfLife { get; set; }
        public int? MPHalfLife { get; set; }

        public float? ConsumeTime { get; set; }

        // List? sideEffects { get; set; }
    }
}