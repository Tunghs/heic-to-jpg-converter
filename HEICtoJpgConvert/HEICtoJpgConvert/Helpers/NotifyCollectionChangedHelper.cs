using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Reflection;

namespace HEICtoJpgConvert.Helpers
{
    public static class NotifyCollectionChangedHelper
    {
        private static Dictionary<INotifyCollectionChanged, Delegate> CollectionDictionary { get; set; }

        static NotifyCollectionChangedHelper()
        {
            CollectionDictionary = new Dictionary<INotifyCollectionChanged, Delegate>();
        }

        public static void PasueNotifyCollectionChanged(this INotifyCollectionChanged Target)
        {
            FieldInfo CollectionChangedField = Target.GetCollectionChangedFiled();
            Delegate Result = CollectionChangedField.GetValue(Target) as Delegate;
            CollectionDictionary[Target] = Result;
            CollectionChangedField.SetValue(Target, null);
        }

        public static void ResumeNotifyCollectionChaged(this INotifyCollectionChanged Target)
        {
            if (CollectionDictionary.ContainsKey(Target) == false) return;
            Target.GetCollectionChangedFiled().SetValue(Target, CollectionDictionary[Target]);
            CollectionDictionary.Remove(Target);
        }

        public static void Refresh(this INotifyCollectionChanged Target)
        {
            MethodInfo CollectionResetMethod = Target.GetCollectionResetMethod();

            CollectionResetMethod.Invoke(Target, null);
        }

        public static FieldInfo GetCollectionChangedFiled(this INotifyCollectionChanged Target)
        {
            FieldInfo Field = Target.GetType().GetField("CollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            if (Field == null)
            {
                Field = Target.GetType().BaseType.GetField("CollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            return Field;
        }

        public static MethodInfo GetCollectionResetMethod(this INotifyCollectionChanged Target)
        {
            MethodInfo Method = Target.GetType().GetMethod("OnCollectionReset", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (Method == null)
            {
                Method = Target.GetType().BaseType.GetMethod("OnCollectionReset", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            return Method;
        }
    }
}
