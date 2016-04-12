using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitJiraConfiguration
{
    class ConcreteType : Attribute
    {
        public Type Value { get; private set; }

        public ConcreteType(Type concreteType)
        {
            Value = concreteType;
        }
    }
}
