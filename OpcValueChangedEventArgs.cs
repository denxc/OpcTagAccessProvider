namespace OpcTagAccessProvider
{
    using System;

    /// <summary>
    /// Аргумент при изменении значения тега.
    /// </summary>
    public class OpcValueChangedEventArgs
    {
        /// <summary>
        /// Значение.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Качество.
        /// </summary>
        public int Quality { get; private set; }

        /// <summary>
        /// Время чтения.
        /// </summary>
        public DateTime ReadTime { get; private set; }        

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="aValue">Значение.</param>
        /// <param name="aQuality">Качество.</param>
        /// <param name="aReadTime">Время.</param>
        public OpcValueChangedEventArgs(object aValue, int aQuality, DateTime aReadTime)
        {
            Value = aValue;
            Quality = aQuality;
            ReadTime = aReadTime;
        }
    }
}
