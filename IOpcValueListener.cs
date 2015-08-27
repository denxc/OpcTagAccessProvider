namespace OpcTagAccessProvider
{
    public interface IOpcValueListener
    {
        void OnValueChanged(IOpcValue aOpcValue, OpcValueChangedEventArgs aValueChangedEventArgs);
    }
}
