namespace OpcTagAccessProvider
{
    using System;
    using System.Collections.Generic;
    using OPCAutomation;

    public class OpcValueImpl : IOpcValue
    {
        private string name;
        private bool isListenValueChanging;
        private OPCDataSource source;
        private OPCServer server;
        private OPCGroup opcGroup;
        private OPCItem opcItem;
        private List<IOpcValueListener> listeners = null;

        public string Name {
            get { return name; }
            set {
                if (IsActive) {
                    throw new OpcValueException("Попытка изменения имени тега после активации.");
                }

                name = value;
            }
        }

        public bool IsListenValueChanging {
            get { return isListenValueChanging; }
            set {
                if (IsActive) {
                    throw new OpcValueException("Попытка изменения состояния возможности подписки " +
                                                "на изменение значения после активации тега.");
                }

                isListenValueChanging = value;
                if (isListenValueChanging && listeners == null) {
                    listeners = new List<IOpcValueListener>();
                }
            }
        }

        public OPCDataSource Source {
            get { return source; }
            set {
                if (IsActive) {
                    throw new OpcValueException("Попытка изменить значение Source после активации тега.");
                }

                source = value;
            }
        }

        public bool IsActive { get; private set; }
        public string GroupName { get; private set; }        

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
                throw new OpcValueException("Двойная активация OPC тега: " + Name);
            }

            if (string.IsNullOrEmpty(Name)) {
                throw new OpcValueException("Имя OPC параметра не задано.");
            }

            if (server.ServerState != (int) OPCServerState.OPCRunning) {
                throw new OpcValueException(
                    "Ошибка при активации тега: OPC сервер не подключен.");
            }
            
            GroupName = Guid.NewGuid().ToString();
            opcGroup = server.OPCGroups.Add(GroupName);
            opcItem = opcGroup.OPCItems.AddItem(Name, opcGroup.ClientHandle);

            if (isListenValueChanging) {
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
                throw new OpcValueException(
                    "OPC тег не активирован, но поступил запрос на деактивацию: " + Name);
            }

            if (server.ServerState == (int) OPCServerState.OPCRunning) {                
                server.OPCGroups.Remove(GroupName);
            }
        }

        public object ReadCurrentValue()
        {
            object value;
            int quality;
            DateTime timeStamp;
            ReadCurrentValue(out value, out quality, out timeStamp);

            return value;
        }

        public void ReadCurrentValue(out object aValue, out int aQuality, out DateTime aReadTime)
        {
            if (!IsActive) {
                throw new OpcValueException(
                    "OPC параметр не активирован, но предпринята попытка чтения: " + Name);
            }
            
            object value;
            object quality;
            object timeStamp;

            try {
                opcItem.Read((short) Source, out value, out quality, out timeStamp);
            }
            catch (Exception ex) {
                throw new OpcValueException(string.Format("Ошибка при чтении " +
                                                          "значения тега {0}: {1}", Name, ex.Message));
            }

            aValue = value;
            aQuality = (int) quality;
            aReadTime = (DateTime) timeStamp;
        }

        public void WriteValue(object aValue)
        {
            if (aValue == null) {
                throw new ArgumentNullException("aValue");
            }

            if (!IsActive) {
                throw new OpcValueException(
                    "Параметр OPC не активирован, но предпринята попытка его записи: " + Name);
            }

            try {
                opcItem.Write(aValue);
            }
            catch (Exception ex) {
                throw new OpcValueException(string.Format("Ошибка при записи " +
                                                          "значения тега {0} : {1}", Name, ex.Message));
            }
        }

        public void SubscribeToValueChange(IOpcValueListener aListener)
        {
            if (aListener == null) {
                throw new ArgumentNullException("aListener");
            }

            if (!IsListenValueChanging) {
                throw new OpcValueException(
                    "Измените состояние IsListenValueChanging для подписки на изменение значения OPC тега.");
            }            
            
            listeners.Add(aListener);
        }

        public void UnSubscribeToValueChange(IOpcValueListener aListener)
        {
            if (aListener == null) {
                throw new ArgumentNullException("aListener");
            }

            if (listeners != null && listeners.Contains(aListener)) {
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
            var quality = aQualities.GetValue(1);
            var timestamp = aTimestamps.GetValue(1);
            AlertListeners(currentValue, quality, timestamp);
        }

        private void AlertListeners(object aCurrentValue, object aQuality, object aTimestamp)
        {
            var eventArgs = new OpcValueChangedEventArgs(aCurrentValue, (int)aQuality, (DateTime)aTimestamp);
            foreach (var listener in listeners) {                
                listener.OnValueChanged(this, eventArgs);
            }
        }
    }
}
