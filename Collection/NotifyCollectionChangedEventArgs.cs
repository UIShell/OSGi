namespace UIShell.OSGi.Collection
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class NotifyCollectionChangedEventArgs<T> : EventArgs
    {
        private NotifyCollectionChangedAction _action;
        private ReadOnlyCollection<T> _newItems;
        private int _newStartingIndex;
        private ReadOnlyCollection<T> _oldItems;
        private int _oldStartingIndex;

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Reset)
            {
                throw new ArgumentException(SR.GetString("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Reset }), "action");
            }
            InitializeAdd(action, null, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(SR.GetString("MustBeResetAddOrRemoveActionForCtor", new object[0]), "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException(SR.GetString("ResetActionRequiresNullItem", new object[0]), "action");
                }
                InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                InitializeAddOrRemove(action, changedItems, -1);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(SR.GetString("MustBeResetAddOrRemoveActionForCtor", new object[0]), "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException(SR.GetString("ResetActionRequiresNullItem", new object[0]), "action");
                }
                InitializeAdd(action, null, -1);
            }
            else
            {
                InitializeAddOrRemove(action, new T[] { changedItem }, -1);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> newItems, IList<T> oldItems)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(SR.GetString("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            InitializeMoveOrReplace(action, newItems, oldItems, -1, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems, int startingIndex)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(SR.GetString("MustBeResetAddOrRemoveActionForCtor", new object[0]), "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItems != null)
                {
                    throw new ArgumentException(SR.GetString("ResetActionRequiresNullItem", new object[0]), "action");
                }
                if (startingIndex != -1)
                {
                    throw new ArgumentException(SR.GetString("ResetActionRequiresIndexMinus1", new object[0]), "action");
                }
                InitializeAdd(action, null, -1);
            }
            else
            {
                if (changedItems == null)
                {
                    throw new ArgumentNullException("changedItems");
                }
                if (startingIndex < -1)
                {
                    throw new ArgumentException(SR.GetString("IndexCannotBeNegative", new object[0]), "startingIndex");
                }
                InitializeAddOrRemove(action, changedItems, startingIndex);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem, int index)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (((action != NotifyCollectionChangedAction.Add) && (action != NotifyCollectionChangedAction.Remove)) && (action != NotifyCollectionChangedAction.Reset))
            {
                throw new ArgumentException(SR.GetString("MustBeResetAddOrRemoveActionForCtor", new object[0]), "action");
            }
            if (action == NotifyCollectionChangedAction.Reset)
            {
                if (changedItem != null)
                {
                    throw new ArgumentException(SR.GetString("ResetActionRequiresNullItem", new object[0]), "action");
                }
                if (index != -1)
                {
                    throw new ArgumentException(SR.GetString("ResetActionRequiresIndexMinus1", new object[0]), "action");
                }
                InitializeAdd(action, null, -1);
            }
            else
            {
                InitializeAddOrRemove(action, new T[] { changedItem }, index);
            }
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T newItem, T oldItem)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(SR.GetString("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
            }
            InitializeMoveOrReplace(action, new T[] { newItem }, new T[] { oldItem }, -1, -1);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> newItems, IList<T> oldItems, int startingIndex)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(SR.GetString("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
            }
            if (newItems == null)
            {
                throw new ArgumentNullException("newItems");
            }
            if (oldItems == null)
            {
                throw new ArgumentNullException("oldItems");
            }
            InitializeMoveOrReplace(action, newItems, oldItems, startingIndex, startingIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, IList<T> changedItems, int index, int oldIndex)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException(SR.GetString("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Move }), "action");
            }
            if (index < 0)
            {
                throw new ArgumentException(SR.GetString("IndexCannotBeNegative", new object[0]), "index");
            }
            InitializeMoveOrReplace(action, changedItems, changedItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T changedItem, int index, int oldIndex)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Move)
            {
                throw new ArgumentException(SR.GetString("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Move }), "action");
            }
            if (index < 0)
            {
                throw new ArgumentException(SR.GetString("IndexCannotBeNegative", new object[0]), "index");
            }
            T[] newItems = new T[] { changedItem };
            InitializeMoveOrReplace(action, newItems, newItems, index, oldIndex);
        }

        public NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction action, T newItem, T oldItem, int index)
        {
            _newStartingIndex = -1;
            _oldStartingIndex = -1;
            if (action != NotifyCollectionChangedAction.Replace)
            {
                throw new ArgumentException(SR.GetString("WrongActionForCtor", new object[] { NotifyCollectionChangedAction.Replace }), "action");
            }
            InitializeMoveOrReplace(action, new T[] { newItem }, new T[] { oldItem }, index, index);
        }

        private void InitializeAdd(NotifyCollectionChangedAction action, IList<T> newItems, int newStartingIndex)
        {
            _action = action;
            _newItems = (newItems == null) ? null : new ReadOnlyCollection<T>(newItems);
            _newStartingIndex = newStartingIndex;
        }

        private void InitializeAddOrRemove(NotifyCollectionChangedAction action, IList<T> changedItems, int startingIndex)
        {
            if (action == NotifyCollectionChangedAction.Add)
            {
                InitializeAdd(action, changedItems, startingIndex);
            }
            else if (action == NotifyCollectionChangedAction.Remove)
            {
                InitializeRemove(action, changedItems, startingIndex);
            }
        }

        private void InitializeMoveOrReplace(NotifyCollectionChangedAction action, IList<T> newItems, IList<T> oldItems, int startingIndex, int oldStartingIndex)
        {
            InitializeAdd(action, newItems, startingIndex);
            InitializeRemove(action, oldItems, oldStartingIndex);
        }

        private void InitializeRemove(NotifyCollectionChangedAction action, IList<T> oldItems, int oldStartingIndex)
        {
            _action = action;
            _oldItems = (oldItems == null) ? null : new ReadOnlyCollection<T>(oldItems);
            _oldStartingIndex = oldStartingIndex;
        }

        public NotifyCollectionChangedAction Action => _action;

        public ReadOnlyCollection<T> NewItems => _newItems;

        public ReadOnlyCollection<T> OldItems => _oldItems;
    }
}

