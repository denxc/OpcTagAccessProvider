namespace OpcTagAccessProvider
{
    using System;
    using System.Collections.Generic;
    using OPCAutomation;

    public class OpcValueImpl : IOpcValue
    {
        public string Name { get; set; }
        public bool IsListenValueChanging { get; set; }
        public bool IsActive { get; private set; }
        public string GroupName { get; private set; }
        public OPCDataSource Source { get; set; }

        private OPCServer server;
        private OPCGroup opcGroup;
        private OPCItem opcItem;

        private List<IOpcValueListener> listeners = new List<IOpcValueListener>();

        public OpcValueImpl(OPCServer aServer, string aName = "", OPCDataSource aSource = OPCDataSource.OPCCache)
        {
            if (aServer == null) {
                throw new ArgumentNullException("aServer");
            }

            Source = aSource;
            server = aServer;
            Name = aName;
        }

        public void Activate()
        {
            if (IsActive) {
                throw new ArgumentException("Двойная активация OPC тега: " + Name);
            }

            if (string.IsNullOrEmpty(Name)) {
                throw new ArgumentException("Имя OPC параметра не задано.");
            }

            if (server.ServerState != (int) OPCServerState.OPCRunning) {
                throw new ArgumentException(
                    "Ошибка при активации тега: OPC сервер не подключен.");
            }
            
            GroupName = Guid.NewGuid().ToString();
            opcGroup = server.OPCGroups.Add(GroupName);
            opcItem = opcGroup.OPCItems.AddItem(Name, opcGroup.ClientHandle);

            if (IsListenValueChanging) {                
                opcGroup.DataChange += ValueChanged;
                opcGroup.UpdateRate = 100;
                opcGroup.IsActive = true;
                opcGroup.IsSubscribed = true;
            }

            IsActive = true;
        }        

        public void Deactivate()
        {
            if (!IsActive) {
                throw new ArgumentException(
                    "OPC тег не активирован, но поступил запрос на деактивацию: " + Name);
            }

            if (server.ServerState == (int) OPCServerState.OPCRunning) {                
                server.OPCGroups.Remove(GroupName);
            }
        }

        public object ReadCurrentValue()
        {
            if (!IsActive) {
                throw new ArgumentException(
                    "OPC параметр не активирован, но предпринята попытка чтения: " + Name);
            }
            
            object value;
            object quality;
            object timeStamp;
            opcItem.Read((short) Source, out value, out quality, out timeStamp);
            return value;
        }

        public void WriteValue(object aValue)
        {
            if (aValue == null) {
                throw new ArgumentNullException("aValue");
            }

            if (!IsActive) {
                throw new ArgumentException(
                    "Параметр OPC не активирован, но предпринята попытка его записи: " + Name);
            }
            
            opcItem.Write(aValue);
        }

        public void SubscribeToValueChange(IOpcValueListener aListener)
        {
            if (aListener == null) {
                throw new ArgumentNullException("aListener");
            }

            if (!IsListenValueChanging) {
                throw new ArgumentException(
                    "Измените состояние IsListenValueChanging для подписки на изменение значения OPC тега.");
            }            

            listeners.Add(aListener);
        }

        public void UnSubscribeToValueChange(IOpcValueListener aListener)
        {
            if (aListener == null) {
                throw new ArgumentNullException("aListener");
            }

            if (listeners.Contains(aListener)) {
                listeners.Remove(aListener);
            }
        }

        private void ValueChanged(
            int aTransactionid, 
            int aNumitems, 
            ref Array aClienthandles, 
            ref Array aItemvalues, 
            ref Array aQualities, 
            ref Array aTimestamps)
        {
            var currentValue = aItemvalues.GetValue(1);
            AlertListeners(currentValue);
        }

        private void AlertListeners(object aCurrentValue)
        {
            foreach (var listener in listeners) {
                listener.OnValueChanged(this, aCurrentValue);
            }
        }
    }
}
