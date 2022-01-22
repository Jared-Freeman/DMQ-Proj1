using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    // author: Jared Freeman


    /// <summary>
    /// Implements a list of types, enforcing that items added MUST be types of subclasses of param T.     
    /// &#xA; 
    /// 
    ///  *** This code was written referencing posts from stackoverflow: 
    /// (https://stackoverflow.com/questions/3260665/how-to-make-lists-add-method-protected-while-exposing-list-with-get-property)
    /// (https://stackoverflow.com/questions/42958839/c-sharp-list-of-subclass-types)
    /// </summary>
    /// <typeparam name="T"> Parent Type</typeparam>
    public class TypeList<T>
    {
        private List<System.Type> _ProtectedList = new List<System.Type>();

        //public-exposed. Elements are readonly!
        public IReadOnlyCollection<System.Type> List
        {
            get
            {
                return _ProtectedList.AsReadOnly();
            }
        }



        //
        public void Add<subclass>() where subclass : T
        {
            _ProtectedList.Add(typeof(subclass));
        }
        public void Add<t>(t item) where t : T
        {
            _ProtectedList.Add(typeof(t));
        }



        //
        public void Remove<subclass>() where subclass : T
        {
            _ProtectedList.Remove(typeof(subclass));
        }
        public void Remove<t>(t item) where t : T
        {
            _ProtectedList.Remove(typeof(t));
        }



        //
        public void Clear()
        {
            _ProtectedList.Clear();
        }
        //



        public bool IsEmpty()
        {
            if (_ProtectedList.Count < 1) return true;
            return false;
        }



        //
        public bool Contains<subclass>() where subclass : T
        {
            System.Type type = typeof(subclass);
            if (_ProtectedList.Contains(type)) return true;
            return false;
        }
        public bool Contains<t>(t item) where t : T
        {
            if (_ProtectedList.Contains(typeof(t))) return true;
            return false;
        }
    }
}