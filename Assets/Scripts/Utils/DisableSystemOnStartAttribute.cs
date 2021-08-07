using System;

namespace Game.Scripts
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class DisableSystemOnStartAttribute : Attribute
    {
        public DisableSystemOnStartAttribute()
        {
        }
    }
}
