using System;
using System.Collections.Generic;
using UnityEngine;

public class VariableStore
{
    public class Database
    {
        public Dictionary<string, Variable> variables = new Dictionary<string, Variable>(); 
    }

    public abstract class Variable
    {
        public abstract void Get();
        public abstract void Set(object value);

    }

    public class Variable<T> : Variable
    {
        private T value;

        private Func<T> getter;
        private Action<T> setter;

        public Variable(T defaultValue = default, Func<T> getter = null, Action<T> setter = null)
        {
            value = defaultValue;

            if (getter == null)
                this.getter = () => value;
            else
                this.getter = getter;

            if (setter == null)
                this.setter = newValue => value = newValue;
            else
                this.setter = setter;


        }

        public override void Get() => getter();

        public override void Set(object newValue) => setter((T)newValue);
    }

    public static
}

 