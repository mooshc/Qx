using NHibernate;
using NHibernate.Type;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Qx.BL
{
    public class TypedStateInterceptor : EmptyInterceptor
    {
        private Dictionary<object, TypedStateInterceptor.ObjectModificationType> _modifications = new Dictionary<object, TypedStateInterceptor.ObjectModificationType>();

        public IDictionary<System.Type, Action<object>> TypeDeleteCallback { get; private set; }

        public IDictionary<System.Type, Action<object>> TypeCreateCallback { get; private set; }

        public IDictionary<System.Type, Action<object>> TypeUpdateCallback { get; private set; }

        public TypedStateInterceptor()
        {
            this.TypeDeleteCallback = (IDictionary<System.Type, Action<object>>)new Dictionary<System.Type, Action<object>>();
            this.TypeCreateCallback = (IDictionary<System.Type, Action<object>>)new Dictionary<System.Type, Action<object>>();
            this.TypeUpdateCallback = (IDictionary<System.Type, Action<object>>)new Dictionary<System.Type, Action<object>>();
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (!this._modifications.ContainsKey(entity))
                this._modifications.Add(entity, TypedStateInterceptor.ObjectModificationType.Delete);
            else
                this._modifications[entity] = TypedStateInterceptor.ObjectModificationType.Delete;
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            if (!this._modifications.ContainsKey(entity))
                this._modifications.Add(entity, TypedStateInterceptor.ObjectModificationType.Update);
            return false;
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (!this._modifications.ContainsKey(entity))
                this._modifications.Add(entity, TypedStateInterceptor.ObjectModificationType.Created);
            return false;
        }

        public override void PostFlush(ICollection entities)
        {
            foreach (KeyValuePair<object, TypedStateInterceptor.ObjectModificationType> modification in this._modifications)
            {
                System.Type type = modification.Key.GetType();
                Action<object> action = (Action<object>)null;
                switch (modification.Value)
                {
                    case TypedStateInterceptor.ObjectModificationType.Delete:
                        this.TypeDeleteCallback.TryGetValue(type, out action);
                        break;
                    case TypedStateInterceptor.ObjectModificationType.Update:
                        this.TypeUpdateCallback.TryGetValue(type, out action);
                        break;
                    case TypedStateInterceptor.ObjectModificationType.Created:
                        this.TypeCreateCallback.TryGetValue(type, out action);
                        break;
                }
                if (action != null)
                    action(modification.Key);
            }
            this._modifications.Clear();
        }

        private enum ObjectModificationType
        {
            Delete,
            Update,
            Created,
        }
    }
}