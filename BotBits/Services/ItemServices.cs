﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace BotBits
{
    public static class ItemServices
    {
        private static readonly ConcurrentDictionary<Smiley, PackAttribute> _smileyPacks = new ConcurrentDictionary<Smiley, PackAttribute>();
        private static readonly ConcurrentDictionary<AuraColor, PackAttribute> _auraColorPacks = new ConcurrentDictionary<AuraColor, PackAttribute>();
        private static readonly ConcurrentDictionary<AuraShape, PackAttribute> _auraShapePacks = new ConcurrentDictionary<AuraShape, PackAttribute>();
        private static readonly ConcurrentDictionary<int, PackAttribute> _blockPacks = new ConcurrentDictionary<int, PackAttribute>();
        private static readonly Dictionary<int, Type> _blockGroups = new Dictionary<int, Type>();

        static ItemServices()
        {
            LoadPacks(typeof(Background));
            LoadPacks(typeof(Foreground));
            LoadEnum(_smileyPacks);
            LoadEnum(_auraColorPacks);
            LoadEnum(_auraShapePacks);
        }

        public static Type GetGroup(Foreground.Id id)
        {
            if (id == 0) return typeof(Foreground);
            return GetGroup((int)id);
        }

        public static Type GetGroup(Background.Id id)
        {
            if (id == 0) return typeof(Background);
            return GetGroup((int)id);
        }

        public static KeyValuePair<int, Type>[] GetGroups()
        {
            return _blockGroups.ToArray();
        }

        public static PackAttribute GetPackage(AuraShape id)
        {
            PackAttribute pack;
            _auraShapePacks.TryGetValue(id, out pack);
            return pack;
        }

        public static PackAttribute GetPackage(AuraColor id)
        {
            PackAttribute pack;
            _auraColorPacks.TryGetValue(id, out pack);
            return pack;
        }

        public static PackAttribute GetPackage(Smiley id)
        {
            PackAttribute pack;
            _smileyPacks.TryGetValue(id, out pack);
            return pack;
        }

        public static PackAttribute GetPackage(Foreground.Id id)
        {
            return GetPackageInternal((int)id);
        }

        public static PackAttribute GetPackage(Background.Id id)
        {
            return GetPackageInternal((int)id);
        }

        public static void SetPackage(AuraShape id, PackAttribute package)
        {
            _auraShapePacks[id] = package;
        }

        public static void SetPackage(AuraColor id, PackAttribute package)
        {
            _auraColorPacks[id] = package;
        }

        public static void SetPackage(Smiley id, PackAttribute package)
        {
            _smileyPacks [id] = package;
        }

        public static void SetPackage(Foreground.Id id, PackAttribute package)
        {
            SetPackageInternal((int)id, package);
        }

        public static void  SetPackage(Background.Id id, PackAttribute package)
        {
            SetPackageInternal((int)id, package);
        }

        internal static void SetPackageInternal(int id, PackAttribute package)
        {
            _blockPacks[id] = package;
        }

        internal static PackAttribute GetPackageInternal(int id)
        {
            PackAttribute pack;
            _blockPacks.TryGetValue(id, out pack);
            return pack;
        }

        private static void LoadPacks(Type type)
        {
            foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var value = (ushort)field.GetValue(null);
                try
                {
                    if (value != 0)
                        _blockGroups.Add(value, type);

                    var pack = GetPack(field);
                    if (pack != null)
                    {
                        var result = _blockPacks.TryAdd(value, pack);
                        Debug.Assert(result); // _blockGroups.Add must fail if the value is duplicate
                    }
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException("Duplicate block: " + value, ex);
                }
            }

            foreach (var i in type.GetNestedTypes())
            {
                LoadPacks(i);
            }
        }


        private static void LoadEnum<T>(ConcurrentDictionary<T, PackAttribute> collection)
        {
            foreach (var field in typeof(T).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                var pack = GetPack(field);
                if (pack != null)
                {
                    var result = collection.TryAdd((T)field.GetValue(null), pack); 
                    Debug.Assert(result); // Enums employ a compile time uniqueness check
                }
            }
        }

        private static PackAttribute GetPack(ICustomAttributeProvider provider)
        {
            return (PackAttribute)provider
                .GetCustomAttributes(typeof(PackAttribute), false)
                .FirstOrDefault();
        }

        private static Type GetGroup(int id)
        {
            Type type;
            _blockGroups.TryGetValue(id, out type);
            return type;
        }
    }
}