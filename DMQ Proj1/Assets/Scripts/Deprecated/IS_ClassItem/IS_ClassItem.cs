using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem.Deprecated
{
    ///// <summary>
    ///// Container for an item with rules for how it is transferred between classes
    ///// </summary>
    //public class IS_ClassItem : IS_ItemBase
    //{

    //}

    /// <summary>
    /// Defines the default item, and the items to convert to per-class
    /// </summary>
    [CreateAssetMenu(fileName = "ClassItem_", menuName = "ScriptableObjects/Items/Class Item Preset", order = 1)]
    public class IS_ClassItemPreset : ScriptableObject
    {
        [System.Serializable]
        public struct ClassItemData
        {
            public ClassSystem.CharacterClass _Class;
            public IS_ItemBase _Item;
        }

        public IS_ItemBase DefaultItem;
        public List<ClassItemData> ClassItemVariations = new List<ClassItemData>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_class"></param>
        /// <returns>Item for class, or default if no entry exists for this class.</returns>
        public IS_ItemBase LookupItem(ClassSystem.CharacterClass _class)
        {
            foreach( var d in ClassItemVariations)
            {
                if(d._Class == _class)
                {
                    return d._Item;
                }
            }
            return DefaultItem;
        }
    }

}