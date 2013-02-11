using System;

namespace org.theGecko.Utilities.MefMvc
{
    public interface IControllerMetaData
    {
        string ControllerName { get; }
        Type ControllerType { get; }
    }
}
