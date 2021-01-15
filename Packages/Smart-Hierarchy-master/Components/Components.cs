﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

namespace AV.Hierarchy
{
    internal class Components
    {
        public readonly Component main;
        public readonly Texture2D icon;
        private readonly List<ComponentData> data;

        public ComponentData this[int index] => data[index];

        public Components(GameObject instance)
        {
            var components = instance.GetComponents<Component>();
            data = new List<ComponentData>(components.Length);

            foreach (var component in components)
            {
                // TODO: Show null component in hierarchy
                if (component)
                    data.Add(new ComponentData(component));
            }

            main = ChooseMainComponent(components);

            if (main)
                icon = ObjectContent(main, main.GetType()).image as Texture2D;
        }
        
        public static Component ChooseMainComponent(params Component[] components)
        {
            var length = components.Length;
            if (length == 0) 
                return null;

            var prefs = HierarchySettingsProvider.Preferences;
            
            var zero = components[0];
            
            if (length == 1)
            {
                switch (prefs.transformIcon)
                {
                    case TransformIcon.Always: 
                        return zero;
                    
                    case TransformIcon.OnUniqueOrigin:
                        if (zero is Transform transform)
                        {
                            if (transform.localPosition != Vector3.zero || 
                                transform.localRotation != Quaternion.identity)
                                return zero;
                        }
                        return zero is RectTransform ? zero : null;
                        
                    case TransformIcon.OnlyRectTransform:
                        return zero is RectTransform ? zero : null;
                }

                return null;
            }
            
            var first = components[1];
            var last = components[length - 1];

            var prioritized = prefs.componentsPriority.SelectPrioritizedComponents(components).ToArray();

            if (prioritized.Any())
            {
                first = prioritized.First();
                last = prioritized.Last();
            }
            
            return prefs.preferLastComponent ? last : first;
        }
    }
}