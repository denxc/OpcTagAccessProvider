using System;

namespace OpcTagAccessProvider
{
    /// <summary>
    /// Значение параметра на сервере OPC.
    /// </summary>
    public interface IOpcValue
    {
        /// <summary>
        /// Имя OPC тега. Задается до активации.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Параметр, указывающий, что необходимо подписываться 
        /// на изменения значений.
        /// Задается до активации.
        /// </summary>
        bool IsListenValueChanging { get; set; }

        /// <summary>
        /// Параметр, указывающий на текущее состояние тега.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Имя группы OPC тега. Устанавливается после активации.
        /// </summary>
        string GroupName { get; }

        /// <summary>
        /// Активирует ОРС паарметра.
        /// </summary>
        void Activate();

        /// <summary>
        /// Деактивирует OPC параметр.
        /// </summary>
        void Deactivate();

        /// <summary>
        /// Возвращает текущее значение ОРС параметра.
        /// </summary>
        /// <returns>Текущее значение.</returns>
        object ReadCurrentValue();

        /// <summary>
        /// Читает текущее значение.
        /// </summary>
        /// <param name="aValue">Значение.</param>
        /// <param name="aQuality">Качество.</param>
        /// <param name="aReadTime">Время чтения.</param>
        void ReadCurrentValue(out object aValue, out int aQuality, out DateTime aReadTime);

        /// <summary>
        /// Записывает значение в ОРС параметр.
        /// </summary>
        /// <param name="aValue">Значение.</param>
        void WriteValue(object aValue);

        /// <summary>
        /// Подписывает слушателя на изменение значения ОРС параметра.
        /// </summary>
        /// <param name="aListener">Слушатель.</param>
        void SubscribeToValueChange(IOpcValueListener aListener);

        /// <summary>
        /// Отписывает слушателя от оповещений при изменении ОРС параметра.
        /// </summary>
        /// <param name="aListener">Слушатель.</param>
        void UnSubscribeToValueChange(IOpcValueListener aListener);
    }
}
